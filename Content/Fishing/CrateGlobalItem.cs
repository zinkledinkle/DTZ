using Mycology.Content.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Mycology.Content.Fishing
{
    public class CrateGlobalItem : GlobalItem
    {
        public static void AddVanillaCrateDrops(ref ItemLoot itemLoot)
        {
            IItemDropRule[] ores = [
                ItemDropRule.Common(ItemID.CopperOre, 1, 20, 35),
                ItemDropRule.Common(ItemID.TinOre, 1, 20, 35),
                ItemDropRule.Common(ItemID.IronOre, 1, 20, 35),
                ItemDropRule.Common(ItemID.LeadOre, 1, 20, 35),
                ItemDropRule.Common(ItemID.SilverOre, 1, 20, 35),
                ItemDropRule.Common(ItemID.TungstenOre, 1, 20, 35),
                ItemDropRule.Common(ItemID.GoldOre, 1, 20, 35),
                ItemDropRule.Common(ItemID.PlatinumOre, 1, 20, 35),
            ];
            IItemDropRule[] bars = [
                ItemDropRule.Common(ItemID.IronBar, 1, 6, 16),
                ItemDropRule.Common(ItemID.LeadBar, 1, 6, 16),
                ItemDropRule.Common(ItemID.SilverBar, 1, 6, 16),
                ItemDropRule.Common(ItemID.TungstenBar, 1, 6, 16),
                ItemDropRule.Common(ItemID.GoldBar, 1, 6, 16),
                ItemDropRule.Common(ItemID.PlatinumBar, 1, 6, 16),
            ];
            IItemDropRule[] potions = [
                ItemDropRule.Common(ItemID.ObsidianSkinPotion, 1, 2, 4),
                ItemDropRule.Common(ItemID.SpelunkerPotion, 1, 2, 4),
                ItemDropRule.Common(ItemID.HunterPotion, 1, 2, 4),
                ItemDropRule.Common(ItemID.GravitationPotion, 1, 2, 4),
                ItemDropRule.Common(ItemID.MiningPotion, 1, 2, 4),
                ItemDropRule.Common(ItemID.HeartreachPotion, 1, 2, 4),
            ];
            IItemDropRule[] healPotions = [
                ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 17),
                ItemDropRule.Common(ItemID.ManaPotion, 1, 5, 17),
            ];
            IItemDropRule[] bait = [
                ItemDropRule.Common(ItemID.JourneymanBait, 1, 2, 6),
                ItemDropRule.Common(ItemID.MasterBait, 1, 2, 6),
            ];
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 4, 5, 12));
            itemLoot.Add(new OneFromRulesRule(7, ores));
            itemLoot.Add(new OneFromRulesRule(4, bars));
            itemLoot.Add(new OneFromRulesRule(4, potions));
            itemLoot.Add(new OneFromRulesRule(2, healPotions));
            itemLoot.Add(new OneFromRulesRule(2, bait));
        }
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type is ItemID.CrimsonFishingCrate or ItemID.CrimsonFishingCrateHard) itemLoot.Add(ItemDropRule.Common(ItemID.ViciousMushroom, 4, 2, 4));
            if (item.type is ItemID.CorruptFishingCrate or ItemID.CorruptFishingCrateHard) itemLoot.Add(ItemDropRule.Common(ItemID.VileMushroom, 4, 2, 4));
            if (item.type is ItemID.FrozenCrate or ItemID.FrozenCrateHard) itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Icelium>(), 4, 2, 4));
            if (item.type is ItemID.LavaCrate or ItemID.LavaCrateHard) itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Hellcap>(), 4, 2, 4));
            if (item.type is ItemID.GoldenCrate or ItemID.GoldenCrateHard) itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Toadstool>(), 4, 2, 4));
        }
    }
}
