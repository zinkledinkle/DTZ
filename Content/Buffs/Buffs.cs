using DTZ.Content.Projectiles;
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
            player.lifeRegen += 5 * player.ownedProjectileCounts[ModContent.ProjectileType<GlowingMushion>()];
        }
    }

    public class HellcapBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetCritChance(DamageClass.Generic) += 2 * player.ownedProjectileCounts[ModContent.ProjectileType<HellcapMushion>()];
        }
    }
    public class IceliumBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.DefenseEffectiveness *= 1f - (.075f *player.ownedProjectileCounts[ModContent.ProjectileType<IceliumMushion>()]); 
        }
    }
    public class ToadBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += .3f * player.ownedProjectileCounts[ModContent.ProjectileType<ToadMushion>()];
            player.jumpSpeedBoost += 1 * player.ownedProjectileCounts[ModContent.ProjectileType<ToadMushion>()];
        }
    }
}
