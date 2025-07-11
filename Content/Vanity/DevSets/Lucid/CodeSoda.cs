using DTZ.Content.Vanity.DevSets.Zinkle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Vanity.DevSets.Lucid
{
    public class CodeSoda : ModItem
    {
        public override void Load()
        {
            if (!Main.dedServ)
            {
                EquipLoader.AddEquipTexture(Mod, "DTZ/Content/Vanity/DevSets/Lucid/CodeSoda_Head", EquipType.Head, this);
                EquipLoader.AddEquipTexture(Mod, "DTZ/Content/Vanity/DevSets/Lucid/CodeSoda_Body", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, "DTZ/Content/Vanity/DevSets/Lucid/CodeSoda_Legs", EquipType.Legs, this);
            }
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Cyan;
            Item.vanity = true;
            Item.accessory = true;
            Item.width = 22;
            Item.height = 26;
        }
        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<CodeSodaPlayer>().On = true;
        }
        private class CodeSodaPlayer : ModPlayer
        {
            public bool On = false;
            public override void ResetEffects()
            {
                On = false;
            }
            public override void FrameEffects()
            {
                if (!On) return;
                Player.head = EquipLoader.GetEquipSlot(Mod, nameof(CodeSoda), EquipType.Head);
                Player.body = EquipLoader.GetEquipSlot(Mod, nameof(CodeSoda), EquipType.Body);
                Player.legs = EquipLoader.GetEquipSlot(Mod, nameof(CodeSoda), EquipType.Legs);
            }
        }
    }
}
