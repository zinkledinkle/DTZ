using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
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
        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == ModContent.TileType<MushionSeedsTile>() && HasMud(x, y);
        }
        public static bool HasMud(int i, int j)
        {
            int tilledMud = ModContent.TileType<TilledMud>();
            return Framing.GetTileSafely(i, j + 1).TileType == tilledMud && Framing.GetTileSafely(i + 1, j + 1).TileType == tilledMud;
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
            growth = Math.Min(growth + 5f / 3600, 3); //half a phase an hour
            Main.NewText(ID + ", " + growth);
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

            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<MushionSeedsGrowth>().Generic_HookPostPlaceMyPlayer;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight; //opposite direction
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<MushionSeedsGrowth>().Kill(i, j);
        }
        private float GlowAlpha(float modifier)
        {
            int tickCycle = 240;
            float alpha = 0.5f + (float)Math.Sin(Main.timeForVisualEffects / tickCycle * MathHelper.TwoPi) / 2; //make the sine 0-1

            return MathHelper.Lerp(modifier / 2, modifier, alpha);
        }
        private float growth { get; set; } = 0;
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            if (tile.TileFrameY % 34 != 0)
                return base.PreDraw(i, j, spriteBatch);

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_glow").Value;
            Vector2 drawPosition = new Vector2(i * 16, j * 16) - Main.screenPosition + zero;
            drawPosition.Y += yOffset;
            short frameY = tile.TileFrameY;
            getFrameYOffset(ref frameY, growth);
            Rectangle source = new(tile.TileFrameX, frameY, 16, 34);

            Color color = Color.CadetBlue;
            color *= GlowAlpha(0.25f + (growth / 6));
            SpriteBatch old = spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(texture, drawPosition, source, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

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
                getFrameYOffset(ref tileFrameY, growth);
                growth = entity.growth;
            }
        }
        private void getFrameYOffset(ref short frameY, float growth)
        {
            int phase = (int)Math.Floor(growth);
            frameY += (short)(phase * 34);
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
        private bool CheckColony(int i, int j)
        {
            return false;
        }
    }
}