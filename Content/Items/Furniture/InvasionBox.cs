using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Mycology.Content.Tiles;

namespace Mycology.Content.Items.Furniture
{
    public class InvasionBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Music/Invasion"), ModContent.ItemType<InvasionBox>(), ModContent.TileType<InvasionBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<InvasionBoxTile>(), 0);
        }
    }
}