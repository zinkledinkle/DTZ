using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Projectiles
{
    public class ShroombrellaSpore : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.02f, 5);
            if (Main.rand.NextBool(5)) Dust.NewDust(Projectile.Center, 1, 1, DustID.MushroomTorch);
        }
    }
}
