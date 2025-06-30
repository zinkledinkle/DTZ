using DTZ.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Items
{
    public class IceshroomItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 5);
            Item.maxStack = 9999;

            Item.ammo = ItemID.Mushroom;
            Item.damage = 7;
            Item.knockBack = 4f;
            Item.DamageType = DamageClass.Ranged;
            Item.shootSpeed = 6;
            Item.shoot = ModContent.ProjectileType<IceliumShot>();
        }
    }
    public class IceliumShot : MushroomShotBase
    {
        protected override float Weight => 0.25f;
        public override string Texture => ModContent.GetInstance<IceshroomItem>().Texture;
        protected override float RotationSpeed => 0.25f;
        protected override int DustType => DustID.IceTorch;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.GetGlobalNPC<IceliumShotGlobalNPC>().Hits.Add(new IceliumShotGlobalNPC.IceHit() { id = Projectile.whoAmI, time = 45, hitInfo = hit, player = Main.player[Projectile.owner] });
        }
        private class Frozen : ModBuff
        {
            public override string Texture => "Terraria/Images/Buff_312";
            public override void Update(NPC npc, ref int buffIndex)
            {
                if (!npc.boss)
                {
                    npc.position.X -= npc.velocity.X / 2;
                }
            }
        }
        private class IceliumShotGlobalNPC : GlobalNPC
        {
            public override bool InstancePerEntity => true;
            public bool IsFrozen = false;
            public List<IceHit> Hits = [];
            public class IceHit
            {
                public int id;
                public int time;
                public NPC.HitInfo hitInfo;
                public Player player;
            }
            public override void ResetEffects(NPC npc)
            {
                for (int i = 0; i < Hits.Count; i++)
                {
                    Hits[i].time -= 1;
                    IceHit iceHit = Hits[i];
                    npc.AddBuff(ModContent.BuffType<Frozen>(), iceHit.time);

                    if (iceHit.time == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item27, npc.Center);
                        int dmg = npc.StrikeNPC(iceHit.hitInfo);
                        foreach (NPC n in Main.npc.Where(n => n.Distance(npc.Center) < 75 && n != npc && !npc.friendly)) dmg += n.StrikeNPC(iceHit.hitInfo); //very messy sorry
                        for (int d = 0; d < Main.rand.Next(5, 10); d++)
                            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Ice, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-4, 4));
                        iceHit.player.addDPS(dmg);
                    }
                }
                Hits.RemoveAll(h => h.time <= 0);
                IsFrozen = Hits.Any(h => h.time > 0);

                if (IsFrozen && Main.rand.NextBool(2))
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.IceTorch);
                }
            }
            /*private Effect Tint;
            private Texture2D Ice;
            public override void Load()
            {
                if (!Main.dedServ)
                {
                    Tint = ModContent.Request<Effect>("DTZ/Assets/Effects/Basic/Tint", AssetRequestMode.ImmediateLoad).Value;
                    Ice = ModContent.Request<Texture2D>("DTZ/Assets/Textures/IceNoise").Value;
                }
            }*/
            public override void DrawEffects(NPC npc, ref Color drawColor)
            {
                if (IsFrozen)
                {
                    drawColor = drawColor.MultiplyRGB(Color.LightBlue).MultiplyRGB(Color.LightBlue);
                }
            }
        }
    }
}
