using Mycology.Content.NPCs;
using Mycology.Content.Tiles;
using Mycology.Content.Tiles.Ambient;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Personalities;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace Mycology.Systems
{
    public class Haven : ModBiome
    {
        public override Color? BackgroundColor => Color.DarkBlue;
        public override LocalizedText DisplayName => Language.GetText("HavenBiomeName");
        public override string MapBackground => "Terraria/Images/MapBG21";
        public override int BiomeTorchItemType => ItemID.MushroomTorch;
        public override int BiomeCampfireItemType => ItemID.MushroomCampfire;
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<HavenSystem>().MushionGrassCount > 400;
        }
        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive) Main.undergroundBackground = 8;
        }
        public override int Music => MusicLoader.GetMusicSlot("Mycology/Music/Haven");
    }
    public class HavenSystem : ModSystem
    {
        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("SpawnPositionForLoneMushion", SpawnPositionForLoneMushion);
        }
        public override void LoadWorldData(TagCompound tag)
        {
            SpawnPositionForLoneMushion = tag.Get<Vector2>("SpawnPositionForLoneMushion");
        }
        public override void OnWorldLoad()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<LoneMushion>()))
            {
                NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)SpawnPositionForLoneMushion.X, (int)SpawnPositionForLoneMushion.Y, ModContent.NPCType<LoneMushion>());
            }
        }

        public int MushionGrassCount;
        public static Vector2 SpawnPositionForLoneMushion;
        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            MushionGrassCount = tileCounts[ModContent.TileType<MushionGrass>()];
        }
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int mushroomIndex = tasks.FindIndex(pass => pass.Name == "Marble");
            if (mushroomIndex != -1)
            {
                tasks.Insert(mushroomIndex+1, new HavenGenPass("Mushion Haven pass", 100f));
            }
        }
        private static readonly List<(int, int, int, int)> ChestLoot = new()
        {
             (ItemID.GlowingMushroom, 50, 80, 1),
             (ItemID.GoldCoin, 4, 7, 1),
             (ItemID.Dynamite, 2, 7, 2),
             (ItemID.Bomb, 3, 10, 2),
             (ItemID.ShroomMinecart, 1, 1, 10),
             (ItemID.Shroomerang, 1, 1, 5),
             (ItemID.SuspiciousLookingEye, 1, 1, 5),
             (ItemID.BandofRegeneration, 1, 1, 3),
        };
        public class HavenGenPass(string name, double loadWeight) : GenPass(name, loadWeight)
        {
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration) => Generate();
            private static Dictionary<Vector2, int> minibiomes = new();
            public static void Generate()
            {
                minibiomes = new();
                Vector2 center = new(Main.maxTilesX / 2, Main.maxTilesY/6);
                List<Vector2> biomes = FindMushroomBiomes();
                Vector2 closest = biomes.OrderBy(biome =>
                {
                    float dx = biome.X - center.X;
                    float dy = biome.Y - center.Y;
                    return 2 * Math.Abs(dx) + Math.Abs(dy);
                })
                .FirstOrDefault();

                foreach (Vector2 biome in biomes)
                {
                    if (WorldGen.genRand.NextBool(2, 5) && biome != closest) continue;

                    int width = WorldGen.genRand.Next(80, 120);
                    int height = WorldGen.genRand.Next(30, 60);

                    Vector2 start = biome + WorldGen.genRand.NextVector2CircularEdge(50, 50);
                    minibiomes.Add(start, height);
                    Clear((int)start.X, (int)start.Y, width, height);
                    MakeOval((int)start.X, (int)start.Y, width, height);
                    Wallify((int)start.X, (int)start.Y, width, height);
                    Smooth((int)start.X, (int)start.Y, width, height); 
                    Reframe((int)start.X, (int)start.Y, width, height);
                    Vegetate((int)start.X, (int)start.Y, width, height);

                    if (biome == closest)
                    {
                        Structure((int)start.X, (int)start.Y);
                    }
                }
                grassify(); //do this seperately, not per biome, since it checks all biomes itself
            }
            public static void Clear(int cX, int cY, int rX, int rY)
            {
                for (int x = cX - rX; x <= cX + rX; x++)
                {
                    for (int y = cY - rY; y <= cY + rY; y++)
                    {
                        if (x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY)
                        {
                            double dx = (x - cX) / (double)rX;
                            double dy = (y - cY) / (double)rY;
                            if (dx * dx + dy * dy <= 1.0)
                            {
                                if (TileID.Sets.DungeonBiome[Framing.GetTileSafely(x, y).TileType] == 1)
                                {
                                    continue;
                                }
                                Main.tile[x, y].ClearEverything();
                            }
                        }
                    }
                }
            }
            private static void MakeOval(int cX, int cY, int rx, int ry)
            {
                int points = 1000;
                for (int i = 0; i < points; i++)
                {
                    double theta = (2 * Math.PI * i) / points;
                    float px = (float)(rx * Math.Cos(theta));
                    float py = (float)(ry * Math.Sin(theta));
                    px += WorldGen.genRand.Next(-3, 4);
                    py += WorldGen.genRand.Next(-3, 4);

                    int x = cX + (int)px;
                    int y = cY + (int)py;

                    Point16 point = new Point16((int)x, (int)y);

                    WorldGen.TileRunner(point.X, point.Y, 12 + WorldGen.genRand.Next(0, 5), 1, ModContent.TileType<PackedMud>(), true, 0, 0);
                }
            }
            private static void grassify()
            {
                int mudType = ModContent.TileType<PackedMud>();
                int grassType = ModContent.TileType<MushionGrass>();

                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        Tile tile = Framing.GetTileSafely(i, j);
                        if (tile.TileType != mudType) continue;
                        if (isExposed(i, j) || WorldGen.genRand.NextBool(30))
                        {
                            tile.TileType = (ushort)grassType;
                        }
                    }
                }
            }
            private static void Vegetate(int cX, int cY, int rX, int rY)
            {
                int[] rubbles = new[]
                {
                    ModContent.TileType<HavenGrass>(),
                    ModContent.TileType<HavenShroomSmall>(),
                };
                for (int x = cX - rX - 10; x < cX + rX + 10; x++)
                {
                    for (int y = cY - rY - 10; y < cY + rY + 10; y++)
                    {
                        if (!IsPointInOval(x, y, cX, cY, rX, rY) || !WorldGen.genRand.NextBool(5)) continue;
                        Tile tile = Framing.GetTileSafely(x, y);
                        Tile above = Framing.GetTileSafely(x, y - 1);

                        if (above.HasTile) continue;

                        int rubble = rubbles[WorldGen.genRand.Next(rubbles.Length)];
                        WorldGen.PlaceTile(x, y - 1, rubble, true, true);

                        if (WorldGen.genRand.NextBool(5)) 
                        {
                            WorldGen.PlaceTile(x+1, y-1, ModContent.TileType<HavenShroomBig>(), true, true);
                        }
                    }
                }
            }
            private static readonly Point[] cardinals =
            [
                        new(0, -1),
                        new(1, 0),
                        new(0, 1),
                        new(-1, 0),
                        new(1, -1),
                        new(1, 1),
                        new(-1, 1),
                        new(-1, -1)
            ];
            private static bool isExposed(int x, int y) => (NeighborCount(x, y) != 8);
            private static int NeighborCount(int x, int y)
            {
                int count = 0;
                foreach (Point point in cardinals)
                {
                    if (Framing.GetTileSafely(x + point.X, y + point.Y).HasTile) count++;
                }
                return count;
            }
            private static void Smooth(int cX, int cY, int rX, int rY)
            {
                int startPositionX = cX - rX;
                int endPositionX = cX + rX;
                int startPositionY = cY - rY;
                int endPositionY = cY + rY;
                for (int x = startPositionX; x < endPositionX; x++)
                {
                    for (int y = startPositionY; y < endPositionY; y++)
                    {
                        if (NeighborCount(x, y) < 4)
                        {
                            Framing.GetTileSafely(x, y).Clear(TileDataType.Tile);
                        }
                    }
                }
            }
            private static List<Vector2> Islands = new();
            private static void Wallify(int cX, int cY, int rX, int rY)
            {
                Islands = new();
                Noise noise = new(WorldGen._genRandSeed, 16);

                for (int x = cX - rX; x < cX + rX; x++)
                {
                    for (int y = cY - rY; y < cY + rY; y++)
                    {
                        if (!IsPointInOval(x, y, cX, cY, rX, rY)) continue;

                        float value = noise.Value(x, y);
                        if (value > 0.1f) {
                            continue;
                        };
                        WorldGen.PlaceWall(x, y, WallID.MushroomUnsafe, true);
                        if (value <= 0.01f)
                        {
                            if (Islands.Any(i => i.Distance(new Vector2(x, y)) < 25)) continue;
                            Vector2 offset = new Vector2(cX, cY) - new Vector2(x, y);
                            offset.X /= 6;
                            float distance = offset.Length();
                            if (distance > 25) continue;
                            Islands.Add(new Vector2(x, y));
                        }
                    }
                }
                foreach (var island in Islands)
                {
                    Point pos = island.ToPoint();
                    double strength = WorldGen.genRand.NextFloat(5, 10);
                    WorldGen.TileRunner(pos.X, pos.Y, strength, WorldGen.genRand.Next(50,100), ModContent.TileType<PackedMud>(), true, WorldGen.genRand.NextFloat(-10, 10), 0, true);
                }
            }
            private static void Structure(int x, int cY)
            {
                for (int y = cY + 25; y < cY + 100; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    Tile tile2 = Framing.GetTileSafely(x, y+1);
                    if (tile.HasTile && tile2.HasTile) {
                        StructureSystem.PlaceStructure("ShroomThing", new Point(x - 14, y - 6), ChestLoot);
                        SpawnPositionForLoneMushion = new Vector2(x - 4, y - 3);
                        break;
                    }
                }
            }
            private static void Reframe(int cX, int cY, int rX, int rY)
            {
                for (int x = cX - rX - 10; x < cX + rX + 10; x++)
                {
                    for (int y = cY - rY - 10; y < cY + rY + 10; y++)
                    {
                        if (IsPointInOval(x, y, cX, cY, rX, rY))
                            WorldGen.Reframe(x, y); //might be unnecessary 
                    }
                }
            }
            private static bool IsPointInOval(int x, int y, int cX, int cY, int rx, int ry)
            {
                double dx = (x - cX) / (double)rx;
                double dy = (y - cY) / (double)ry;
                return dx * dx + dy * dy <= 1.0;
            }
            private static List<Vector2> visited;
            private static List<Vector2> FindMushroomBiomes()
            {
                visited = new();
                List<Vector2> list = new List<Vector2>();
                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    for (int y = 0; y < Main.maxTilesY; y++)
                    {
                        bool tooClose = false;
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.HasTile || tile.TileType != TileID.MushroomGrass) continue;

                        Vector2 pos = new(x, y);

                        if (visited != null) foreach (Vector2 position in visited)
                        {
                            float distance = Vector2.Distance(position, pos);
                            if (distance < 600)
                            {
                                tooClose = true; break;
                            }
                        }
                        if (tooClose) continue;
                        list.Add(pos); 
                        visited.Add(pos);
                    }
                }
                return list;
            }
            private class Noise
            {
                private readonly int seed;
                private readonly int cellSize;
                private readonly Random rand;
                public Noise(int seed, int cellSize = 32)
                {
                    this.seed = seed;
                    this.cellSize = cellSize;
                    this.rand  = new Random(seed);
                }
                private Vector2 GetCell(int x, int y)
                {
                    int hash = x * 73856093 ^ y * 19349663 ^ seed;
                    Random cellRand = new Random(hash);
                    float px = x * cellSize + (float)cellRand.NextDouble() * cellSize;
                    float py = y * cellSize + (float)cellRand.NextDouble() * cellSize;
                    return new Vector2(px, py);
                }
                public float Value(float x, float y)
                {
                    int cellX = (int)Math.Floor(x / cellSize);
                    int cellY = (int)Math.Floor(y / cellSize);

                    float dist1 = float.MaxValue;
                    float dist2 = float.MaxValue;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Vector2 point = GetCell(cellX + i, cellY + j);
                            float dist = Vector2.Distance(new Vector2(x, y), point);
                            if (dist < dist1)
                            {
                                dist2 = dist1;
                                dist1 = dist;
                            } else if (dist < dist2)
                            {
                                dist2 = dist;
                            }
                        }
                    }
                    float value = dist2 - dist1;

                    value = Math.Clamp(value / cellSize, 0, 1);
                    value = value * value * (3 - 2 * value);
                    return value;
                }
            }
        }
    }
    public class HavenGlobalNPC : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            int x = spawnInfo.SpawnTileX;
            int y = spawnInfo.SpawnTileY;
            if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<MushionGrass>())
            {
                pool.Add(NPCID.SporeBat, 5);
                pool.Add(NPCID.ZombieMushroom, 5);
                pool.Add(NPCID.ZombieMushroomHat, 5);
            }
        }
    }
}
