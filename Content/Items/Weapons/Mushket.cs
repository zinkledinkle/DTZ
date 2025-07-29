using Mycology.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Mycology.Content.Items.Weapons
{
    public class Mushket : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 40;

            Item.DamageType = ModContent.GetInstance<ShroomyRanged>();
            Item.damage = 26;
            Item.crit = 5;
            Item.knockBack = 3f;

            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 40);

            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<MushketProj>();
            Item.useAmmo = ItemID.Mushroom;
            Item.shootSpeed = 20;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, Vector2.Zero, Item.shoot, damage, knockback); //why did I have to do this because of the ammo type?? relogic die
            return false;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;
    }
    public class MushketProj : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<Mushket>().Texture;
        private static Texture2D muzzleFlashTex;
        public override void Load() {
            if (!Main.dedServ)
            {
                muzzleFlashTex = ModContent.Request<Texture2D>("Mycology/Assets/Textures/MuzzleFlash", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
        }
        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.DamageType = ModContent.GetInstance<ShroomyRanged>();
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netUpdate = true;
        }
        SpriteEffects flip = SpriteEffects.None;
        float muzzleFlashAlpha = 0f;
        float recoil = 0;
        float lerpedRecoil = 0; //thats right we DOUBLE lerping ts //this sucks probably
        Vector2 offset = Vector2.Zero;
        public override void AI()
        {
            Player plr = Main.player[Projectile.owner];
            Item item = plr.HeldItem.type == ModContent.ItemType<Mushket>() ? plr.HeldItem : null;
            if (item == null) return;
            int maxTime = item.useTime;

            muzzleFlashAlpha = MathHelper.Lerp(muzzleFlashAlpha, 0, 0.08f);
            recoil = MathHelper.Lerp(recoil, 0, 0.6f);

            if (plr.channel && item != null)
            {
                Projectile.timeLeft = 2;
                plr.heldProj = Projectile.whoAmI;

                offset = plr.MountedCenter.DirectionTo(Main.MouseWorld);
                plr.direction = Math.Sign(offset.X);

                lerpedRecoil = MathHelper.Lerp(lerpedRecoil, recoil, 0.5f);
                Projectile.Center = plr.MountedCenter + offset * (20 - lerpedRecoil);
                float rot = offset.ToRotation();
                Projectile.rotation = rot;
                plr.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot - MathHelper.PiOver2);

                flip = offset.X < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;

                if (Projectile.ai[0] == 0)
                {
                    plr.PickAmmo(item, out int projID, out float shootSpeed, out int damage, out float knockBack, out int AmmoItemID);
                    if (Main.rand.NextBool(20)) if (!plr.ConsumeItem(AmmoItemID)) return;

                    if (projID == ModContent.ProjectileType<ToadstoolShot>() && Main.rand.NextBool(1000000)) projID = ModContent.ProjectileType<GUNKstoolShot>();

                    Vector2 spawnPos = Projectile.Center + offset * 22;
                    if (!Collision.CanHitLine(plr.Center, 5, 5, spawnPos + offset * 10, 5, 5))
                    {
                        spawnPos = plr.Center;
                    }

                    Projectile.NewProjectile(plr.GetSource_FromThis(), spawnPos, 
                        offset * shootSpeed, projID, damage, knockBack, Projectile.owner);
                    muzzleFlashAlpha = 1;
                    recoil = 25;
                    SoundEngine.PlaySound(SoundID.Item61, Projectile.Center);
                }

                Projectile.ai[0] += Projectile.ai[0] <= maxTime ? 1 : -maxTime -1;
            }
            else Projectile.Kill();
            Projectile.netUpdate = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 origin = tex.Size() / 2;
            Vector2 drawpos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(tex, drawpos, null, lightColor, Projectile.rotation, origin, Projectile.scale, flip, 1f);
            Vector2 muzzleFlashOrigin = muzzleFlashTex.Size() / 2;
            Vector2 muzzleFlashDrawpos = (Projectile.Center + offset * 26) - Main.screenPosition;

            Color muzzleFlashColor = Color.White * muzzleFlashAlpha;
            muzzleFlashColor.A = 0;
            Main.spriteBatch.Draw(muzzleFlashTex, muzzleFlashDrawpos, null, muzzleFlashColor, Projectile.rotation, muzzleFlashOrigin , muzzleFlashAlpha, SpriteEffects.None, 1f);
            return false;
        }
    }
    #region mushroom projectiles
    public abstract class MushroomShotBase : ModProjectile
    {
        protected virtual int TrailLength { get; } = 10;
        protected virtual float Weight { get; } = 0.2f;
        protected virtual float RotationSpeed { get; } = 0.1f;
        protected virtual int DustType { get; } = 0;
        public override string Texture => "Terraria/Images/Item_5";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = TrailLength;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
        }
        public override void AI()
        {
            Projectile.netUpdate = true;
            Projectile.velocity.Y += Weight;
            Projectile.velocity.X *= (1 - Weight / 10);
            Projectile.rotation += RotationSpeed;

            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.Center, 1, 1, DustType);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int count = Projectile.oldPos.Length;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() / 2;
            for (int i = 0; i < count; i++)
            {
                float alpha = 1 - (i / (float)count);
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2;
                drawPos -= Main.screenPosition;
                Main.spriteBatch.Draw(texture, drawPos, null, lightColor * alpha, Projectile.oldRot[i], origin, Projectile.scale * alpha, SpriteEffects.None, 1f);
            }
            return base.PreDraw(ref lightColor);
        }
        public override void OnKill(int timeLeft)
        {
            if (DustType == 0) return;
            for (int i = 0; i < Main.rand.Next(3, 5); i++)
            {
                Dust.NewDust(Projectile.Center, 1, 1, DustType, Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2));
            }
        }
    }
    public class MushroomShot : MushroomShotBase
    {
        protected override float Weight => 0.15f;
    }
    public class GlowingMushroomShot : MushroomShotBase
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.GlowingMushroom;
        protected override float Weight => 0.15f;
        protected override int TrailLength => 20;
        protected override float RotationSpeed => 0.05f;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate += 4;
        }
        protected override int DustType => DustID.GlowingMushroom;
    }
    public class VileMushroomShot : MushroomShotBase
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.VileMushroom;
        protected override float Weight => 0.35f;
        protected override int TrailLength => 10;
        protected override float RotationSpeed => 0.2f;
        protected override int DustType => DustID.VilePowder;
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < Main.rand.Next(4, 7); i++)
            {
                Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(5, 5), ModContent.ProjectileType<VileMushroomShotPiece>(), Projectile.damage / 4, 0.1f, Projectile.owner);
            }
        }
        private class VileMushroomShotPiece : ModProjectile
        {
            public override string Texture => "Terraria/Images/Item_" + ItemID.VileMushroom;
            public override void SetDefaults()
            {
                Projectile.width = 4;
                Projectile.height = 4;
                Projectile.friendly = true;
                Projectile.penetrate = 2;
                Projectile.scale = 0.5f;
            }
            public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
            {
                target.AddBuff(BuffID.Poisoned, 120);
            }
            public override void AI()
            {
                Projectile.rotation += 0.4f;
                Projectile.velocity.Y += 0.2f;
                Projectile.velocity.X *= 0.97f;
            }
        }
    }
    public class ViciousMushroomShot : MushroomShotBase
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.ViciousMushroom;
        protected override float Weight => 0.25f;
        protected override int TrailLength => 15;
        protected override float RotationSpeed => 0.3f;
        protected override int DustType => DustID.Blood;
        public override void AI()
        {
            base.AI();
            if (Projectile.ai[0] % 7 == 0)
            {
                Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromThis(), Projectile.Center, new Vector2(0, 7), ProjectileID.BloodRain, Projectile.damage / 4, 0.1f, Projectile.owner);
            }
            Projectile.ai[0]++;
        }
    }
    #endregion
    public class MushroomShotGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            switch (entity.type)
            {
                case ItemID.Mushroom:
                    entity.damage = 14;
                    entity.DamageType = ModContent.GetInstance<ShroomyRanged>();
                    entity.ammo = entity.type;
                    entity.shoot = ModContent.ProjectileType<MushroomShot>();
                    break;
                case ItemID.GlowingMushroom:
                    entity.damage = 10;
                    entity.DamageType = ModContent.GetInstance<ShroomyRanged>();
                    entity.ammo = ItemID.Mushroom;
                    entity.shoot = ModContent.ProjectileType<GlowingMushroomShot>();
                    entity.shootSpeed = 8;
                    break;
                case ItemID.VileMushroom:
                    entity.damage = 19;
                    entity.DamageType = ModContent.GetInstance<ShroomyRanged>();
                    entity.ammo = ItemID.Mushroom;
                    entity.shoot = ModContent.ProjectileType<VileMushroomShot>();
                    entity.shootSpeed = -4;
                    break;
                case ItemID.ViciousMushroom:
                    entity.damage = 8;
                    entity.DamageType = ModContent.GetInstance<ShroomyRanged>();
                    entity.ammo = ItemID.Mushroom;
                    entity.shoot = ModContent.ProjectileType<ViciousMushroomShot>();
                    entity.shootSpeed = 3;
                    break;
            }
        }
    }
}
