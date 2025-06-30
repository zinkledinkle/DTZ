using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace DTZ.Systems.Drawing.Particles
{
    public abstract class Particle
    {
        protected virtual string Texture { get; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public Particle(Vector2 position, Vector2 velocity, Color color, float rotation, float scale)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Rotation = rotation;
            Scale = scale;
        }
        public virtual void Update()
        {
            Position += Velocity;
        }
        public virtual void Draw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = tex.Size() / 2;
            Vector2 drawPos = Position - Main.screenPosition;
            spriteBatch.Draw(tex, drawPos, null, lightColor, Rotation, origin, Scale, SpriteEffects.None, 1f);
        }
        public virtual bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => true;
        public Particle NewParticle(ParticleID type, Vector2 position, Vector2 velocity, Color color, float rotation, float scale)
        {
            Particle particle = type switch
            {
                //ParticleID.Basic => new Particle(position, velocity, color, rotation, scale), 
                _ => throw new ArgumentException("Invalid particle ID idiot"),
            };
            ParticleManager.particle.Append(particle);
            return particle;
        }
        public enum ParticleID
        {
            Basic = 0
        }
    }
    public class ParticleManager : ModSystem
    {
        public static Particle[] particle = [];
        public override void Load() => On_Main.DrawNPCs += Draw;
        public override void PreUpdateDusts()
        {
            foreach (var particle in particle)
            {
                particle.Update();
            }
        }
        private void Draw(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            foreach (var particle in particle)
            {
                var sb = Main.spriteBatch;
                Color lightColor = Lighting.GetColor(particle.Position.ToTileCoordinates());
                if (particle.PreDraw(sb, lightColor)) particle.Draw(sb, lightColor);
            }
            orig(self, behindTiles);
        }
    }
}
