using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Utilities;
using Mycology.Content;
using Terraria.GameContent.ItemDropRules;
using Mycology.Content.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mycology.Systems;
using Terraria.Localization;
using Terraria.Audio;

namespace Mycology.Content.Enemies
{
    public class Oppsgother : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 42;
            NPC.damage = 1;
            NPC.defense = 1;
            NPC.lifeMax = 2000;
            NPC.value = 50f;
            NPC.aiStyle = 1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            AIType = NPCID.GreenSlime;
            AnimationType = NPCID.GreenSlime;
        }
        public override void AI()
        {
            base.AI();
            if (Math.Abs(NPC.velocity.X) > 0)
            {
                NPC.spriteDirection = -Math.Sign(NPC.velocity.X);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20)
            {
                NPC.frameCounter = 0;
            }
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulofSpite>(), 1, 1, 2));
        }
        public override void OnKill()
        {
            SoundStyle myDeathSound = new SoundStyle("Mycology/Assets/Sounds/Oppsgotherscream");
            SoundEngine.PlaySound(myDeathSound, NPC.position);

        }
    }
}