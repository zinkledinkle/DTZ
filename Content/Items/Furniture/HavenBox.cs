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
    public class HavenBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Music/Haven"), ModContent.ItemType<HavenBox>(), ModContent.TileType<HavenBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<HavenBoxTile>(), 0);
        }
    }
}