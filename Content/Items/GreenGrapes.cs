using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Mycology.Content.Buffs;
using Mycology.Content.Enemies;

namespace Mycology.Content.Items
{
    public class GreenGrapes : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item2;
            Item.consumable = true;
            Item.maxStack = 30;
            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.buffType = ModContent.BuffType<Buffs.GREEN>();
            Item.buffTime = 60 * 10;
        }
        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                int npcType = ModContent.NPCType<Zinklet>();
                int npcIndex = NPC.NewNPC(null, (int)player.position.X, (int)player.position.Y, npcType);
            }

            return true;
        }
    }
}
