using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DTZ.Content.Fishing
{
    public class MycologyFishingPlayer : ModPlayer
    {
        private bool MycoRod = false;
        public override void ResetEffects()
        {
            MycoRod = Player.HeldItem.type == ModContent.ItemType<MycoRod>();
        }
        public override void ModifyFishingAttempt(ref FishingAttempt attempt)
        {
            if (MycoRod && !attempt.crate && Main.rand.NextBool(4))
            {
                attempt.crate = true;
            }
        }
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool water = !attempt.inHoney && !attempt.inLava;
            if (water && attempt.crate)
            {
                if (attempt.rare && !attempt.veryrare && !attempt.legendary) //dont override golden crates
                {
                    if (Player.ZoneGlowshroom) itemDrop = !Main.hardMode ?
                            ModContent.ItemType<MushroomCrate>() :
                            ModContent.ItemType<MushroomCrate>(); //add hardmode crate later
                }
            }
        }
        public override bool? CanConsumeBait(Item bait)
        {
            if (MycoRod) return Main.rand.NextBool(bait.bait);
            return base.CanConsumeBait(bait);
        }
    }
}
