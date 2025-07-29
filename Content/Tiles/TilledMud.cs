using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Mycology.Content.Tiles
{
    public class TilledMud : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][TileID.Mud] = true;
            Main.tileMerge[Type][TileID.MushroomGrass] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            VanillaFallbackOnModDeletion = TileID.Mud;
            MineResist = 0.6f;
            HitSound = SoundID.Dig;

            DustType = DustID.Mud;

            AddMapEntry(new Color(314, 247, 271), Language.GetText("TilledMud"));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 2 : 5;
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ItemID.MudBlock, 1);
        }
    }
}
