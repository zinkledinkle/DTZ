using DTZ.Content.Projectiles;
using DTZ.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DTZ
{
    public class MycologyPlayer : ModPlayer //general all use player for most generic mycology things, feel free to use if the use case doesn't necessitate its own class
    {
        public int MushionHealAmount { get; set; } = 10;
        public int MaxMushions { get; set; } = 3;
        public Projectile[] mushion => Main.projectile.Where(p => p.active && p.owner == Player.whoAmI && projectileTypes.Contains(p.type)).ToArray();
        public void KillFirstMushion()
        {
            Projectile first = mushion.FirstOrDefault();
            first?.Kill();
        }
        public int MushionCount => mushion.Length;
        public static readonly int[] projectileTypes = new[]
{
            ModContent.ProjectileType<GlowingMushion>(),
            ModContent.ProjectileType<IceliumMushion>(),
            ModContent.ProjectileType<HellcapMushion>(),
            ModContent.ProjectileType<ToadMushion>(),
        };
        public override void ResetEffects()
        {
            if (MushionCount > MaxMushions)
            {
                KillFirstMushion();
            }

            MushionHealAmount = 10;
            MaxMushions = 3;
        }
    }
}
