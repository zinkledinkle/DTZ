using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Mycology
{
	public class Mycology : Mod
	{
        public override void Load()
        {
            Asset<Effect> fungusEffect = ModContent.Request<Effect>("Mycology/Assets/Effects/BunkFungus", AssetRequestMode.ImmediateLoad);
            Filters.Scene["fungus"] = new Filter(new ScreenShaderData(fungusEffect, "fungus"), EffectPriority.VeryHigh);
            Filters.Scene["fungus"].Load();
        }
    }
}
