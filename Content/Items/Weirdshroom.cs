using System.Reflection;
using Microsoft.Xna.Framework;
using Mycology.Content.Buffs;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace Mycology.Content.Items
{
    public class Weirdshroom : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 26;
            Item.DefaultToFood(22, 26, ModContent.BuffType<WeirdshroomBuff>(), 36000);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 10);
        }
    }
    public class WeirdshroomBuff : ModBuff
    {
        public override string Texture => ModContent.GetInstance<FungusBuff>().Texture;
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<WeirdshroomPlayer>().on = true;
        }
    }
    public class WeirdshroomPlayer : ModPlayer
    {
        public override void Load()
        {
            On_Player.ResizeHitbox += ResizeHitbox;
            On_PlayerDrawLayers.DrawPlayer_TransformDrawData += Stretch;
        }
        public override void Unload()
        {
            On_Player.ResizeHitbox -= ResizeHitbox;
            On_PlayerDrawLayers.DrawPlayer_TransformDrawData -= Stretch;
        }
        private int intro = 0;
        public bool on = false;
        private bool wasOn = false;
        public Vector2 stretch = new(1, 1);
        private float IMFUCKINTWEAKIN = 0f;
        public override void ResetEffects()
        {
            if (on)
            {
                if (!wasOn)
                {
                    intro = 480;
                    wasOn = true;
                }
                if (intro > 0)
                {
                    IMFUCKINTWEAKIN += (480 - intro)/600f;
                    float shakeAmountX = MathF.Sin(IMFUCKINTWEAKIN);
                    float shakeAmountY = MathF.Cos(IMFUCKINTWEAKIN * 0.854f);

                    float progress = intro / 480f;
                    float amount = MathHelper.Lerp(0.5f, 0, progress);
                    shakeAmountX *= amount;
                    shakeAmountY *= amount;

                    stretch = new(1 + shakeAmountX, 1 + shakeAmountY);

                    intro--;
                    if (intro == 0)
                    {
                        stretch = new Vector2(1, 0.1f);
                    }
                } else
                {
                    stretch = Vector2.Lerp(stretch, new Vector2(1, 2), 0.35f);
                    if (stretch.Y > 1.98f) stretch.Y = 2;
                }
            } else
            {
                IMFUCKINTWEAKIN = 0;
                wasOn = false;
                stretch = Vector2.Lerp(stretch, new(1, 1), 0.25f);
            }
            on = false;
        }
        private void Stretch(On_PlayerDrawLayers.orig_DrawPlayer_TransformDrawData orig, ref PlayerDrawSet drawinfo)
        {
            orig(ref drawinfo);
            for (int i = 0; i < drawinfo.DrawDataCache.Count; i++)
            {
                Vector2 stretchStoredForSomeReasonIDK = drawinfo.drawPlayer.GetModPlayer<WeirdshroomPlayer>().stretch;

                var input = drawinfo.DrawDataCache[i];
                Rectangle rect = input.destinationRectangle;

                int srcWidth = input.sourceRect?.Width ?? input.texture.Width;
                int srcHeight = input.sourceRect?.Height ?? input.texture.Height;

                int X = input.useDestinationRectangle ? input.destinationRectangle.Width : srcWidth;
                int Y = input.useDestinationRectangle ? input.destinationRectangle.Height : srcHeight;
                Vector2 origin = input.origin;
                Vector2 offset = new(
                    origin.X * (stretchStoredForSomeReasonIDK.X - 1f),
                    origin.Y * (stretchStoredForSomeReasonIDK.Y - 1f)
                );

                Vector2 inputPosition = input.position + offset;
                inputPosition -= new Vector2(0, 56 * (stretchStoredForSomeReasonIDK.Y - 1));
                inputPosition.X -= (stretchStoredForSomeReasonIDK.X - 1f) * (srcWidth / 2f);

                Rectangle stretched = new((int)inputPosition.X, (int)inputPosition.Y, (int)(X * stretchStoredForSomeReasonIDK.X), (int)(Y * stretchStoredForSomeReasonIDK.Y));
                var output = new DrawData(input.texture, stretched, input.sourceRect, input.color, input.rotation, input.origin, input.effect);
                drawinfo.DrawDataCache[i] = output;
            }
        }
        private void ResizeHitbox(On_Player.orig_ResizeHitbox orig, Player self)
        {
            if (Main.gameMenu || self.mount == null || !self.TryGetModPlayer<WeirdshroomPlayer>(out var wsp) || wsp.stretch != new Vector2(1, 2))
            {
                orig(self); return;
            }
            bool isNull = self.mount.Type == -1;
            int oldBoost = -1;
            bool reset = self.mount._data is null;
            bool prevActive = self.mount._active;

            if (reset) self.mount._data = new Mount.MountData();

            int offset = Player.defaultHeight;
            self.mount._data.heightBoost = offset;
            self.mount._active = true;

            orig(self);

            self.mount._active = prevActive;

            if (reset) self.mount._data = null;
            if (!isNull) self.mount._data.heightBoost = oldBoost;
            else self.mount.Reset();

            //thanks gabe :))
        }
    }
}
