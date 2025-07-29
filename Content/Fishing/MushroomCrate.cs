using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Mycology.Content.Fishing
{
    public class MushroomCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsFishingCrate[Type] = true;
            Item.ResearchUnlockCount = 10;
        }
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12; //apparently lower values 'look better' on the fishing line
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
            Item.DefaultToPlaceableTile(ModContent.TileType<MushroomCrate_Tile>());
        }
        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            int[] ChestLoot = [
                ItemID.Shroomerang
            ];
            itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, ChestLoot));
            itemLoot.Add(ItemDropRule.Common(ItemID.GlowingMushroom, 1, 15, 35));
            CrateGlobalItem.AddVanillaCrateDrops(ref itemLoot);
        }
    }
    public class MushroomCrate_Tile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;
            DustType = DustID.GlowingMushroom;
            
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(0.1f, 0.05f, 1f), name);
        }
    }
}
