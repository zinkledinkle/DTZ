using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Mycology.Content.Projectiles;

namespace Mycology
{
    public class LowBatteryPlayer : ModPlayer
    {
        public bool isProtected;

        public override void ResetEffects()
        {
            isProtected = false;
        }
        public override void OnRespawn()
        {
            // i feel like i need to be resetting something but i can think of what...
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            Projectile.NewProjectile(
                Player.GetSource_Death(),
                Player.Center,
                Vector2.Zero,
                ModContent.ProjectileType<LowBatteryProjectile>(),
                0, 0, Player.whoAmI
            );
        }
    }

}