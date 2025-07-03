using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Fishing
{
    public class MycoRod : ModItem
    {
        private static Texture2D inventoryTexture;
        public override void Load()
        {
            if (!Main.dedServ) inventoryTexture = ModContent.Request<Texture2D>(Texture + "_inventory").Value;
        }
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodFishingPole);

            Item.width = 44;
            Item.height = 34;
            Item.shootSpeed = 15;
            Item.fishingPole = 25;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<MycoRodBobber>();
        }
        public override void HoldItem(Player player)
        {
            player.accFishingLine = true;
        }
        public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor)
        {
            if (bobber.type == Item.shoot)
            {
                lineOriginOffset = new Vector2(35, -20);
                lineColor = new Color(1, 0.89f, 0.729f);
            }
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            inventoryTexture ??= ModContent.Request<Texture2D>(Texture + "_inventory").Value;
            spriteBatch.Draw(inventoryTexture, position, null, drawColor, 0, inventoryTexture.Size()/2, scale, SpriteEffects.None, 1f);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GlowingMushroom, 50)
                .AddIngredient(ItemID.Cobweb, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class MycoRodBobber : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BobberWooden);
            DrawOriginOffsetY = -12;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(30)) Dust.NewDust(Projectile.Center, 1, 1, DustID.MushroomTorch);
        }
    }
}
