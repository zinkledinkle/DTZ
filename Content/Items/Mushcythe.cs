using DTZ.Content.Tiles;
using DTZ.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Items
{
    public class Mushcythe : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 38;
            Item.damage = 8;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.knockBack = 0.1f;
            Item.crit = 0;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(silver: 10);
        }
        public override bool? UseItem(Player player)
        {
            ShroomGen.PlaceShrooms(150, 20, 150);
            Point16 tileCoords = Main.MouseWorld.ToTileCoordinates16();
            Tile tile = Framing.GetTileSafely(tileCoords);
            int type = tile.TileType;
            if (type is TileID.Mud or TileID.MushroomGrass)
            {
                tile.TileType = (ushort)ModContent.TileType<TilledMud>();
                SoundEngine.PlaySound(SoundID.Grass, Main.MouseWorld);
                for (int i = 0; i < Main.rand.Next(1, 4); i++) {
                    Vector2 vel = -Main.rand.NextVector2Unit(MathHelper.PiOver4, MathHelper.PiOver2) * 2;
                    int dustType = type == TileID.MushroomGrass ? DustID.MushroomSpray : DustID.Mud;
                    Dust.NewDust(tileCoords.ToWorldCoordinates(), 16, 1, dustType, vel.X, vel.Y);
                }
                NetMessage.SendTileSquare(-1, tileCoords.X, tileCoords.Y);
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GlowingMushroom, 15)
                .AddIngredient(ItemID.Sickle)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
