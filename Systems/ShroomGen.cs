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
            PlaceShrooms(30, 75, 50); //feel free to adjust these rates later btw
        }
        public override void PreUpdateWorld()
        {
            if ((int)Main.time % 3600 == 0)
            {
                PlaceShrooms(90, 200, 150);
            }
        }
        public static void PlaceShrooms(int hellcapChance, int toadstoolChance, int iceliumChance)
        {
            int[] chances = { hellcapChance, toadstoolChance, iceliumChance };
            //int[] amount = { 0, 0, 0 };
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (!tile.HasTile || tile.LiquidAmount > 0) continue;
                    if (Framing.GetTileSafely(x, y - 1).HasTile) continue; //obviously don't spawn if theres already a block occupying it

                    int index = -1;
                    foreach (var shroom in shroomTileDict)
                    {
                        index++;
                        if (!Main.rand.NextBool(chances[index])) continue;

                        int shroomTile = shroom.Key;
                        if (shroomTile == shroomTileList[1] &&
                            y < Main.rockLayer) continue; //only let toadstools spawn underground!!

                        int[] validTiles = shroom.Value;
                        if (validTiles.Contains(tile.TileType))
                        {
                            WorldGen.PlaceTile(x, y-1, shroomTile, true, true);
                            NetMessage.SendTileSquare(-1, x, y);
                            //amount[index]++;
                        }

                    }
                }
            }
            /*Main.NewText("Placed " + amount[0] + " hellcaps!");
            Main.NewText("Placed " + amount[1] + " toadstools!");
            Main.NewText("Placed " + amount[2] + " iceliums!");*/ //was using for debugging earlier, uncomment if you wanna test spawns
        }
    }
}
