using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Mycology.Content.Items;
using Terraria.Localization;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using Mycology.Systems;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Mycology.Content.Items.Accessories;

namespace Mycology.Content.NPCs
{
    public class LoneMushion : ModNPC
    {
        private static Profiles.StackedNPCProfile NPCProfile;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.DoesntDespawnToInactivityAndCountsNPCSlots[Type] = true;
            NPCID.Sets.SpawnsWithCustomName[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 46;
            NPC.height = 48;
            NPC.lifeMax = 50;
            NPC.damage = 0;
            NPC.dontTakeDamage = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.noTileCollide = false;
            NPCProfile = new Profiles.StackedNPCProfile(new Profiles.DefaultNPCProfile(Texture, -1));
        }
        int frameCounter = 0;
        bool doAnimation = false;
        int animCooldown = 0;
        public override void AI()
        {
            NPC.velocity.X = 0;
            Vector2 spawnPos = HavenSystem.SpawnPositionForLoneMushion.ToWorldCoordinates();
            if (NPC.position.Distance(spawnPos) > 500)
            {
                NPC.position = spawnPos;
            }

            animCooldown = Math.Max(0, animCooldown - 1);
            if (Main.rand.NextBool(200) && !doAnimation && animCooldown == 0)
            {
                doAnimation = true;
                animType = Main.rand.Next(0, 2);
            }

            if (doAnimation)
            {
                frameCounter++;
                if (frameCounter == 5)
                {
                    frameCounter = 0;
                    frame++;
                    if ((frame > 2 && animType == 0) || (frame > 7 && animType == 1))
                    {
                        frame = 0;
                        doAnimation = false;
                        animCooldown = 300;
                    }
                }
            }
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override void AddShops()
        {
            NPCShop shop = new(Type);
            shop.Add(ItemID.GlowingMushroom);
            shop.Add(ModContent.ItemType<Shiitake>());
            shop.Add(ModContent.ItemType<MushionSeeds>());
            shop.Add(ModContent.ItemType<GlowingSalve>());
            shop.Add(ModContent.ItemType<MoonlordUnderworldBunkFungus>());
            shop.Register();
        }
        public override bool CanChat() => true;
        public override ITownNPCProfile TownNPCProfile()
        {
            return NPCProfile;
        }
        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "Myco",
                "Psilo",
                "Boletus",
                "Clavar",
                "Flaelo"
            };
        }
        int chatIndex = 0;
        private static readonly LocalizedText[] chats = new[]
        {
            Language.GetOrRegister("Mods.Mycology.LoneMushionSpecialDialogue1", () =>
            "If you happen to find any mushion seeds, till some mud and plant them."),
            Language.GetOrRegister("Mods.Mycology.LoneMushionSpecialDialogue2", () =>
            "Glowing mushrooms make tools too, you know?"),
            Language.GetOrRegister("Mods.Mycology.LoneMushionSpecialDialogue3", () =>
            "If you grow any mushions, keep it away from light. They're quite fragile when developing."),
            Language.GetOrRegister("Mods.Mycology.LoneMushionSpecialDialogue4", () =>
            "Mushion seeds only grow in groups of three or more, we don't grow lonely."),
            Language.GetOrRegister("Mods.Mycology.LoneMushionSpecialDialogue5", () =>
            "We're not a glowing mushroom species. Actually, we come in ice, fire, and... toad variants."),
        };
        public override string GetChat()
        {
            chatIndex = 0;
            if (currentChatMessage != null)
            {
                string result = currentChatMessage;
                currentChatMessage = null;
                return result;
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            chat.Add(Language.GetOrRegister("Mods.Mycology.LoneMushionDialogue1", () => "If you come across a walking Truffle, tell him to stop composting my babies.").Value);
            chat.Add(Language.GetOrRegister("Mods.Mycology.LoneMushionDialogue2", () => "Just because we decompose doesn't give you reason to compost us!").Value);
            chat.Add(Language.GetOrRegister("Mods.Mycology.LoneMushionDialogue3", () => "We call this spot a haven. Oh, that? That's a little lab of mine.").Value);
            chat.Add(Language.GetOrRegister("Mods.Mycology.LoneMushionDialogue4", () => "Shoes off, please.").Value);
            chat.Add(Language.GetOrRegister("Mods.Mycology.LoneMushionDialogue5", () => "We're white on blue, not blue on white. We're foreign to glowing mushrooms.").Value);
            chat.Add(Language.GetOrRegister("Mods.Mycology.LoneMushionDialogue6", () => "I've seen The Goonies, don't ask.").Value);
            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = Language.GetOrRegister("Mods.Mycology.LoneMushionChatButton1", () => "Chat").Value;
        }
        private string currentChatMessage = null;
        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (!firstButton)
            {
                currentChatMessage = chats[chatIndex].Value;
                chatIndex = (chatIndex + 1) % chats.Length;
                Main.npcChatText = currentChatMessage;
            } else
            {
                shop = "Shop";
            }
        }

        private int animType = 0;
        private int frame = 0;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int width = 46;
            int height = 48;

            int frameX = animType == 0 ? 0 : width;

            Rectangle rect = new Rectangle(frameX, height * frame, width, height);
            Texture2D tex = TextureAssets.Npc[Type].Value;
            NPC.frame = rect;
            spriteBatch.Draw(tex, NPC.Center - screenPos + new Vector2(0,2), rect, drawColor, 0, new Vector2(22, 23), 1, SpriteEffects.None, 1f);

            return false;
        }
        private class SpawnSystem : ModSystem
        {
            public override void PreUpdateWorld()
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<LoneMushion>()))
                {
                    Vector2 spawn = HavenSystem.SpawnPositionForLoneMushion.ToWorldCoordinates();
                    List<Player> closePlayers = Main.player.Where(p => p.Distance(spawn) < 2000 && p.active).ToList();
                    if (closePlayers.Count > 0) return;
                    NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)spawn.X, (int)spawn.Y, ModContent.NPCType<LoneMushion>());
                }
            }
        }
    }
}
