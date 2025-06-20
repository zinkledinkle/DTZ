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
            Item.useTime = 20;
            Item.useAnimation = 20;
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
    }
}
