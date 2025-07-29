using Mycology.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Mycology.Content.Tiles
{
    public class MushionSeedsGrowthHell : MushionSeedsGrowthBase
    {
        protected override int ProjectileID => ModContent.ProjectileType<HellcapMushion>();
        protected override Color DefaultBaseColor => Color.DarkRed;
        public MushionSeedsGrowthHell()
        {
            growRate = Main.rand.Next(3000, 4200);
        }
        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == ModContent.TileType<MushionSeedsTileHell>();
        }
        public override void Update()
        {
            base.Update();
        }
    }
    public class MushionSeedsTileHell : MushionSeedsTileBase
    {
        public override string Texture => "Mycology/Content/Tiles/MushionSeedsGrowthHell_sheet";
        protected override MushionSeedsGrowthBase GetTileEntityInstance() => ModContent.GetInstance<MushionSeedsGrowthHell>();
        public override bool CanPlace(int i, int j) => MushionSeedsGrowthBase.HasMud(i, j);
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }
}