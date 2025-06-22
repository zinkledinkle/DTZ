using DTZ.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Items
{
    public class SUPERMEGAULTRADEBUGITEM9000 : ModItem
    {
        public override string Texture => "Terraria/Images/Item_0";
        public override void SetDefaults()
        {
            Item.autoReuse = false;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = int.MaxValue;
            Item.rare = ItemRarityID.Red;
        }
        private Point start = new(0, 1);
        private Point end = new(0, 1);
        int click = 0;
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                StructureSystem.PlaceStructure("test", Main.MouseWorld.ToTileCoordinates());
                return true;
            }

            switch(click)
            {
                case 0:
                    start = Main.MouseWorld.ToTileCoordinates();
                    Main.NewText("start: " + start);
                    break;
                case 1:
                    end = Main.MouseWorld.ToTileCoordinates();
                    Main.NewText("end: " + end);
                    break;
                default:
                    int width = (int)Math.Abs(end.X - start.X) + 1;
                    int height = (int)Math.Abs(end.Y - start.Y) + 1;
                    StructureSystem.CreateStructure(new Rectangle((int)start.X, (int)start.Y, width, height), "test");
                    break;
            }
            click++;
            if (click >= 3) click = 0;

            return base.UseItem(player);
        }
    }
}
