using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Mycology.Content.Items;

namespace Mycology.Content.Tiles
{
    public class HellcapTile : ModTile
    {
        private const int frameWidth = 18;
        public static readonly int[] validTiles = 
        {
            TileID.Ash,
            TileID.AshGrass,
            TileID.Hellstone
        };
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileNoFail[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorValidTiles = validTiles;
            TileObjectData.addTile(Type);

            HitSound = SoundID.Grass;
            DustType = DustID.Torch;

            AddMapEntry(new Color(200, 55, 0), Language.GetText("Hellcap"));
        }
        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            spriteEffects = (i % 2 == 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally; //most plants alternate sprite direction, I don't remember if mushrooms do too but herbs do
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<Hellcap>(), 1);
        }
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
            tileFrameX += (short)((i % 3) * frameWidth); //idk how vanilla usually decides the tileframing but for now its just repeated horizontally based on world position
        }
    }
}
