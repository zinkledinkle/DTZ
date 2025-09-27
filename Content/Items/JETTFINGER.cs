using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mycology.Systems;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mycology.Content.Projectiles;
using Mycology.Content.Enemies;

namespace Mycology.Content.Items
{
    public class JETTFINGER : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 26;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = 10000;
            Item.rare = ItemRarityID.Pink;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item4;
        }

        private bool playingAnimation = false;
        private int animFrame = 0;
        private int animTimer = 0;

        public override bool CanUseItem(Player player)
        {
            return !playingAnimation;
        }

        public override bool? UseItem(Player player)
        {
            Vector2 basePos = player.Center - new Vector2(0, player.height / 2 + 16);

            for (int i = 0; i < 15; i++)
            {
                Vector2 spawnPos = basePos + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-10f, 10f));
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));

                Projectile.NewProjectile(
                    player.GetSource_ItemUse(Item),
                    spawnPos,
                    velocity,
                    ModContent.ProjectileType<JETTFINGERPROJ>(),
                    0,
                    0f,
                    player.whoAmI
                );
            }

            playingAnimation = true;
            animFrame = 0;
            animTimer = 0;

            return true;
        }


        public override void UpdateInventory(Player player)
        {
            if (playingAnimation)
            {
                animTimer++;
                if (animTimer >= 5)
                {
                    animTimer = 0;
                    animFrame++;

                    if (animFrame >= 16)
                    {
                        playingAnimation = false;
                        animFrame = 0;
                        DoItemEffect(player);
                    }
                }
            }
        }

        private void DoItemEffect(Player player)
        {
            JUNK.JUNKLE(); //MAHAHAHAHAHAHAHHAHAHAHAHAHAHAHA
            player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " shouldn't have done that."), 9999, 0);
            if (Main.myPlayer == player.whoAmI)
            {
                for (int i = 0; i < 3; i++)
                {
                    float offsetX = Main.rand.NextFloat(-100f, 100f);
                    float offsetY = Main.rand.NextFloat(-50f, 50f);

                    NPC.NewNPC(
                        player.GetSource_ItemUse(Item),
                        (int)(player.Center.X + offsetX),
                        (int)(player.Center.Y + offsetY),
                        ModContent.NPCType<Oppsgother>()
                    );
                }
            }
        }
        public class MyAnimLayer : PlayerDrawLayer
        {
            public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);

            public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
            {
                return drawInfo.drawPlayer.HeldItem.ModItem is JETTFINGER myItem && myItem.playingAnimation;
            }

            protected override void Draw(ref PlayerDrawSet drawInfo)
            {
                JETTFINGER myItem = (JETTFINGER)drawInfo.drawPlayer.HeldItem.ModItem;

                Texture2D animTexture = ModContent.Request<Texture2D>("Mycology/Assets/Textures/JETTFINGERANIM").Value;
                int frameHeight = animTexture.Height / 16;
                Rectangle sourceRect = new Rectangle(0, frameHeight * myItem.animFrame, animTexture.Width, frameHeight);

                Vector2 drawPos = drawInfo.drawPlayer.Center - new Vector2(0, drawInfo.drawPlayer.height / 2 + 16) - Main.screenPosition;
                drawInfo.DrawDataCache.Add(new DrawData(animTexture, drawPos, sourceRect, Color.White, 0f, new Vector2(animTexture.Width / 2f, frameHeight / 2f), 1f, SpriteEffects.None, 0));
            }
        }
    }
}
