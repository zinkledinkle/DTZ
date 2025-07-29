using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria;
using System;

namespace Mycology.Systems.Drawing.Particles
{
    public class WindParticle(Vector2 position, Vector2 velocity, Color color, float rotation = 0, float scale = 1) : Particle(position, velocity, color, rotation, scale)
    {
        //public override string Texture => "Mycology/Systems/Drawing/Particles/WindParticle";
        private int timer;
        public override void Update()
        {
            base.Update();
            float sine = MathF.Sin(timer * MathHelper.TwoPi / 60f);
            Velocity = (Velocity.ToRotation() + sine / 100).ToRotationVector2() * Velocity.Length();
            if (Scale < 0.05f)
            {
                Kill();
            }
            Scale -= 0.01f;
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
            timer++;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = TextureData.Size() / 2;
            Vector2 drawPos = Position - Main.screenPosition;
            spriteBatch.Draw(TextureData, drawPos, null, lightColor * Scale * 0.5f, Rotation, origin, new Vector2(0.5f * Scale, Scale), SpriteEffects.None, 1f);
            return false;
        }
    }
}
