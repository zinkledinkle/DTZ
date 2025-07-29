using Mycology.Systems;
using Mycology.Systems.Drawing.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Mycology.Content.Items
{
    public class SUPERMEGAULTRADEBUGITEM9000 : ModItem
    {
        public override string Texture => "Terraria/Images/Item_0";
        public override void SetDefaults()
        {
            Item.autoReuse = false;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = int.MaxValue;
            Item.rare = ItemRarityID.Red;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool? UseItem(Player player)
        {
            Particle.NewParticle(Particle.ParticleID.Spark, Main.MouseWorld, Vector2.Zero, Color.White, 0, 1);
            return base.UseItem(player);
        }
    }
}
