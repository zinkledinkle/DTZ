using Terraria.ID;
using Terraria.ModLoader;
using Mycology.Content.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Mycology.Content.Buffs;
using Mycology;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Audio;

namespace Mycology.Content.Items
{
    public class Batteries : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.consumable = true;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(platinum: 1);
        }
        public override bool? UseItem(Player player)
        {
            player.ClearBuff(ModContent.BuffType<LowBatteryDebuff>());

            SoundStyle cleanseSound = new SoundStyle("Mycology/Assets/Sounds/ChargeBattery")
            {
                Volume = 1f,
                PitchVariance = 0.1f
            };
            SoundEngine.PlaySound(cleanseSound, player.Center);

            if (Main.myPlayer == player.whoAmI)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-6f, -2f));
                    Projectile.NewProjectile(
                        player.GetSource_ItemUse(Item),
                        player.Center,
                        velocity,
                        ModContent.ProjectileType<BatteryProjectile>(),
                        0,
                        0f,
                        player.whoAmI
                    );
                }
            }

            return true;
        }
    }
}
