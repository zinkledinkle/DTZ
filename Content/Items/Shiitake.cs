using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Items
{
	public class Shiitake : ModItem
	{
		public override void SetStaticDefaults() {
  
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

			Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

			ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
				new Color(249, 230, 136),
				new Color(152, 93, 95),
				new Color(174, 192, 192)
			};

			ItemID.Sets.IsFood[Type] = true;
		}

		public override void SetDefaults() {

			Item.DefaultToFood(22, 22, BuffID.WellFed3, 12000);
            Item.value = Item.buyPrice(silver: 60);
            Item.rare = ItemRarityID.Blue;
		}

		public override void OnConsumeItem(Player player) {
			player.AddBuff(BuffID.WellFed, 12000);
		}
	}
}
