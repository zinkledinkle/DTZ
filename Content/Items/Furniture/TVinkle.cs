using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace DTZ.Content.Items.Furniture
{
    public class TVinkle : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.ExampleTable>());
            Item.value = 150;
            Item.maxStack = 99;
            Item.width = 68;
            Item.height = 56;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient<ZinkliteBar>(5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}