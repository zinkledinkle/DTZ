using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mycology.Systems;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace Mycology.Content.Items.Accessories
{
    public class ShroomyEmblem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(silver: 50);
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(ModContent.GetInstance<PureShroomyDamage>()) += 0.15f;
            player.GetDamage(ModContent.GetInstance<ShroomyMelee>()) += 0.15f;
            player.GetDamage(ModContent.GetInstance<ShroomyRanged>()) += 0.15f;
            player.GetDamage(ModContent.GetInstance<ShroomyMagic>()) += 0.15f;
            player.GetDamage(ModContent.GetInstance<ShroomySummon>()) += 0.15f;
        }
    }
}
