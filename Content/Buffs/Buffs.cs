using DTZ.Content.Projectiles;
using DTZ.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DTZ.Content.Buffs
{

    // i could NOT be bothered to make 4 seperate .cs files when its lowkey like 8 lines of code per buff :broken heart:
    public class GlowingBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 5;
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
            player.GetCritChance(DamageClass.Generic) += 5;
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
            player.DefenseEffectiveness *= .90f; 
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
            player.moveSpeed += .3f;
            player.jumpSpeedBoost += 3;
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
