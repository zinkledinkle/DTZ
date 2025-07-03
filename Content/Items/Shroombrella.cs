using DTZ.Systems;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using DTZ.Content.Projectiles;

namespace DTZ.Content.Items
{
    public class Shroombrella : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 52;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 50);

            Item.damage = 10;
            Item.DamageType = ModContent.GetInstance<ShroomyMelee>();
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.knockBack = 2;

            Item.holdStyle = ItemHoldStyleID.HoldUp;
        }
        private int timer = 0;
        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemRotation = player.controlDown ? MathHelper.PiOver2 * player.direction : 0;
            Vector2 offset = player.controlDown ?
                new Vector2(10 * player.direction, 25) :
                new Vector2(20 * player.direction, 0);
            player.itemLocation -= offset;
        }
        public override void HoldItem(Player player)
        {
            if (player.controlDown) return;

            if (player.velocity.Y > 0) player.position.Y -= player.velocity.Y * 0.75f;
            if (timer % 5 == 0) Dust.NewDust(player.Center + new Vector2(0, -30), 1, 1, DustID.MushroomTorch);
            if (timer == 20)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + new Vector2(0, -30), new Vector2(Main.rand.NextFloatDirection(), 5), ModContent.ProjectileType<ShroombrellaSpore>(), Item.damage, Item.knockBack, player.whoAmI);
            }
            timer += timer < 20 ? 1 : -21;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Umbrella)
                .AddIngredient(ItemID.GlowingMushroom, 50)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
