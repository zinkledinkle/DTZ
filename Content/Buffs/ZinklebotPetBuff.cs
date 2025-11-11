using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Mycology.Content.Projectiles;

namespace Mycology.Content.Buffs
{
    public class ZinklebotPetBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ZinklebotPet>()] <= 0 && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    player.GetSource_Buff(buffIndex),
                    player.Center,
                    Microsoft.Xna.Framework.Vector2.Zero,
                    ModContent.ProjectileType<ZinklebotPet>(),
                    0,
                    0f,
                    player.whoAmI
                );
            }
            player.buffTime[buffIndex] = 18000;
        }
    }
}
