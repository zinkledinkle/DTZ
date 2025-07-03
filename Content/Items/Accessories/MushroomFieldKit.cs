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
    public class MushroomFieldKit : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;
            Item.value = Item.buyPrice(gold: 1);
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ModContent.GetInstance<Sporepack>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<GlowingSalve>().UpdateAccessory(player, hideVisual);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlowingSalve>()
                .AddIngredient<Sporepack>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
