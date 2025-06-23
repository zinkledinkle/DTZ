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
    }

    public class HellcapBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetCritChance(DamageClass.Generic) += 5;
        }
    }
    public class IceliumBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.DefenseEffectiveness *= .90f; 
        }
    }
    public class ToadBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += .3f;
            player.jumpSpeedBoost += 3;
        }
    }
}
