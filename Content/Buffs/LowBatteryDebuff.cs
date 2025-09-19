using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Mycology.Content.Buffs;
using Mycology;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mycology.Systems;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mycology.Content.Projectiles;
using Mycology.Content.Enemies;

namespace Mycology.Content.Buffs
{
    public class LowBatteryDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed *= 0.01f;

            if (player.buffTime[buffIndex] == 1)
            {
                if (!player.GetModPlayer<LowBatteryPlayer>().isProtected)
                {
                    player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " forgot to get AA instead of AAA batteries."), 9999, 0);
                }
            }
        }
    }
}