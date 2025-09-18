using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using System;
using Terraria.GameContent.ItemDropRules;


namespace Mycology.Content.Enemies
{
    public class Zinklet : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 28;
            NPC.damage = 15;
            NPC.defense = 10;
            NPC.lifeMax = 100;
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

            if (NPC.aiStyle == 1 && NPC.velocity.Y == 0)
            {
                NPC.ai[0]--;
                if (NPC.ai[0] <= 0)
                {
                    NPC.velocity.Y = -4;
                    NPC.ai[0] = 40;

                    SoundEngine.PlaySound(new SoundStyle("Mycology/Assets/Sounds/ZinkletJump"), NPC.position);
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 12)
            {
                NPC.frameCounter = 0;
            }
            NPC.frame.Y = (int)NPC.frameCounter / 4 * frameHeight;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.ZinkliteOre>(), 1, 1, 2));
        }

        public override void OnKill()
        {
            SoundStyle myDeathSound = new SoundStyle("Mycology/Assets/Sounds/Oppsgotherscream");
            SoundEngine.PlaySound(myDeathSound, NPC.position);
        }
    }
}
