using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Mycology.Content.Projectiles
{
    public class UhBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 26;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Projectile.frameCounter++;

            if (Projectile.frame == 0)
            {
                if (Projectile.frameCounter >= 15)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = 1;
                }
            }
            else
            {
                if (Projectile.frameCounter >= 10)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;

                    if (Projectile.frame > 2)
                    {
                        Projectile.frame = 1;
                    }
                }
            }

            int npcIndex = (int)Projectile.ai[0];
            if (npcIndex >= 0 && npcIndex < Main.npc.Length)
            {
                NPC npc = Main.npc[npcIndex];
                if (npc.active)
                {
                    Projectile.Center = npc.Center + new Vector2(0, -40);
                }
                else
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.Kill();
            }
        }
    }
}
