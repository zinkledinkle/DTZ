using DTZ.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace DTZ.Content.Items
{
    public class ZinliteOre : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            // Item.DefaultToPlaceableTile(ModContent.TileType<ZinkliteOre>());
        }
    }
}
