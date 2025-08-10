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
    //I TOTALLY MADE MY OWN SYSTEM....
    public class JUNK : ModSystem
    {
        public static float alpha = 0f;
        private static Texture2D junkle;
        private static SoundStyle JETTFINGERSCREAM = new("Mycology/Systems/JETTFINGERSCREAM")
        {
            Volume = 11,
            PitchVariance = 0
        };
        public override void NetSend(BinaryWriter writer) { }// => writer.Write(alpha);
        public override void NetReceive(BinaryReader reader) { }// => alpha = reader.ReadSingle();
        public override void Load()
        {
            junkle = ModContent.Request<Texture2D>("Mycology/Systems/NoItsYOURFaultZinks", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
        public override void UpdateUI(GameTime gameTime)
        {
            alpha = Math.Max(alpha - 0.02f, 0);
        }
        public static void JUNKLE()
        {
            SoundEngine.PlaySound(JETTFINGERSCREAM);
            alpha = 1;
        }
        //"Jett Finger Scream" is something i NEVER would've thought i'd say
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(junkle, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * alpha);
        }
        public class JUNKSHAKE : ModPlayer
        {
            public override void ModifyScreenPosition()
            {
                Main.screenPosition += Main.rand.NextVector2CircularEdge(JUNK.alpha * 200, JUNK.alpha * 200);
            }
        }
    }
}
