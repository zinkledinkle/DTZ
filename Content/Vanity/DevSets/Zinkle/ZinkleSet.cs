//using static DTZ.Systems.Drawing.Particles.Particle;
using DTZ.Systems.Drawing.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Vanity.DevSets.Zinkle
{
    [AutoloadEquip(EquipType.Body)]
    public class ZinkleBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            //ArmorIDs.Body.Sets.HidesArms[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesTopSkin[Item.bodySlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 10;
            Item.rare = ItemRarityID.Cyan;
            Item.value = 0;
            Item.vanity = true;
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class ZinkleHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
            ArmorIDs.Head.Sets.IsTallHat[Item.headSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;
            Item.rare = ItemRarityID.Cyan;
            Item.value = 0;
            Item.vanity = true;
        }
        public override bool IsVanitySet(int head, int body, int legs)
        {
            return Main.LocalPlayer.armor[11].type == ModContent.ItemType<ZinkleBody>()
                && Main.LocalPlayer.armor[12].type == ModContent.ItemType<ZinkleLegs>();
        }
        public override void UpdateVanitySet(Player player)
        {
            Vector2 feet = player.position + player.Size;
            feet.X -= player.width / 2f;
            if (Math.Abs(player.velocity.X) > 0 && player.velocity.Y == 0)
            {
                float speedModifier = player.velocity.X * 0.1f;
                speedModifier = MathHelper.Clamp(Math.Abs(speedModifier), 0, 1f);
                int randChance = (int)MathHelper.Lerp(20, 3, speedModifier);
                if (Main.rand.NextBool(randChance))
                {
                    Vector2 velocity = new(-Math.Sign(player.velocity.X) * 10, -5);
                    velocity += Main.rand.NextVector2Circular(1, 3);
                    velocity *= speedModifier;

                    Color color = Color.Lerp(Color.PaleVioletRed, Color.LightCyan, Main.rand.NextFloat()) * speedModifier;

                    Particle.NewParticle(Particle.ParticleID.Spark, feet, velocity, color, 0, Main.rand.NextFloat(0.5f, 0.8f));
                }
            }
            if (player.justJumped)
            {
                for (int i = 0; i < Main.rand.Next(3,5); i++)
                {
                    Dust.NewDust(feet, 1, 1, DustID.Electric, 0, -3);
                }
            }
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class ZinkleLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Legs.Sets.HidesBottomSkin[Item.legSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 16;
            Item.rare = ItemRarityID.Cyan;
            Item.value = 0;
            Item.vanity = true;
        }
    }
}
