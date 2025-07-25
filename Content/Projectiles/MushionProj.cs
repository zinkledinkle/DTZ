﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DTZ.Content.Projectiles
{
    public abstract class MushionProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.damage = 1;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override bool CanHitPlayer(Player target)
        {
            return target.whoAmI == Projectile.ai[2];
        }
        public override void AI()
        {
            var player = Main.player[(int)Projectile.ai[2]];

            Projectile.damage = 1;
            var goal = Projectile.Center.DirectionTo(player.Center);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, goal * 8f, 0.1f);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            int amount = target.GetModPlayer<MycologyPlayer>().MushionHealAmount;

            modifiers.Cancel();
            target.Heal(amount);
            Projectile.Kill();
        }
    }
}
