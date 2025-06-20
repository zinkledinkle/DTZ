using DTZ.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Systems
{
    public class ShroomGen : ModSystem
    {
        private static Dictionary<int, int[]> shroomTileDict = new()
        {
            { ModContent.TileType<HellcapTile>(), HellcapTile.validTiles },
            { ModContent.TileType<ToadstoolTile>(), ToadstoolTile.validTiles },
            { ModContent.TileType<Iceshroom>(), Iceshroom.validTiles },
        };
        private static int[] shroomTileList = shroomTileDict.Keys.ToArray();
        public override void PostWorldGen()
        {
            PlaceShrooms(200, 200, 150);
        }
        public override void PreUpdateWorld()
        {
        }
        public static void PlaceShrooms(int hellcapChance, int toadstoolChance, int iceliumChance)
        {
            int[] chances = { hellcapChance, toadstoolChance, iceliumChance };
            int[] amount = { 0, 0, 0 };
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    if (Framing.GetTileSafely(x, y - 1).HasTile) continue; //obviously don't spawn if theres already a block occupying it
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (!tile.HasTile || tile.LiquidAmount > 0) continue;

                    int index = 0;
                    foreach (var shroom in shroomTileDict)
                    {
                        if (!Main.rand.NextBool(chances[index])) continue;

                        int shroomTile = shroom.Key;
                        if (shroomTile == shroomTileList[1] &&
                            y < Main.rockLayer) continue; //only let toadstools spawn underground!!

                        int[] validTiles = shroom.Value;
                        if (validTiles.Contains(tile.TileType))
                        {
                            WorldGen.PlaceTile(x, y-1, shroomTile, true, true);
                            NetMessage.SendTileSquare(-1, x, y);
                            amount[index]++;
                        }
                        index++;
                    }
                }
            }
            Main.NewText("Placed " + amount[0] + " hellcaps!");
            Main.NewText("Placed " + amount[1] + " toadstools!");
            Main.NewText("Placed " + amount[2] + " iceliums!");
        }
    }
}
