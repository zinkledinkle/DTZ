using DTZ.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent;
using Terraria.DataStructures;
using System;
using Terraria.ID;
using ReLogic.Graphics;

namespace DTZ.UI
{
    public class GrowthBarUI : UIState
    {
        private float Size { get; set; } = 0;
        private Vector2 Pos { get; set; } = Vector2.Zero;
        private float Progress { get; set; } = 0;
        private TileEntity lastTileEntity;
        public override void Update(GameTime gameTime)
        {
            Vector2 mouseScreen = Main.MouseScreen * Main.UIScale;
            Vector2 mouseWorldTransformed = Vector2.Transform(mouseScreen, Matrix.Invert(Main.GameViewMatrix.ZoomMatrix));
            mouseWorldTransformed += Main.screenPosition;

            Point tileCoords = mouseWorldTransformed.ToTileCoordinates();
            Tile tile = Framing.GetTileSafely(mouseWorldTransformed.ToTileCoordinates());
            if (tile.HasTile && tile.TileType == ModContent.TileType<MushionSeedsTile>())
            {
                Point topleft = MushionSeedsTile.GetTopLeft(tileCoords.X, tileCoords.Y);
                if ((TileEntity.TryGet(new Point16(topleft), out TileEntity entity)) && entity is MushionSeedsGrowth mushionEntity)
                {
                    if (lastTileEntity != mushionEntity) Size = 0;
                    lastTileEntity = mushionEntity;

                    Pos = topleft.ToWorldCoordinates();
                    Pos += new Vector2(8, 8);
                    Size = MathHelper.Lerp(Size, 1, 0.25f);
                    Progress = mushionEntity.growth / 4;
                }
            } else
            {
                Size = MathHelper.Lerp(Size, 0, 0.25f);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Size < 0.01f) return;
            Texture2D tex = ModContent.Request<Texture2D>("DTZ/UI/GrowthBar").Value;
            Vector2 worldDrawPos = Pos + new Vector2(0, -50 * Size);
            Vector2 drawPos = (worldDrawPos - Main.screenPosition);
            drawPos /= Main.UIScale;

            Vector2 offsetFromCenter = drawPos - Main.ScreenSize.ToVector2() / 2;
            offsetFromCenter *= Main.GameViewMatrix.Zoom;
            drawPos = Main.ScreenSize.ToVector2() / 2 + offsetFromCenter;

            float scale = Size;
            if (scale >= 0.98f) scale = 1;

            spriteBatch.Draw(tex, drawPos, null, Color.White, 0, tex.Size() / 2, scale, SpriteEffects.None, 1f); //the amount of point to vector2 conversion is SICKENING

            int scaledWidth = (int)(24 * Progress * scale);
            int scaledHeight = (int)(12 * scale);
            int scaledX = (int)(14 * scale) + (int)(tex.Width * (0.5f - scale/2));
            int scaledY = (int)(14 * scale) + (int)(tex.Height * (0.5f - scale / 2));

            Rectangle rect = new(
                (int)drawPos.X - tex.Width / 2 + scaledX,
                (int)drawPos.Y - tex.Height / 2 + scaledY,
                scaledWidth,
                scaledHeight);

            Rectangle fade = rect;
            fade.Inflate(1, 1);

            Color color = Color.Lerp(Color.DeepSkyBlue, Color.LightSkyBlue, Progress) * Size;

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, fade, color * 0.5f);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, color);

            MushionSeedsGrowth m = lastTileEntity as MushionSeedsGrowth;
            string text = String.Concat("ID: ", lastTileEntity.ID, ", Colony ID: ", m.colonyID);
            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, drawPos, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
        }
    }
    public class GrowthBarUISystem : ModSystem
    {
        internal GrowthBarUI barUI;
        private UserInterface barInterface;
        public override void Load()
        {
            barUI = new();
            barUI.Activate();
            barInterface = new();
            barInterface.SetState(barUI);
        }
        public override void UpdateUI(GameTime gameTime) => barInterface.Update(gameTime);
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Add(new LegacyGameInterfaceLayer("Mycology/GrowthBarUI", 
                delegate
                {
                    barInterface.Draw(Main.spriteBatch, new GameTime());
                    return true;
                },
                InterfaceScaleType.UI));
        }
    }
}
