using DTZ.Content.Items.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DTZ.Systems;

namespace DTZ.Content.Items
{
    public class Toadstool : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 5);
            Item.maxStack = 9999;

            Item.ammo = ItemID.Mushroom;
            Item.damage = 2;
            Item.DamageType = ModContent.GetInstance<ShroomyRanged>();
            Item.shootSpeed = -1;
            Item.shoot = ModContent.ProjectileType<ToadstoolShot>();
        }
    }
    public class ToadstoolShot : MushroomShotBase
    {
        protected override float Weight => 0.3f;
        public override string Texture => ModContent.GetInstance<Toadstool>().Texture;
        protected override float RotationSpeed => 0.3f;
        protected override int DustType => DustID.GreenMoss;
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(3, 4); i++)
            {
                Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(5, 5), ProjectileID.SporeCloud, Projectile.damage / 4, 0, Projectile.owner);
            }
        }
    }
}
