using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Mycology.Content.Buffs
{
    public class FungusBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FungusPlayer>().active = true;
        }
    }
    public class FungusPlayer : ModPlayer
    {
        private float intensity = 0;
        public bool active = false;
        public override void ResetEffects()
        {
            intensity = MathHelper.Lerp(intensity, active ? 1 : 0, 0.01f);
            active = false;
        }
        public override void PostUpdate()
        {
            if (intensity > 0.01f)
            {
                if (!Filters.Scene["fungus"].Active)
                {
                    Filters.Scene.Activate("fungus");
                }
                Filters.Scene["fungus"].GetShader().UseIntensity(intensity);
            } else
            {
                Filters.Scene.Deactivate("fungus");
            }
        }
    }
}
