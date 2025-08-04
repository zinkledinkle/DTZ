using Mycology.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Mycology.Content.Items
{
    public class ZinklitePlate_item : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.DefaultToPlaceableTile(ModContent.TileType<ZinklitePlate>());
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient<ZinkliteOre>(2)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
