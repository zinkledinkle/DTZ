using DTZ.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Items
{
    public class MushionSeeds : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(silver: 50);
            Item.DefaultToPlaceableTile(ModContent.TileEntityType<MushionSeedsGrowth>());
        }
    }
}
