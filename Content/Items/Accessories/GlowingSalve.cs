using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Items.Accessories
{
    public class GlowingSalve : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(silver: 50);
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MycologyPlayer mPlr = player.GetModPlayer<MycologyPlayer>();
            mPlr.MushionHealAmount = (int)(mPlr.MushionHealAmount * 1.25f);
        }
    }
}
