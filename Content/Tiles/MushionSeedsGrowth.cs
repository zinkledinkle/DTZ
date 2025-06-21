using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace DTZ.Content.Tiles
{
    public class MushionSeedsGrowth : ModTileEntity
    {
        public int phase = 0;
        public float growth = 0;
        public int growRate = 3600;
        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == ModContent.TileType<MushionSeedsTile>() && HasMud(x, y);
        }
        public static bool HasMud(int i, int j)
        {
            int tilledMud = ModContent.TileType<TilledMud>();
            Tile[] tiles =
{
                Framing.GetTileSafely(i, j + 1),
                Framing.GetTileSafely(i + 1, j + 1),
                Framing.GetTileSafely(i, j + 2),
                Framing.GetTileSafely(i + 1, j + 2)
            };
            int mudTiles = tiles.Where(tile => tile.HasTile && (tile.TileType == tilledMud)).Count();
            return mudTiles == 4;
        }
        public static bool HasHeadroom(int i, int j) //call from the bottom left corner
        {
            Tile[] tiles =
            {
                Framing.GetTileSafely(i, j - 1),
                Framing.GetTileSafely(i + 1, j - 1),
                Framing.GetTileSafely(i, j - 2),
                Framing.GetTileSafely(i + 1, j - 2)
            };
            int airTiles = tiles.Where(tile => !tile.HasTile).Count();
            return airTiles == 4;
        }
        public override void Update()
        {
            if (!MushionColonySystem.IsValidForGrowing(Position.X, Position.Y)) return; //make sure to shift downward since the tilentity is always on the top left and we need to get the mud tiles

            growth = Math.Min(growth + 5f / growRate, 3);
        
            phase = (int)Math.Floor(growth);

            NetMessage.SendData(MessageID.TileEntitySharing, number: ID); //been told this is important for tileEntities 
        }
        public override void SaveData(TagCompound tag) => tag[nameof(growth)] = growth;
        public override void LoadData(TagCompound tag) => growth = tag.GetFloat(nameof(growth));
        public override void NetSend(BinaryWriter writer) => writer.Write(growth);
        public override void NetReceive(BinaryReader reader) => growth = reader.ReadSingle();
    }
    public class MushionSeedsTile : ModTile
    {
        public override string Texture => "DTZ/Content/Tiles/MushionSeedsGrowth_sheet";
        public override bool CanPlace(int i, int j) => MushionSeedsGrowth.HasMud(i, j);
        private int frame { get; set; } = 0;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileHammeringIfOnTopOfIt[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);

            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<MushionSeedsGrowth>().Generic_HookPostPlaceMyPlayer;

            TileObjectData.addTile(Type);
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<MushionSeedsGrowth>().Kill(i, j);
        }
        private float GlowAlpha(float modifier)
        {
            int tickCycle = 500;
            float alpha = 0.5f + (float)Math.Sin(Main.timeForVisualEffects / tickCycle * MathHelper.TwoPi) / 2; //make the sine 0-1

            return MathHelper.Lerp(modifier / 2, modifier, alpha);
        }
        private float growth { get; set; } = 0;
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //will add glow later
            return base.PreDraw(i, j, spriteBatch);
        }
        private int yOffset = 0;
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY += 4;
            yOffset = offsetY;
            var topleft = GetTopLeft(i, j);
            if (TileEntity.TryGet(topleft.X, topleft.Y, out MushionSeedsGrowth entity))
            {
                growth = entity.growth;
                getFrameYOffset(ref tileFrameY, growth, 34);
            }
        }
        private void getFrameYOffset(ref short frameY, float growth, int frameHeight)
        {
            int phase = (int)Math.Floor(growth);
            frameY += (short)(phase * frameHeight);
        }
        private static Point GetTopLeft(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            int left = i - (tile.TileFrameX / 18) % 2;
            int top = j - (tile.TileFrameY / 18) % 2;
            return new Point(left, top);
        }
    }
    public class MushionColonySystem : ModSystem
    {
        private static readonly Point[] cardinals = new Point[]
        {
            new(0, -1), 
            new(1, 0),
            new(0, 1),
            new(-1, 0)
        };
        private static List<Point> ConnectedTiles(int i, int j)
        {
            Queue<Point> pointQueue = new();
            HashSet<Point> visited = new();
            List<Point> connected = new();

            int maxRange = 100;

            pointQueue.Enqueue(new Point(i, j));
            visited.Add(new Point(i, j));
            while (pointQueue.Count > 0 && connected.Count < maxRange)
            {
                Point p = pointQueue.Dequeue();
                connected.Add(p);
                foreach (var shift in cardinals)
                {
                    Point neighbor = p + shift;
                    if (!visited.Contains(neighbor))
                    {
                        Tile tile = Framing.GetTileSafely(neighbor.X, neighbor.Y);
                        if (tile.HasTile && tile.TileType == ModContent.TileType<TilledMud>())
                        {
                            visited.Add(neighbor);
                            pointQueue.Enqueue(neighbor);
                        }
                    }
                }
            }
            return connected;
        }
        private static int ColonyCount(int i, int j)
        {
            List<Point> chunk = ConnectedTiles(i, j);
            int count = 0;
            foreach (var point in chunk)
            {
                if (TileEntity.ByPosition.TryGetValue(new Point16(point.X, point.Y-2), out TileEntity entity) && entity is MushionSeedsGrowth) //possibly inneficient idk
                {
                    count++;
                }
            }
            return count;
        }
        public static bool IsValidForGrowing(int i, int j) =>
            (ColonyCount(i, j + 2) >= 3 && Lighting.Brightness(i, j) < 0.4f);
    }
}