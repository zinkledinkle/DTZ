using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Mycology
{
    public class GreenGrapePlayer : ModPlayer
    {
        public bool greenSkin;

        private readonly Color grapeGreen = new Color(120, 200, 80);

        public override void ResetEffects()
        {
            greenSkin = false;
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (greenSkin)
            {
                drawInfo.colorHead = grapeGreen;
                drawInfo.colorBodySkin = grapeGreen;
                drawInfo.colorLegs = grapeGreen;
                drawInfo.colorHair = grapeGreen;

                drawInfo.colorArmorHead = grapeGreen;
                drawInfo.colorArmorBody = grapeGreen;
                drawInfo.colorArmorLegs = grapeGreen;

                drawInfo.colorUnderShirt = grapeGreen;
                drawInfo.colorShoes = grapeGreen;
                drawInfo.colorShirt = grapeGreen;
                drawInfo.colorPants = grapeGreen;
            }
        }
    }
}