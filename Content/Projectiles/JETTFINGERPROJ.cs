using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Mycology.Content.Projectiles;

namespace Mycology.Content.Projectiles;
public class JETTFINGERPROJ : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 30;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 30;
        Projectile.alpha = 0;
    }

    public override void AI()
    {
        Projectile.velocity *= 0.95f;
        Projectile.velocity.Y -= 0.1f;

        Projectile.rotation += 0.05f;

        Projectile.alpha += 8;
        if (Projectile.alpha > 255)
            Projectile.alpha = 255;
    }
}
