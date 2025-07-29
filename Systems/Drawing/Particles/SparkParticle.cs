using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria;

namespace Mycology.Systems.Drawing.Particles
{
    public class SparkParticle(Vector2 position, Vector2 velocity, Color color, float rotation, float scale) : Particle(position, velocity, color, rotation, scale)
    {
        public override string Texture => "Terraria/Images/Extra_98";
        public override void Update()
        {
            base.Update();
            Scale *= 0.95f;
            Velocity *= 0.97f;
            Velocity += new Vector2(0, 0.1f);
            if (Scale < 0.05f) Kill();
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;

            Lighting.AddLight(Position, TorchID.White);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = TextureData.Size() / 2;
            Vector2 drawPos = Position - Main.screenPosition;
            lightColor.A = 0;
            spriteBatch.Draw(TextureData, drawPos, null, lightColor * Scale, Rotation, origin, Scale, SpriteEffects.None, 1f);
            return false;
        }
    }
}
