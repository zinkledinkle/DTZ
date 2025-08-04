using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Mycology.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Mycology.Content.Items.Accessories
{
    public class MycelialParasite : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Mushroom;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.damage = 15;
            Item.DamageType = ModContent.GetInstance<PureShroomyDamage>();
            Item.expert = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Item.damage = player.name == "GUNK" ? 300 : 15;

            player.GetModPlayer<MycelialParasitePlayer>().Active = true;
            player.lifeRegen = 0;
            player.runAcceleration += 0.2f;
            player.moveSpeed += 1f;
            player.statDefense *= 0.7f;
            player.jumpSpeedBoost += 1.5f;
            player.maxFallSpeed += 1f;
        }
    }
    public class MycelialParasitePlayer : ModPlayer
    {
        public bool Active = false;
        private int AttackTime = 0;
        public static float Radius = 300;
        private static int Max = 5;
        public float ProjectileCount = 0;
        public override void PreUpdate()
        {
            ProjectileCount = Player.ownedProjectileCounts[ModContent.ProjectileType<MycelialParasiteTentacle>()];
        }
        public override void ResetEffects()
        {
            Max = Player.name == "GUNK" ? 25 : 5;
            Radius = Player.name == "GUNK" ? 1000 : 300;
            AttackTime = Math.Max(AttackTime - 1, 0);
            int projectileType = ModContent.ProjectileType<MycelialParasiteTentacle>();
            if (Active)
            {
                if (AttackTime == 0 && Player.controlUseItem && Player.HeldItem.damage > 0 &&
                    ProjectileCount < Max) //while attacking
                {
                    AttackTime = Player.name == "GUNK" ? 1 : 30;
                    Item accessory = Player.armor.FirstOrDefault(i => i.type == ModContent.ItemType<MycelialParasite>());
                    int damage = accessory != null ? Player.GetWeaponDamage(accessory) : 30;
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, projectileType, damage, 0, Player.whoAmI);
                }
            }
            else
            {
                foreach (Projectile p in Main.projectile.Where(p => p.owner == Player.whoAmI && p.type == projectileType)) p.Kill();
            }
            while (ProjectileCount > Max)
            {
                Projectile first = Main.projectile.Where(p => p.type == projectileType && p.owner == Player.whoAmI).FirstOrDefault();
                first.Kill();
                ProjectileCount--;
            }

            Active = false;
        }
    }
    public class MycelialParasiteTentacle : ModProjectile
    {
        public override string Texture => "Mycology/Content/Projectiles/TEST";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        private Player Owner => Main.player[Projectile.owner];
        private int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private int AttackTimer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private int HealTimer
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        private bool DoingAttack = false;
        private NPC stuckNPC = null;
        private Vector2 StuckOffset = Vector2.Zero;
        private float initialStuckRotation = 0f;
        private Vector2 AttackPosition = Vector2.Zero;
        private Vector2 ChillPosition = Vector2.Zero;
        private float Radius => MycelialParasitePlayer.Radius;
        private bool Retracting = false;
        private NPC GetTarget()
        {
            if (Owner.MinionAttackTargetNPC > -1 && Main.npc[Owner.MinionAttackTargetNPC].active)
            {
                return Main.npc[Owner.MinionAttackTargetNPC];
            }
            List<NPC> orderedByDistance = [.. Main.npc
                //.Where(npc => npc.CanBeChasedBy(this, false))
                .OrderBy(npc => npc.Distance(Main.MouseWorld))];

            NPC closest = orderedByDistance.FirstOrDefault();
            if (closest == null || closest.Distance(Owner.Center) > Radius || !closest.active)
            {
                NPC closestToPlayer = Main.npc.OrderBy(n => n.Distance(Owner.Center)).Where(n => n.active && n.Distance(Owner.Center) < Radius && n.CanBeChasedBy(this, false)).FirstOrDefault();
                return closestToPlayer;
            }
            return closest;
        }
        private int GetIndex()
        {
            int index = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == Projectile.type && proj.owner == Projectile.owner)
                {
                    if (proj == Projectile)
                        return index;
                    index++;
                }
            }
            return -1; 
        }
        private int SuckBulgeThingHaha = -1;
        float distance = 100;
        public override void AI()
        {
            int index = GetIndex();

            float angleIncrement = MathHelper.TwoPi / (float)Owner.ownedProjectileCounts[Type];
            float angle = angleIncrement * index;
            float restDistance = Owner.name == "GUNK" ? 300 : 100;
            float attackDistance = restDistance * 1.5f;

            Projectile.timeLeft = 2;
            Projectile.friendly = DoingAttack;

            if (SuckBulgeThingHaha >= 0)
            {
                SuckBulgeThingHaha++;
            }

            NPC target = GetTarget();
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Retracting)
            {
                Timer++;
                float multiplier = MathHelper.Clamp(Timer / 30f, 0, 0.9f);
                Projectile.velocity = Vector2.Lerp(Projectile.Center, Owner.Center, 0.1f + multiplier) - Projectile.Center;
                if (Projectile.Distance(Owner.Center) < 15f)
                {
                    Projectile.Kill();
                }
                return;
            }
            if (stuckNPC != null && stuckNPC.active)
            {
                DoingAttack = false;
                float rotate = MathHelper.WrapAngle(stuckNPC.rotation - initialStuckRotation);
                Vector2 rotatedStuckOffset = StuckOffset.RotatedBy(rotate);
                Projectile.Center = stuckNPC.Center + rotatedStuckOffset;
                Projectile.rotation = rotatedStuckOffset.ToRotation() - MathHelper.PiOver2;
                Projectile.velocity = Vector2.Zero;
                HealTimer++;
                if (HealTimer % (Owner.name == "GUNK" ? 5 : 90) == 0)
                {
                    bool crit = Main.rand.NextBool((int)Owner.GetCritChance(DamageClass.Generic), 100);
                    NPC.HitInfo hitInfo = stuckNPC.CalculateHitInfo(Projectile.damage, 0, crit, 0, ModContent.GetInstance<PureShroomyDamage>(), true);
                    Owner.StrikeNPCDirect(stuckNPC, hitInfo);
                    int damage = hitInfo.Damage;
                    int heal = (int)((float)damage * 0.1f);
                    if (heal > 0) Owner.Heal(heal);
                    SuckBulgeThingHaha = 0;
                }
                if (HealTimer > (Owner.name == "GUNK" ? 2000 : 270) || !stuckNPC.active || Projectile.Distance(Owner.Center) > Radius * 1.3f)
                {
                    Retracting = true;
                    Timer = 0;
                    return;
                }
                return;
            }
            if (DoingAttack)
            {
                if (target == null || Projectile.Distance(Owner.Center) > Radius * 1.3f)
                {
                    DoingAttack = false;
                } else
                {
                    float multiplier = AttackTimer / 60f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * 30, 0.05f + multiplier);
                    AttackTimer++;
                }
                return;
            }

            if (target != null)
            {
                distance = MathHelper.Lerp(distance, attackDistance, 0.3f);
                distance += AttackTimer;
                Projectile.rotation = Projectile.DirectionTo(target.Center).ToRotation() + MathHelper.PiOver2;
                AttackTimer++;
                if (AttackTimer >= 60 && Timer % 30 == 0)
                {
                    DoingAttack = true;
                    AttackTimer = 0;
                }
            } else
            {
                distance = MathHelper.Lerp(distance, restDistance, 0.3f);
            }
            Vector2 offset = new(0, -distance);
            float sin = MathF.Sin((Timer / 180f) * MathHelper.TwoPi);
            float cos = MathF.Cos((Timer / 180f) * MathHelper.TwoPi);
            Vector2 bob = new(30 * cos, 10 * sin * cos);

            offset += bob;

            Vector2 rotatedOffset = offset.RotatedBy(angle);
            ChillPosition = Owner.Center + rotatedOffset;
            Projectile.velocity = Vector2.Lerp(Projectile.Center, ChillPosition, 0.2f) - Projectile.Center;

            Timer++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            stuckNPC = target;
            Vector2 lastCenter = Projectile.oldPos[2] + Projectile.Size / 2f;
            Vector2 closestPoint = target.Hitbox.ClosestPointInRect(lastCenter);
            StuckOffset = closestPoint - target.Center;
            initialStuckRotation = target.rotation;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D chain = TextureAssets.Chain24.Value;
            Vector2 chainOrigin = chain.Size() / 2f;
            int amount = (int)(Vector2.Distance(Projectile.Center, Owner.Center) / chain.Height);
            if (SuckBulgeThingHaha > amount) SuckBulgeThingHaha = -1;

            for (int i = 0; i < amount; i++)
            {
                Vector2 position = Vector2.Lerp(Projectile.Center, Owner.Center, (float)i / amount);
                float rotation = (Owner.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2;
                float scale = i == SuckBulgeThingHaha ? 1.5f : 1f;
                if (Math.Abs(i - SuckBulgeThingHaha) == 1) scale = 1.25f;
                Main.spriteBatch.Draw(chain, position - Main.screenPosition, null, lightColor, rotation, chainOrigin, scale, SpriteEffects.None, 0f);
            }

            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 origin = tex.Size() / 2;
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPosition = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size/2f;
                float alpha = 1 - i / (float)Projectile.oldPos.Length;
                Main.spriteBatch.Draw(tex, drawPosition, null, lightColor * alpha, Projectile.oldRot[i], origin, Projectile.scale * alpha, SpriteEffects.None, 1f);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 1f);

            if (GetIndex() == 0)
            {
                float ringamount = 40;
                for (int i = 0; i < ringamount; i++)
                {
                    float rotation = ((i / ringamount) + (float)Main.timeForVisualEffects/300f) * MathHelper.TwoPi;
                    float radius = MycelialParasitePlayer.Radius;
                    Vector2 pos = new Vector2(0, -radius).RotatedBy(rotation) + Owner.Center;
                    Texture2D shroom = TextureAssets.Item[ItemID.GlowingMushroom].Value;
                    Vector2 shrorigin = shroom.Size() / 2f;
                    Main.spriteBatch.Draw(shroom, pos - Main.screenPosition, null, new Color(255,255,255, 0) * 0.5f, 0f, shrorigin, 0.7f, SpriteEffects.None, 1f);
                }
            }
            
            return false;
        }
    }
}
