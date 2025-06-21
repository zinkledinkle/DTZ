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

namespace DTZ.UI
{
    public class GrowthBarUI : UIState
    {
        private float size { get; set; } = 0;
        private Vector2 pos { get; set; } = Vector2.Zero;
        private float progress { get; set; } = 0;
        public override void Update(GameTime gameTime)
        {
            Vector2 mouseWorldTransformed = Vector2.TransformNormal(Main.MouseWorld - Main.screenPosition, Main.UIScaleMatrix) + Main.screenPosition;

            Point tileCoords = mouseWorldTransformed.ToTileCoordinates();
            Tile tile = Framing.GetTileSafely(mouseWorldTransformed.ToTileCoordinates());
            if (tile.HasTile && tile.TileType == ModContent.TileType<MushionSeedsTile>())
            {
                Point topleft = MushionSeedsTile.GetTopLeft(tileCoords.X, tileCoords.Y);
                if ((TileEntity.TryGet(new Point16(topleft), out TileEntity entity)) && entity is MushionSeedsGrowth mushionEntity)
                {
                    pos = topleft.ToWorldCoordinates();
                    pos += new Vector2(8, 8);
                    size = MathHelper.Lerp(size, 1, 0.25f);
                    progress = mushionEntity.growth / 4;
                }
            } else
            {
                size = MathHelper.Lerp(size, 0, 0.25f);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (size < 0.01f) return;
            Texture2D tex = ModContent.Request<Texture2D>("DTZ/UI/GrowthBar").Value;
            Vector2 worldDrawPos = pos + new Vector2(0, -50 * size);
            Vector2 drawPos = (worldDrawPos - Main.screenPosition);
            drawPos /= Main.UIScale;

            float scale = size;
            if (scale >= 0.98f) scale = 1;

            spriteBatch.Draw(tex, drawPos, null, Color.White, 0, tex.Size() / 2, scale, SpriteEffects.None, 1f); //the amount of point to vector2 conversion is SICKENING

            int scaledWidth = (int)(24 * progress * scale);
            int scaledHeight = (int)(12 * scale);
            int scaledX = (int)(14 * scale) + (int)(tex.Width * (0.5f - scale/2));
            int scaledY = (int)(14 * scale) + (int)(tex.Height * (0.5f - scale / 2));

            Rectangle rect = new Rectangle(
                (int)drawPos.X - tex.Width / 2 + scaledX,
                (int)drawPos.Y - tex.Height / 2 + scaledY,
                scaledWidth,
                scaledHeight);

            Rectangle fade = rect;
            fade.Inflate(1, 1);

            Color color = Color.Lerp(Color.DeepSkyBlue, Color.LightSkyBlue, progress) * size;

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, fade, color * 0.5f);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, color);
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
