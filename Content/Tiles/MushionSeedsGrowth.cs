using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
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
        public float colonyGlowModifier = 0;
        public MushionSeedsGrowth()
        {
            growRate = Main.rand.Next(3000, 4200);
        }
        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == ModContent.TileType<MushionSeedsTile>();
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
            if (!MushionColonySystem.IsValidForGrowing(Position.X, Position.Y))
            {
                colonyGlowModifier = MathHelper.Lerp(colonyGlowModifier, 0, 0.25f);
                return;
            }
            colonyGlowModifier = MathHelper.Lerp(colonyGlowModifier, 1, 0.25f);

            growth = Math.Min(growth + 5f / growRate, 3.99f);
        
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
        public static Effect Blur;
        private MushionGlowTarget _target;
        public override void Load()
        {
            On_Main.DrawTiles += On_Main_DrawTiles;
            _target = new MushionGlowTarget();
            Main.ContentThatNeedsRenderTargets.Add(_target);
            Blur = ModContent.Request<Effect>("DTZ/Assets/Effects/GaussianBlur", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
        public override void Unload()
        {
            On_Main.DrawTiles -= On_Main_DrawTiles;
            Main.ContentThatNeedsRenderTargets.Remove(_target);
            _target = null;
        }
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
        public static Point GetTopLeft(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            int left = i - (tile.TileFrameX / 18) % 2;
            int top = j - (tile.TileFrameY / 18) % 2;
            return new Point(left, top);
        }
        #region drawing
        private float GlowAlpha(float modifier)
        {
            int tickCycle = 500;
            float alpha = 0.5f + (float)Math.Sin(Main.timeForVisualEffects / tickCycle * MathHelper.TwoPi) / 2; //make the sine 0-1

            return MathHelper.Lerp(modifier / 2, modifier, alpha);
        }
        private float ColonyGlowModifier = 0;
        private void On_Main_DrawTiles(On_Main.orig_DrawTiles orig, Main self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets, int waterStyleOverride)
        {
            _target.Request();
            if (_target.IsReady)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, Blur);

                Vector2 offset = Main.screenLastPosition - Main.screenPosition;

                Color color = Color.DeepSkyBlue;
                float amount = MathHelper.Lerp(0.25f, 0.75f, growth / 4) / 2;
                amount = GlowAlpha(amount) * glowMod;
                _target.color = color;

                Blur.Parameters["uIntensity"].SetValue(1);
                Blur.Parameters["horizontal"].SetValue(false);
                Blur.Parameters["uScreenResolution"].SetValue(Main.ScreenSize.ToVector2() / GlowAlpha(2));
                Blur.Parameters["uOpacity"].SetValue(amount);
                Blur.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(_target.GetTarget(), offset, color);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
                DrawAllTiles(Main.spriteBatch, false);
            }
            orig(self, solidLayer, forRenderTargets, intoRenderTargets, waterStyleOverride);
        }
        public static void DrawAllTiles(SpriteBatch spriteBatch, bool blur = false, Color? color = null)
        {
            int minX = Main.screenPosition.ToPoint().X / 16 - 1;
            int maxX = minX + Main.screenWidth / 16 + 1;
            int minY = Main.screenPosition.ToPoint().Y / 16 - 1;
            int maxY = minY + Main.screenHeight / 16 + 1;
            int Type = ModContent.TileType<MushionSeedsTile>();

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile.TileType != Type || !tile.HasTile) continue;
                    Texture2D tex = ModContent.Request<Texture2D>("DTZ/Content/Tiles/MushionSeedsGrowth_sheet").Value;

                    int frameX = tile.TileFrameX;
                    short frameY = tile.TileFrameY;

                    int tilePhase = GetPhaseFromCoordinates(x, y);

                    MushionSeedsTile.getFrameYOffset(ref frameY, 34, tilePhase);
                    Rectangle frame = new Rectangle(frameX, frameY, 16, 16);
                    Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
                    Vector2 drawPos = new Vector2(x * 16, y * 16) - Main.screenPosition + zero;
                    drawPos.Y += 4;

                    if (blur)
                    {
                        MushionSeedsTile.Blur.Parameters["uIntensity"].SetValue(1);
                        MushionSeedsTile.Blur.Parameters["horizontal"].SetValue(true);
                        MushionSeedsTile.Blur.Parameters["uScreenResolution"].SetValue(Main.ScreenSize.ToVector2() / 64);
                        MushionSeedsTile.Blur.Parameters["uOpacity"].SetValue(1);
                        MushionSeedsTile.Blur.CurrentTechnique.Passes[0].Apply();
                    }
                    Color drawColor = color.HasValue ? color.Value : Color.White;

                    spriteBatch.Draw(tex, drawPos, frame, drawColor, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
                }
            }
        }
        private static int phase = 0;
        private float growth { get; set; } = 0;
        private float glowMod = 0;
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY += 4;
            var topleft = GetTopLeft(i, j);
            if (TileEntity.TryGet(topleft.X, topleft.Y, out MushionSeedsGrowth entity))
            {
                growth = entity.growth;
                phase = entity.phase;
                getFrameYOffset(ref tileFrameY, 34, phase);
                glowMod = entity.colonyGlowModifier;
            }
        }
        public static void getFrameYOffset(ref short frameY, int frameHeight, int phase)
        {
            frameY += (short)(phase * frameHeight);
        }
        public static int GetPhaseFromCoordinates(int x, int y)
        {
            var topleft = GetTopLeft(x, y);
            if (TileEntity.TryGet(topleft.X, topleft.Y, out MushionSeedsGrowth entity))
            {
                return entity.phase;
            }
            return 0;
        }
        #endregion
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
                if ((TileEntity.ByPosition.TryGetValue(new Point16(point.X, point.Y-2), out TileEntity entity)) && entity is MushionSeedsGrowth)
                {
                    count++;
                }
            }
            if (chunk.Count < 20) return 0;
            return count;
        }
        public static bool IsValidForGrowing(int i, int j) =>
            (ColonyCount(i, j + 2) >= 3 && Lighting.Brightness(i, j) < 0.4f);
    }
    public class MushionGlowTarget : ARenderTargetContentByRequest
    {
        public MushionGlowTarget() { }
        public Color color;
        public float alpha = 0;
        protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth, Main.screenHeight, RenderTargetUsage.PreserveContents);

            device.SetRenderTarget(_target);
            device.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, MushionSeedsTile.Blur);

            int minX = Main.screenPosition.ToPoint().X / 16 - 1;
            int maxX = minX + Main.screenWidth / 16 + 1;
            int minY = Main.screenPosition.ToPoint().Y / 16 - 1;
            int maxY = minY + Main.screenHeight / 16 + 1;
            int Type = ModContent.TileType<MushionSeedsTile>();

            MushionSeedsTile.DrawAllTiles(spriteBatch, true, color);

            spriteBatch.End();
            device.SetRenderTarget(null);
            _wasPrepared = true;
        }
    }
}