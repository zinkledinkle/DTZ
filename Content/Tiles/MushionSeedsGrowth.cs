using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DTZ.Content.Tiles
{
    public class MushionSeedsGrowth : ModTile
    {
        private int phase = 0;
        private int[] frameSizes = { 8, 12, 20, 26 };
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addAlternate(Type);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return base.TileFrame(i, j, ref resetFrame, ref noBreak);
        }
        public override bool CanPlace(int i, int j)
        {
            int tilledMud = ModContent.TileType<TilledMud>();
            return Framing.GetTileSafely(i, j+1).TileType == tilledMud && Framing.GetTileSafely(i + 1, j + 1).TileType == tilledMud;
        }
    }
}
