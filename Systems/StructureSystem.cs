using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Mycology.Systems
{
    public class TileData
    {
        #region fields
        public ushort TileType { get; set; }
        public ushort WallType { get; set; }
        public bool HasTile { get; set; }
        public bool IsActuated { get; set; }
        public bool HasActuator { get; set; }
        public bool HasUnactuatedTile { get; set; }
        public int Slope { get; set; }
        public int BlockType { get; set; }
        public bool IsHalfBlock { get; set; }
        public bool TopSlope => Slope == (int)SlopeType.SlopeDownLeft || Slope == (int)SlopeType.SlopeDownRight;
        public bool BottomSlope => Slope == (int)SlopeType.SlopeUpLeft || Slope == (int)SlopeType.SlopeUpRight;
        public bool LeftSlope => Slope == (int)SlopeType.SlopeDownRight || Slope == (int)SlopeType.SlopeUpRight;
        public bool RightSlope => Slope == (int)SlopeType.SlopeDownLeft || Slope == (int)SlopeType.SlopeUpLeft;
        public short TileFrameX { get; set; }
        public short TileFrameY { get; set; }
        public int WallFrameX { get; set; }
        public int WallFrameY { get; set; }
        public int TileFrameNumber { get; set; }
        public int WallFrameNumber { get; set; }
        public byte TileColor { get; set; }
        public byte WallColor { get; set; }
        public byte LiquidAmount { get; set; }
        public int LiquidType { get; set; }
        public bool SkipLiquid { get; set; }
        public bool CheckingLiquid { get; set; }
        public bool RedWire { get; set; }
        public bool GreenWire { get; set; }
        public bool BlueWire { get; set; }
        public bool YellowWire { get; set; }
        public bool IsTileInvisible { get; set; }
        public bool IsWallInvisible { get; set; }
        public bool IsTileFullbright { get; set; }
        public bool IsWallFullbright { get; set; }
        #endregion
        public static Tile TileFromData(TileData data)
        {
            Tile tile = new()
            {
                TileType = data.TileType,
                WallType = data.WallType,
                HasTile = data.HasTile,
                IsActuated = data.IsActuated,
                HasActuator = data.HasActuator,
                Slope = (SlopeType)data.Slope,
                BlockType = (BlockType)data.BlockType,
                IsHalfBlock = data.IsHalfBlock,
                TileFrameX = data.TileFrameX,
                TileFrameY = data.TileFrameY,
                WallFrameX = data.WallFrameX,
                WallFrameY = data.WallFrameY,
                TileFrameNumber = data.TileFrameNumber,
                WallFrameNumber = data.WallFrameNumber,
                TileColor = data.TileColor,
                WallColor = data.WallColor,
                LiquidAmount = data.LiquidAmount,
                LiquidType = data.LiquidType,
                SkipLiquid = data.SkipLiquid,
                CheckingLiquid = data.CheckingLiquid,
                RedWire = data.RedWire,
                GreenWire = data.GreenWire,
                BlueWire = data.BlueWire,
                YellowWire = data.YellowWire,
                IsTileInvisible = data.IsTileInvisible,
                IsWallInvisible = data.IsWallInvisible,
                IsTileFullbright = data.IsTileFullbright,
                IsWallFullbright = data.IsWallFullbright
            };
            return tile;
        }
        public static TileData DataFromTile(Tile tile)
        {
            TileData tileData = new()
            {
                TileType = tile.TileType,
                WallType = tile.WallType,
                HasTile = tile.HasTile,
                IsActuated = tile.IsActuated,
                HasActuator = tile.HasActuator,
                Slope = (int)tile.Slope,
                BlockType = (int)tile.BlockType,
                IsHalfBlock = tile.IsHalfBlock,
                TileFrameX = tile.TileFrameX,
                TileFrameY = tile.TileFrameY,
                WallFrameX = tile.WallFrameX,
                WallFrameY = tile.WallFrameY,
                TileFrameNumber = tile.TileFrameNumber,
                WallFrameNumber = tile.WallFrameNumber,
                TileColor = tile.TileColor,
                WallColor = tile.WallColor,
                LiquidAmount = tile.LiquidAmount,
                LiquidType = tile.LiquidType,
                SkipLiquid = tile.SkipLiquid,
                CheckingLiquid = tile.CheckingLiquid,
                RedWire = tile.RedWire,
                GreenWire = tile.GreenWire,
                BlueWire = tile.BlueWire,
                YellowWire = tile.YellowWire,
                IsTileInvisible = tile.IsTileInvisible,
                IsWallInvisible = tile.IsWallInvisible,
                IsTileFullbright = tile.IsTileFullbright,
                IsWallFullbright = tile.IsWallFullbright
            };
            return tileData;
        }
    }
    public class TileEntityData
    {
        #region fields
        public int ID;
        public int PositionX;
        public int PositionY;
        public byte type; //thats it I guess
        #endregion
        public static TileEntityData DataFromEntity(TileEntity entity, Point16 structurePos)
        {
            TileEntityData data = new()
            {
                ID = entity.ID,
                PositionX = entity.Position.X - structurePos.X,
                PositionY = entity.Position.Y - structurePos.Y,
                type = (byte)entity.type
            };
            return data;
        }
        public static void SpawnTileEntityFromData(TileEntityData entityData, Point16 structurePos)
        {
            if (entityData == null) return;
            if (TileEntity.ByID.TryGetValue(entityData.ID, out TileEntity entity))
            {
                entity = TileEntity.manager.GenerateInstance(entityData.type);
                entity.ID = entityData.ID;
                entity.Position = structurePos + new Point16(entityData.PositionX, entityData.PositionY);
                TileEntity.manager.Register(entity);
            }
        }
    }
    public class ChestData
    {
        public ItemData[] Item { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool BankChest { get; set; }
        public string Name { get; set; }
        public int Frame { get; set; }
        public int Style { get; set; }
        public static ChestData DataFromChest(Chest chest, Tile chestTile, Point structurePos)
        {
            ChestData data = new()
            {
                BankChest = chest.bankChest,
                Frame = chest.frame,
                X = chest.x - structurePos.X,
                Y = chest.y - structurePos.Y,
                Name = chest.name,
                Item = chest.item.Select(ItemData.DataFromItem).ToArray(),
                Style = chestTile.TileFrameX/36
            };
            return data;
        }
        public static Chest ChestFromData(ChestData data, int x, int y, List<(int,int,int,int)>? chestLoot = null)
        {
            if (data == null) return null;

            int index = WorldGen.PlaceChest(x + data.X, y + data.Y+1, 21, false, (ushort)data.Style);
            Chest chest = Main.chest[index];
                
            if (chestLoot != null)
            {
                int slot = 0;
                foreach (var item in chestLoot)
                {
                    if (Main.rand.NextBool(item.Item4))
                    {
                        chest.item[slot] = new Item(item.Item1, Main.rand.Next(item.Item2, item.Item3), -1);
                        slot++;
                    }
                }
            } else
            {
                for (int i = 0; i < data.Item.Length; i++)
                {
                    chest.item[i] = data.Item[i]?.ItemFromData();
                }
            }

            chest.name = data.Name;
            return chest;
        }
        public class ItemData
        {
            public int Type { get; set; }
            public int Prefix { get; set; }
            public int Stack { get; set; }
            public static ItemData DataFromItem(Item item)
            {
                return new ItemData
                {
                    Type = item.type,
                    Prefix = item.prefix,
                    Stack = item.stack
                };
            }
            public Item ItemFromData() => new(this.Type, this.Stack, this.Prefix);
        }
    }
    public class StructureData
    {
        public TileData[][] Tiles { get; set; }
        public TileEntityData[] TileEntities { get; set; }
        public ChestData[] Chests { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
    public static class StructureSystem
    {
        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            IncludeFields = true
        };
        private static void SaveStructure(StructureData structure, string filePath)
        {
            string json = JsonSerializer.Serialize(structure, options);
            File.WriteAllText(filePath, json);
        }

        public static void CreateStructure(Rectangle rect, string name)
        {
            StructureData structure = new()
            {
                Width = rect.Width,
                Height = rect.Height,
                Tiles = new TileData[rect.Width][],
                TileEntities = [],
                Chests = []
            };

            for (int x = 0; x < rect.Width; x++)
            {
                structure.Tiles[x] = new TileData[rect.Height];
                for (int y = 0; y < rect.Height; y++)
                {
                    Point worldPos = new(rect.X + x, rect.Y + y);
                    Tile tile = Framing.GetTileSafely(worldPos);
                    TileData tileData = TileData.DataFromTile(tile);
                    structure.Tiles[x][y] = tileData;

                    if (TileEntity.TryGet(rect.X + x, rect.Y + y, out TileEntity entity) && !structure.TileEntities.Any(e => e.ID == entity.ID))
                    {
                        structure.TileEntities = [.. structure.TileEntities, TileEntityData.DataFromEntity(entity, new Point16(rect.X, rect.Y))];
                    }
                    if (Chest.FindChest(worldPos.X, worldPos.Y) is int index && index > -1)
                    {
                        Chest chest = Main.chest[index];
                        ChestData chestData = ChestData.DataFromChest(chest, Framing.GetTileSafely(worldPos), new Point(rect.X, rect.Y));
                        if (structure.Chests.Contains(chestData)) continue;
                        structure.Chests = [.. structure.Chests, chestData];
                    }
                }
            }

            SaveStructure(structure, Path.Combine(Main.SavePath, name + ".json"));
            Main.NewText("Structure saved.");
        }

        private static StructureData LoadStructure(string name)
        {
            byte[] fileBytes = ModContent.GetFileBytes("Mycology/" + name + ".json");
            if (fileBytes == null) return null;
            string json = Encoding.UTF8.GetString(fileBytes);
            return JsonSerializer.Deserialize<StructureData>(json);
        }

        public static void PlaceStructure(string name, Point pos, List<(int,int,int,int)>? chestLoot = null)
        {
            StructureData structure = LoadStructure(name);
            if (structure == null) return;

            for (int x = 0; x < structure.Width; x++)
            {
                for (int y = 0; y < structure.Height; y++)
                {
                    TileData sourceTileData = structure.Tiles[x][y];
                    Tile sourceTile = TileData.TileFromData(sourceTileData);
                    if ((!sourceTile.HasTile && sourceTile.WallType == 0) && sourceTile.LiquidAmount == 0) continue;
                    Tile targetTile = Framing.GetTileSafely(pos.X + x, pos.Y + y);
                    if (sourceTile.TileType is TileID.ShimmerBlock or TileID.Containers) //aetherium can be used to have air blocks in the build
                    {
                        targetTile.Clear(TileDataType.Tile);
                        targetTile.WallType = sourceTile.WallType;
                        WorldGen.Reframe(pos.X + x, pos.Y + y);
                        continue;
                    }
                    if (Main.tileFrameImportant[sourceTile.TileType] && sourceTile.TileFrameX == 0 && sourceTile.TileFrameY == 0)
                    {
                        WorldGen.PlaceTile(x, y, sourceTile.TileType, true, true);
                    } else
                    {
                        targetTile.CopyFrom(sourceTile);
                        WorldGen.Reframe(pos.X + x, pos.Y + y);
                    }
                }
            }
            for (int x = 0; x < structure.Width; x++)
            {
                for (int y = 0; y < structure.Height; y++)
                {
                    TileData sourceTileData = structure.Tiles[x][y];
                    Tile sourceTile = TileData.TileFromData(sourceTileData);
                    if (Main.tileFrameImportant[sourceTile.TileType])// && sourceTile.TileFrameX == 0 && sourceTile.TileFrameY == 0)
                    {
                        //WorldGen.PlaceTile(x, y, sourceTile.TileType, true, true);
                        Framing.GetTileSafely(x, y).WallType = sourceTile.WallType; //anvil is being fucking stupid
                    }
                }
            }
            if (structure.TileEntities != null && structure.TileEntities.Length > 0)
            {
                foreach (var entity in structure.TileEntities) TileEntityData.SpawnTileEntityFromData(entity, new Point16(pos.X, pos.Y));
            }
            if (structure.Chests != null && structure.Chests.Length > 0)
            {
                foreach (var chest in structure.Chests) ChestData.ChestFromData(chest, pos.X, pos.Y, chestLoot);
            }
        }
    }
}
