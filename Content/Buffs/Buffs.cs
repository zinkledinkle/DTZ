using DTZ.Content.Projectiles;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace DTZ.Content.Buffs
{

    // i could NOT be bothered to make 4 seperate .cs files when its lowkey like 8 lines of code per buff :broken heart:
    public class GlowingBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 5 * player.ownedProjectileCounts[ModContent.ProjectileType<GlowingMushion>()];
        }
        public override bool RightClick(int buffIndex)
        {
            Projectile[] mushions = Main.projectile.Where(p => p.type == ModContent.ProjectileType<GlowingMushion>() && p.owner == Main.myPlayer).ToArray();
            Projectile first = mushions.FirstOrDefault();
            first?.Kill();
            return base.RightClick(buffIndex);
        }
    }

    public class HellcapBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetCritChance(DamageClass.Generic) += 2 * player.ownedProjectileCounts[ModContent.ProjectileType<HellcapMushion>()];
        }
        public override bool RightClick(int buffIndex)
        {
            Projectile[] mushions = Main.projectile.Where(p => p.type == ModContent.ProjectileType<HellcapMushion>() && p.owner == Main.myPlayer).ToArray();
            Projectile first = mushions.FirstOrDefault();
            first?.Kill();
            return base.RightClick(buffIndex);
        }
    }
    public class IceliumBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.DefenseEffectiveness *= 1f - (.075f *player.ownedProjectileCounts[ModContent.ProjectileType<IceliumMushion>()]); 
        }
        public override bool RightClick(int buffIndex)
        {
            Projectile[] mushions = Main.projectile.Where(p => p.type == ModContent.ProjectileType<IceliumMushion>() && p.owner == Main.myPlayer).ToArray();
            Projectile first = mushions.FirstOrDefault();
            first?.Kill();
            return base.RightClick(buffIndex);
        }
    }
    public class ToadBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += .3f * player.ownedProjectileCounts[ModContent.ProjectileType<ToadMushion>()];
            player.jumpSpeedBoost += 1 * player.ownedProjectileCounts[ModContent.ProjectileType<ToadMushion>()];
        }
        public override bool RightClick(int buffIndex)
        {
            Projectile[] mushions = Main.projectile.Where(p => p.type == ModContent.ProjectileType<ToadMushion>() && p.owner == Main.myPlayer).ToArray();
            Projectile first = mushions.FirstOrDefault();
            first?.Kill();
            return base.RightClick(buffIndex);
        }
    }
}
