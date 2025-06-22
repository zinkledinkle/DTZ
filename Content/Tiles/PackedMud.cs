using DTZ.Content.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Tiles
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
            Main.tileMerge[TileID.Stone][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<MushionGrass>()] = true;
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<PackedMud_item>());
        }
    }
}
