using Mycology.Content.Items.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Mycology.Systems;
using Terraria.Localization;

namespace Mycology.Content.Items
{
    public class GUNKstool : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 38;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(platinum: 9999);
            Item.maxStack = 9999;

            Item.ammo = ItemID.Mushroom;
            Item.damage = 9999;
            Item.ArmorPenetration = 9999999;
            Item.DamageType = ModContent.GetInstance<ShroomyRanged>();
            Item.shootSpeed = -6;
            Item.shoot = ModContent.ProjectileType<GUNKstoolShot>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (var tooltip in tooltips)
            {
                if (tooltip.Text.Contains("damage"))
                {
                    tooltip.Text = Item.damage + " [c/12bfff:fuck] you [c/ff3311:terry] " + Language.GetText("damage");
                }
            }
        }
    }
    public class GUNKstoolShot : MushroomShotBase
    {
        protected override float Weight => 0.9f;
        public override string Texture => ModContent.GetInstance<GUNKstool>().Texture;
        protected override float RotationSpeed => 0.1f;
        protected override int DustType => DustID.GreenMoss;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 48;
            Projectile.height = 38;
        }
        public override void AI()
        {
            base.AI();
            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
        }
        public override void OnKill(int timeLeft)
        {
            foreach(NPC n in Main.npc)
            {
                if (n.Distance(Projectile.Center) < 500)
                {
                    n.defense = 0;
                    n.takenDamageMultiplier = 1;
                    n.SimpleStrikeNPC(100000, 0);
                }
            }
            GUNK.GUNKLE();
            for (int i = 0; i < Main.rand.Next(200, 300); i++)
            {
                Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(20, 20), ProjectileID.SporeCloud, Projectile.damage / 4, 0, Projectile.owner);
            }
        }
    }
}
