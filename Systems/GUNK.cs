using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Mycology.Systems
{
    public class GUNK : ModSystem
    {
        public static float alpha = 0f;
        private static Texture2D gunkle;
        private static SoundStyle gunkitupinhere = new("Mycology/Systems/GetGunked")
        {
            Volume = 11,
            PitchVariance = 0
        };
        public override void NetSend(BinaryWriter writer) { }// => writer.Write(alpha);
        public override void NetReceive(BinaryReader reader) { }// => alpha = reader.ReadSingle();
        public override void Load()
        {
            gunkle = ModContent.Request<Texture2D>("Mycology/Systems/ThisIsYourFaultTerra", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
        public override void UpdateUI(GameTime gameTime)
        {
            alpha = Math.Max(alpha - 0.02f, 0);
        }
        public static void GUNKLE()
        {
            SoundEngine.PlaySound(gunkitupinhere);
            alpha = 1;
        }
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gunkle, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * alpha);
        }
        public class GUNKSHAKE : ModPlayer
        {
            public override void ModifyScreenPosition()
            {
                Main.screenPosition += Main.rand.NextVector2CircularEdge(GUNK.alpha * 100, GUNK.alpha * 100);
            }
        }
    }
}
