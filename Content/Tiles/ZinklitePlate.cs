using Microsoft.Xna.Framework;
using Mycology.Content.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Enums;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

namespace Mycology.Content.Tiles
{
    public class ZinklitePlate : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][TileID.Mud] = false;
            Main.tileMerge[Type][TileID.MushroomGrass] = false;
            Main.tileMerge[TileID.Mud][Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            VanillaFallbackOnModDeletion = TileID.Stone;
            MineResist = 2f;
            HitSound = SoundID.Dig;

            DustType = DustID.Stone;

            AddMapEntry(new Color(314, 247, 271), Language.GetText("ZinklitePlate"));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 2 : 5;
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<ZinklitePlate_item>());
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.rand.NextBool(60))
            {
                Vector2 position = new Vector2(i * 16, j * 16);

                Vector2 dustPos = position + new Vector2(Main.rand.NextFloat(16), Main.rand.NextFloat(16));

                int dustIndex = Dust.NewDust(dustPos, 0, 0, DustID.WhiteTorch, 0f, 0f, 150, Color.White, 0.6f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.noLight = true;
            }
        }
    }
}
