using Mycology.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Mycology.Content.Items
{
    public class ToadstoolSeeds : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(gold: 1);
            Item.DefaultToPlaceableTile(ModContent.TileType<MushionSeedsTileToad>());
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient<MushionSeeds>()
                .AddIngredient<Toadstool>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
