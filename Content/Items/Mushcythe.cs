using DTZ.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Items
{
    public class Mushcythe : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 38;
            Item.damage = 8;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.knockBack = 0.1f;
            Item.crit = 0;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(silver: 10);
        }
        public override bool? UseItem(Player player)
        {
            Point16 tileCoords = Main.MouseWorld.ToTileCoordinates16();
            Tile tile = Framing.GetTileSafely(tileCoords);
            if (tile.TileType == TileID.Mud || tile.TileType == TileID.MushroomGrass)
            {
                tile.TileType = (ushort)ModContent.TileType<TilledMud>();
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GlowingMushroom, 15)
                .AddIngredient(ItemID.Sickle)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
