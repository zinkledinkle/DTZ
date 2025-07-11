using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Vanity.DevSets.Zinkle
{
    [AutoloadEquip(EquipType.Wings)]
    public class ZinkleHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(90);
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.width = 52;
            Item.height = 26;
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.2f;
            maxCanAscendMultiplier = 1;
            maxAscentMultiplier = 4f;
            constantAscend = 0.1f;
        }
        public override bool WingUpdate(Player player, bool inUse)
        {
            if (!player.controlJump) return true;
            int frame = player.GetModPlayer<ZinkleHatPlayer>().frame;
            Vector2 left = player.Center + new Vector2(-31, -10);
            Vector2 right = player.Center + new Vector2(14, -10);
            if (player.direction == -1)
            {
                right.X += 8;
                left.X += 6;
            }
            if (frame == 5 && Main.rand.NextBool(2))
            {
                Dust.NewDust(left, 1, 1, DustID.Torch, 0, 5);
                Dust.NewDust(right, 1, 1, DustID.Torch, 0, 5);
                Dust.NewDust(left, 1, 1, DustID.Smoke, 0, 5);
                Dust.NewDust(right, 1, 1, DustID.Smoke, 0, 5);
            }
            else if (frame == 3 && player.wingTime == 0 && Main.rand.NextBool(5))
            {
                Dust.NewDust(left, 1, 1, DustID.Smoke, 0, 5);
                Dust.NewDust(right, 1, 1, DustID.Smoke, 0, 5);
            }

            if (player.wingTime > 0 && frame > 3)
            {
                if (SoundEngine.FindActiveSound(SoundID.Item34) == null)SoundEngine.PlaySound(SoundID.Item34, player.Center);
            }
            return true;
        }
    }
    public class ZinkleHatPlayer : ModPlayer
    {
        private float frameF = 0;
        public int frame = 0;
        public override void ResetEffects()
        {
            float speed = 0.5f;
            if (Player.controlJump)
            {
                //frameF = Math.Min(frameF + speed, 5);
                frameF = (float)Math.Round(MathHelper.Lerp(frameF, 5, speed/2), 1, MidpointRounding.ToPositiveInfinity);
            } else
            {
                frameF = Math.Max(frameF - speed, -1);
            }
            frame = (int)frameF;
            if (frame == 5 && Player.wingTime == 0) frame = 3;
        }
        public override void FrameEffects()
        {
            if (Player.equippedWings?.type == ModContent.ItemType<ZinkleHat>())
            {
                Player.wings = -1;
                Player.wingFrame = -1;
            }
        }
    }
    public class ZinkleHatLayer : PlayerDrawLayer
    {
        private Texture2D Texture = null;
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Texture = ModContent.Request<Texture2D>(ModContent.GetInstance<ZinkleHat>().Texture + "_Wings", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
        }
        public override bool IsHeadLayer => true;
        private bool HasVanityWings(Player plr)
        {
            for (int i = 13; i < 19; i++)
            {
                if (plr.armor[i].wingSlot > 0) return true;
            }
            return false;
        }
        private bool InVanity(Player plr)
        {
            for (int i = 13; i < 19; i++)
            {
                if (plr.armor[i].type == ModContent.ItemType<ZinkleHat>()) return true;
            }
            return false;
        }
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.equippedWings == null) return false;
            return (drawInfo.drawPlayer.equippedWings.type == ModContent.ItemType<ZinkleHat>() && !HasVanityWings(drawInfo.drawPlayer)) || InVanity(drawInfo.drawPlayer);
        }
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (!HasVanityWings(drawInfo.drawPlayer)) PlayerDrawLayers.Wings.Hide();

            /*if (Texture == null || Texture.Width <= 1)
            {
                Texture = ModContent.Request<Texture2D>(ModContent.GetInstance<ZinkleHat>().Texture + "_Wings").Value; //why does it do this.
            }*/

            Vector2 position = (drawInfo.Center + new Vector2(-4, -19)) - Main.screenPosition;
            if (drawInfo.drawPlayer.direction == -1) position.X += 8;
            position = new Vector2((int)position.X, (int)position.Y);
            int frame = drawInfo.drawPlayer.GetModPlayer<ZinkleHatPlayer>().frame * 32;
            Rectangle source = new(0, frame, 52, 32);

            Color color = Color.White;

            drawInfo.DrawDataCache.Add(new DrawData(
                Texture,
                position,
                source,
                color,
                0,
                new Vector2(26, 16),
                1f,
                drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally
            ));
        }
    }
}
