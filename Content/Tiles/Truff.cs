using Microsoft.Xna.Framework;
using Mycology.Content.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Mycology.Content.Tiles
{
    public class Truff : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][TileID.Mud] = true;
            Main.tileMerge[Type][TileID.MushroomGrass] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            VanillaFallbackOnModDeletion = TileID.Stone;
            MineResist = 2f;
            HitSound = SoundID.Dig;

            DustType = DustID.Stone;

            AddMapEntry(new Color(128, 128, 128), Language.GetText("Truff"));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 2 : 5;
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<Truff_item>());
        }
    }
}
