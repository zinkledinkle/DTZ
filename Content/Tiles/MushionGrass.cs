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
    public class MushionGrass : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<PackedMud>();
            TileID.Sets.Mud[Type] = true;

            Main.tileLighted[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;

            Main.tileBlockLight[Type] = true;
            Main.tileSolid[Type] = true;
            TileID.Sets.Conversion.JungleGrass[Type] = true;

            MineResist = 0.5f;
            HitSound = SoundID.Dig;
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<PackedMud_item>());
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            b += 0.3f;
            r += 0.1f;
        }
    }
}
