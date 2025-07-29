

using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria;
using Mycology.Content.Projectiles;

namespace Mycology.Content.Tiles
{
    public class MushionSeedsGrowthGlowing : MushionSeedsGrowthBase
    {
        protected override Color DefaultBaseColor => Color.LightSkyBlue;
        protected override int ProjectileID => ModContent.ProjectileType<GlowingMushion>();
        public MushionSeedsGrowthGlowing()
        {
            growRate = Main.rand.Next(3000, 4200);
        }
        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == ModContent.TileType<MushionSeedsTileGlowing>();
        }
        public override void Update()
        {
            base.Update();
        }
    }
    public class MushionSeedsTileGlowing : MushionSeedsTileBase
    {
        public override string Texture => "Mycology/Content/Tiles/MushionSeedsGrowth_sheet";
        protected override MushionSeedsGrowthBase GetTileEntityInstance() => ModContent.GetInstance<MushionSeedsGrowthGlowing>();
        public override bool CanPlace(int i, int j) => MushionSeedsGrowthBase.HasMud(i, j);
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }
}