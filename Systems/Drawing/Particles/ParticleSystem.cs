using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace DTZ.Systems.Drawing.Particles
{
    public abstract class Particle(Vector2 position, Vector2 velocity, Color? color = null, float rotation = 0, float scale = 1, float alpha = 1)
    {
        public virtual string Texture { get; }
        protected virtual Texture2D TextureData => ParticleTextureAssets.Get(Texture);
        public Vector2 Position { get; set; } = position;
        public Vector2 Velocity { get; set; } = velocity;
        public Color Color { get; set; } = color ?? Color.White;
        public float Rotation { get; set; } = rotation;
        public float Scale { get; set; } = scale;
        public float Alpha { get; set; } = alpha;
        public void Kill()
        {
            ParticleManager.particle.Remove(this);
        }
        public virtual void Update()
        {
            Position += Velocity;
        }
        public virtual void Draw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = TextureData.Size() / 2;
            Vector2 drawPos = Position - Main.screenPosition;
            spriteBatch.Draw(TextureData, drawPos, null, lightColor * Alpha, Rotation, origin, Scale, SpriteEffects.None, 1f);
        }
        public virtual bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => true;
        public static Particle NewParticle(ParticleID type, Vector2 position, Vector2 velocity, Color color, float rotation, float scale)
        {
            Particle particle = type switch
            {
                ParticleID.Spark => new SparkParticle(position, velocity, color, rotation, scale),
                _ => throw new ArgumentException("Invalid particle ID idiot"),
            };
            ParticleManager.particle.Add(particle);
            return particle;
        }
        public enum ParticleID
        {
            Spark = 0,
        }
    }
    public class ParticleManager : ModSystem
    {
        public static List<Particle> particle = [];
        public override void Load()
        {
            On_Main.DrawNPCs += Draw;
            var types = typeof(Particle).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Particle)) && !t.IsAbstract);
            foreach (var type in types)
            {
                Particle particle = (Particle)Activator.CreateInstance(type, Vector2.Zero, Vector2.Zero, Color.White, 0f, 1f);
                ParticleTextureAssets.Textures.Add(particle.Texture, ModContent.Request<Texture2D>(particle.Texture).Value);
            }
        }
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
                Color lightColor = particle.Color.MultiplyRGBA(Lighting.GetColor(particle.Position.ToTileCoordinates()));
                if (particle.PreDraw(sb, lightColor)) particle.Draw(sb, lightColor);
            }
            orig(self, behindTiles);
        }
    }
    public static class ParticleTextureAssets
    {
        public static readonly Dictionary<string, Texture2D> Textures = [];
        public static Texture2D Get(string name)
        {
            if (Textures.TryGetValue(name, out Texture2D texture))
            {
                return texture;
            }
            throw new KeyNotFoundException($"Particle texture '{name}' not found :(");
        }
    }
}
