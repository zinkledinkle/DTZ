using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Mycology;
using Microsoft.Xna.Framework;
using Mycology.Content.Buffs;
using Mycology.Content.Projectiles;

namespace Mycology.Content.Items
{
    public class FairyDust : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 44;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.UseSound = SoundID.Item44;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.buffType = ModContent.BuffType<ZinklebotPetBuff>();
            //Item.shoot = ModContent.ProjectileType<ZinklebotPet>();
            Item.buffTime = 99999;
        }
    }
}
