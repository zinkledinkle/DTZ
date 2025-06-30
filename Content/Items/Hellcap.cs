using DTZ.Content.Items.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Items
{
    public class Hellcap : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 5);
            Item.maxStack = 9999;

            Item.ammo = ItemID.Mushroom;
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.shootSpeed = 3;
            Item.shoot = ModContent.ProjectileType<HellcapShot>();
        }
    }
    public class HellcapShot : MushroomShotBase
    {
        protected override float Weight => 0.17f;
        public override string Texture => ModContent.GetInstance<Hellcap>().Texture;
        protected override float RotationSpeed => 0.15f;
        protected override int DustType => DustID.Torch;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 120, false);
        }
    }
}
