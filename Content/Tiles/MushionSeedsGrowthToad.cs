

using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria;
using Mycology.Content.Projectiles;

namespace Mycology.Content.Tiles
{
    public class MushionSeedsGrowthToad : MushionSeedsGrowthBase
    {
        protected override Color DefaultBaseColor => Color.GreenYellow;
        protected override int ProjectileID => ModContent.ProjectileType<ToadMushion>();
        public MushionSeedsGrowthToad()
        {
            growRate = Main.rand.Next(3000, 4200);
        }
        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == ModContent.TileType<MushionSeedsTileToad>();
        }
        public override void Update()
        {
            base.Update();
        }
    }
    public class MushionSeedsTileToad : MushionSeedsTileBase
    {
        public override string Texture => "Mycology/Content/Tiles/MushionSeedsGrowthToad_sheet";
        protected override MushionSeedsGrowthBase GetTileEntityInstance() => ModContent.GetInstance<MushionSeedsGrowthToad>();
        public override bool CanPlace(int i, int j) => MushionSeedsGrowthBase.HasMud(i, j);
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }
}