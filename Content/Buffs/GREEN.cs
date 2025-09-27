using Mycology;
using Mycology.Content.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Mycology.Content.Buffs
{
    public class GREEN : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<GreenGrapePlayer>().greenSkin = true;
        }
    }
}
