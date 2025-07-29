using Mycology.Systems;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Mycology.Content.Projectiles;
using Terraria.Audio;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace Mycology.Content.Items.Weapons
{
    public class Shroombrella : ModItem
    {
        public static Texture2D glow;
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                glow = ModContent.Request<Texture2D>(Texture + "_glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
        }
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 52;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 50);

            Item.damage = 35;
            Item.DamageType = ModContent.GetInstance<ShroomyMelee>();
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.channel = true;
            Item.knockBack = 9f;
            Item.noUseGraphic = true;

            Item.holdStyle = ItemHoldStyleID.HoldUp;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<ShroombrellaProj>();
        }
        public bool floaty = false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, Item.useTime, 0, 1);
            return false;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
        public override void UpdateInventory(Player player)
        {
            int count = player.ownedProjectileCounts[Item.shoot];
            floaty = !player.controlDown && count == 0;
        }
        private int timer = 0;
        public override void HoldItem(Player player)
        {
            if (!floaty)
            {
                return;
            }

            if (timer % 5 == 0) Dust.NewDust(player.Center + new Vector2(0, -30), 1, 1, DustID.MushroomTorch);
            if (timer == 20)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + new Vector2(0, -30), new Vector2(Main.rand.NextFloatDirection(), 5), ModContent.ProjectileType<ShroombrellaSpore>(), Item.damage, Item.knockBack, player.whoAmI);
            }
            timer += timer < 20 ? 1 : -21;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Umbrella)
                .AddIngredient(ItemID.GlowingMushroom, 50)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public class ShroombrellaDrawLayer : PlayerDrawLayer
        {
            public override void Load()
            {
                base.Load();
            }
            public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ArmOverItem);
            public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
            {
                return drawInfo.drawPlayer.HeldItem.ModItem is Shroombrella && drawInfo.drawPlayer.ownedProjectileCounts[ModContent.ProjectileType<ShroombrellaProj>()] == 0;
            }
            private float rotation;
            private Vector2 offset = new(10, 25);
            private bool debounce = false;
            protected override void Draw(ref PlayerDrawSet drawInfo)
            {
                Player player = drawInfo.drawPlayer;

                rotation = MathHelper.Lerp(rotation, player.controlDown ?
                MathHelper.PiOver2 :
                0, 0.5f);
                offset = Vector2.Lerp(offset, player.controlDown ?
                    new Vector2(20, -12) :
                    new Vector2(8, -30), 0.5f);

                Vector2 newOffset = offset;
                newOffset.X *= player.direction;
                float newRotation = rotation * player.direction;

                Vector2 drawPos = new(
                    (int)player.MountedCenter.X - (int)Main.screenPosition.X + newOffset.X,
                    (int)player.MountedCenter.Y - (int)Main.screenPosition.Y + newOffset.Y);

                Texture2D texture = TextureAssets.Item[ModContent.ItemType<Shroombrella>()].Value;
                Texture2D glow = Shroombrella.glow;
                SpriteEffects flip = player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                float glowSine = MathF.Sin((float)(Main.timeForVisualEffects / 60f) * MathHelper.TwoPi / 5); //5 second loop
                Color color = Color.Lerp(Color.Blue, Color.LightBlue, (glowSine + 1) / 2f);
                drawInfo.DrawDataCache.Add(
                    new(glow, drawPos, null, color, newRotation, glow.Size() / 2, player.HeldItem.scale, flip));
                drawInfo.DrawDataCache.Add(
                    new(texture, drawPos, null, Color.White, newRotation, texture.Size() / 2, player.HeldItem.scale, flip, 1f));
            }
        }
    }
    public class ShroombrellaProj : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<Shroombrella>().Texture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 4;
        }
        /// store attack time in ai[0], timer is [1]
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<ShroomyMelee>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Projectile.ai[0];
        }
        private int direction = 1;
        private float startRotation = 0;
        private Vector2 offset;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.HeldItem.type == ModContent.ItemType<Shroombrella>())
            {
                //owner.HeldItem.noUseGraphic = true;
                owner.heldProj = Projectile.whoAmI;
                Projectile.knockBack = owner.HeldItem.knockBack;
                if (Projectile.ai[1] == 0)
                {
                    offset = Main.MouseWorld - owner.MountedCenter;
                    startRotation = MathF.Atan2(offset.Y, offset.X);
                    direction = (int)Projectile.ai[2];
                    Projectile.timeLeft = (int)Projectile.ai[0];
                    SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                }

                Lighting.AddLight(Projectile.Center, TorchID.Mushroom);

                float baseScale = owner.HeldItem.scale;

                float timeMax = Projectile.ai[0];
                float progress = Projectile.ai[1] / timeMax;
                progress = MathF.Min(progress, 1);

                owner.direction = Math.Sign(offset.X);

                float swingRadius = MathHelper.PiOver2 * 1.4f;

                float startRot = startRotation + (swingRadius * direction);
                float endRot = startRotation - (swingRadius * direction);

                float easeExpo = (float)Math.Pow(progress, 2.5f);
                float curRotation = MathHelper.Lerp(startRot, endRot, easeExpo);

                Player.CompositeArmStretchAmount stretchAmount = easeExpo switch
                {
                    < 0.1f => Player.CompositeArmStretchAmount.Quarter,
                    < 0.3f => Player.CompositeArmStretchAmount.ThreeQuarters,
                    < 0.8f => Player.CompositeArmStretchAmount.Full,
                    _ => Player.CompositeArmStretchAmount.Quarter,
                };

                owner.SetCompositeArmFront(true, stretchAmount, curRotation - MathHelper.PiOver2);

                float idkwhattocallthis = SegmentLerp(easeExpo, 0.5f, 0f, 1.5f);

                Projectile.Opacity = idkwhattocallthis;
                Projectile.scale = baseScale * idkwhattocallthis;

                Projectile.rotation = curRotation;
                float distance = 35 * baseScale * (idkwhattocallthis);
                Projectile.Center = owner.MountedCenter + curRotation.ToRotationVector2() * distance;

                Projectile.ai[1]++;
                owner.SetDummyItemTime(Projectile.timeLeft);

                if (Projectile.timeLeft == 1 && owner.channel)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, timeMax, 0, -direction);
                }
            } else
            {
                Projectile.Kill();
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Math.Sign((target.Center - Main.player[Projectile.owner].Center).X);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < Main.rand.Next(2,4); i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Main.rand.NextVector2CircularEdge(10, 10), ModContent.ProjectileType<ShroombrellaSpore>(), damageDone / 2, 0, Projectile.owner);
            }
            for (int i = 0; i < Main.rand.Next(4, 8); i++)
            {
                Dust.NewDust(target.Center, 1, 1, DustID.GlowingMushroom, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, -5));
            }
        }
        private static float SegmentLerp(float progress, float riseEnd, float lowValue, float highValue)
        {
            if (progress <= riseEnd)
                return lowValue + (progress / riseEnd) * (highValue - lowValue);
            else if (progress <= 1f)
                return highValue - ((progress - riseEnd) / (1f - riseEnd)) * (highValue - lowValue);
            else
                return lowValue;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int amount = Projectile.oldPos.Length;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size()/2;
            Color color = Projectile.GetAlpha(lightColor);
            for (int i = 0; i < amount; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + new Vector2(Projectile.width / 2, Projectile.height / 2);
                float alpha = 1f - (i / (float)amount);
                Main.EntitySpriteDraw(texture, drawPos, null, (color * alpha) * 0.1f, Projectile.oldRot[i] + MathHelper.PiOver2, origin, Projectile.scale * MathF.Pow(alpha, 3), SpriteEffects.None, 0);
            }
            Color glowColor = Color.LightBlue;
            glowColor *= Projectile.Opacity / 2;
            Main.spriteBatch.Draw(Shroombrella.glow, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation + MathHelper.PiOver2, Shroombrella.glow.Size()/2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
