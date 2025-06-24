using DTZ.Content.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DTZ.Content.Items
{
    public class MoonlordUnderworldBunkFungus : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.DefaultToFood(26, 28, ModContent.BuffType<FungusBuff>(), 6000, false, 100);
            Item.value = Item.buyPrice(gold: 50);
        }
    }
}
