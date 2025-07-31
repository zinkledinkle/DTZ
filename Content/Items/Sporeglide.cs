using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mycology.Content.Projectiles;
using Mycology.MycoUtils;
using Mycology.Systems;
using Mycology.Systems.Drawing.Particles;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Mycology.Content.Items
{
    public class Sporeglide : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.width = 60;
            Item.height = 52;
            Item.DamageType = ModContent.GetInstance<PureShroomyDamage>();
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
            Item.shoot = ModContent.ProjectileType<SporeglideProj>();
            Item.noUseGraphic = true;
        }
        public override bool CanUseItem(Player player)
        {
            return player.IsPlayerGrounded() && !player.mount.Active && player.ownedProjectileCounts[Item.shoot] == 0;
        }
    }
    public class SporeglideProj : ModProjectile
    {
        private static Texture2D glow;
        public override string Texture => ModContent.GetInstance<Sporeglide>().Texture;
        private static SoundStyle woosh = new("Mycology/Assets/Sounds/Items/Sporeglide_Launch", SoundType.Sound)
        {
            MaxInstances = 0,
            Volume = 1.1f,
            PitchVariance = 0.1f
        };
        public override void Load()
        {
            if (!Main.dedServ)
            {
                glow = ModContent.Request<Texture2D>(Texture + "_glow", AssetRequestMode.ImmediateLoad).Value;
            }
            On_Player.QuickMount += Release;
        }
        public override void Unload()
        {
            On_Player.QuickMount -= Release;
        }
        private void Release(On_Player.orig_QuickMount orig, Player self)
        {
            if (self.ownedProjectileCounts[ModContent.ProjectileType<SporeglideProj>()] > 0
                && Main.projectile.Where(p => p.owner == self.whoAmI 
                && p.type == ModContent.ProjectileType<SporeglideProj>()).FirstOrDefault().ModProjectile is SporeglideProj sg 
                && !sg.closing)
            {
                return;
            }
            orig(self);
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 52;
            Projectile.timeLeft = 10000;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 0;
        }
        private float strength = 40;
        public bool closing = false;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (closing)
            {
                player.fullRotation = 0;
                Projectile.velocity += new Vector2(0, -0.2f);
                if (Projectile.timeLeft > 30)
                {
                    Projectile.timeLeft = 30;
                }
                Projectile.scale = Projectile.timeLeft / 30f;
                return;
            }

            squash = MathHelper.Lerp(squash, player.controlDown ? 0.6f : 1, 0.25f);

            Vector2 offset = new Vector2(0, -28).RotatedBy(Projectile.rotation, new Vector2(0, -10));
            Projectile.Center = player.Center + offset;
            Projectile.Center += new Vector2(0, (1 - squash) * 52 / 2);
            if (Projectile.ai[0] == 0)
            {
                SoundEngine.PlaySound(woosh, Projectile.Center);
                for (int i = 0; i < 30; i++)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Particle.NewParticle(Particle.ParticleID.Wind, Projectile.Center, new Vector2(Main.rand.NextFloat(-3,3), Main.rand.NextFloat(-3, -5)), Color.White, 0f, 1f);
                    }
                    if (Main.rand.NextBool(5))
                    {
                        Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(-3, -5), Main.rand.Next(11, 14));
                        Dust.NewDust(Projectile.Center, 4, 4, DustID.Cloud, 0, 3);
                    }
                }
            }

            if (Projectile.ai[0] % 15 == 0)
            {
                Vector2 position = Projectile.position;
                position.Y += Projectile.height - 10;
                position.X += Main.rand.NextFloat(-30, 30) * Projectile.scale;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, new Vector2(Main.rand.NextFloatDirection(), 3), ModContent.ProjectileType<ShroombrellaSpore>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1, 0.3f);
            strength = MathHelper.Lerp(strength, 0, 0.65f);

            if (strength > 2)
            {
                player.velocity.X *= 0.7f;
            }
            player.velocity.Y -= strength;
            if (player.mount.Active) player.mount.Dismount(player);
            if (player.controlMount) closing = true;
            if (player.IsPlayerGrounded() && strength < 1)
            {
                closing = true;
            }
            Projectile.rotation = Math.Min(Math.Abs(player.velocity.X / 20), 0.3f) * Math.Sign(player.velocity.X);
            player.fullRotation = Projectile.rotation;

            float angle1 = (-0.1f * player.direction) + Projectile.rotation + MathHelper.Pi;
            float angle2 = (0.1f * player.direction) + Projectile.rotation + MathHelper.Pi;
            if (player.itemAnimation == 0 || player.HeldItem.type == ModContent.ItemType<Sporeglide>())
            {
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, angle1);
            }
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, angle2);

            Projectile.ai[0]++;
        }
        private float squash = 1;
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = new(texture.Width/2, 40);
            Vector2 glowOrigin = new(glow.Width / 2, 55);

            SpriteEffects flip = Main.player[Projectile.owner].direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float sin = MathF.Sin((float)Main.timeForVisualEffects * MathHelper.TwoPi / 60f / 2);
            sin /= 2;
            sin = MathHelper.Lerp(0.1f, 0.3f, sin);

            Vector2 scale = new Vector2(1, squash) * Projectile.scale;
            Main.spriteBatch.Draw(glow, drawPos, null, Color.LightBlue * sin, Projectile.rotation, glowOrigin, scale, flip, 1f);
            Main.spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, origin, scale, flip, 1f);
            return false;
        }
    }
}
