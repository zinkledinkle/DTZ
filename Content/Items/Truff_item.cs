using Mycology.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Mycology.Content.Items
{
    public class Truff_item : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.DefaultToPlaceableTile(ModContent.TileType<Truff>());
        }
    }
}
