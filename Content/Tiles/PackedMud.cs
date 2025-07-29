using Mycology.Content.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Mycology.Content.Tiles
{
    public class PackedMud : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.CanBeDugByShovel[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSolid[Type] = true;
            TileID.Sets.Conversion.JungleGrass[Type] = true;

            MineResist = 1;
            HitSound = SoundID.Dig;

            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileMerge[Type][TileID.Dirt] = true;
            Main.tileMerge[TileID.Dirt][Type] = true;
            Main.tileMerge[TileID.Stone][Type] = true;
            Main.tileMerge[Type][TileID.Mud] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<TilledMud>()] = true;
            Main.tileMerge[ModContent.TileType<TilledMud>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<Truff>()] = true;
            Main.tileMerge[ModContent.TileType<Truff>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<MushionGrass>()] = true;
            Main.tileMerge[ModContent.TileType<MushionGrass>()][Type] = true;

            AddMapEntry(new Microsoft.Xna.Framework.Color(0.361f, 0.267f, 0.286f), Language.GetText("packedmudmapentry"));
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<PackedMud_item>());
        }
    }
}
