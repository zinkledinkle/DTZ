

using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria;
using Mycology.Content.Projectiles;

namespace Mycology.Content.Tiles
{
    public class MushionSeedsGrowthIce : MushionSeedsGrowthBase
    {
        protected override Color DefaultBaseColor => Color.AliceBlue;
        protected override int ProjectileID => ModContent.ProjectileType<IceliumMushion>();
        public MushionSeedsGrowthIce()
        {
            growRate = Main.rand.Next(3000, 4200);
        }
        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == ModContent.TileType<MushionSeedsTileIce>();
        }
        public override void Update()
        {
            base.Update();
        }
    }
    public class MushionSeedsTileIce : MushionSeedsTileBase
    {
        public override string Texture => "Mycology/Content/Tiles/MushionSeedsGrowthIce_sheet";
        protected override MushionSeedsGrowthBase GetTileEntityInstance() => ModContent.GetInstance<MushionSeedsGrowthIce>();
        public override bool CanPlace(int i, int j) => MushionSeedsGrowthBase.HasMud(i, j);
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }
}