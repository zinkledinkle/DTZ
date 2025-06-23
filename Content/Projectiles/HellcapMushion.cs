
using DTZ.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Projectiles
{
    public class HellcapMushion : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.height = 32;
            Projectile.width = 32;
            Projectile.friendly = true;
            Projectile.timeLeft = 60 * 60 * 5; // 5 minutes
            Projectile.aiStyle = ProjAIStyleID.Pet;
            Projectile.tileCollide = true;

            AIType = ProjectileID.CavelingGardener;

            Projectile.ai = new float[7];
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        Player owner;

        bool chasingPlayer = false;
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);

            owner = Main.player[Projectile.owner];
            //For testing purposes. When we get these things spawning from the tiles this wont be neccessary
            if (owner == null)
            {
                owner = Main.LocalPlayer;
            }

            chasingPlayer = false;
            currentState = States.idle;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public enum States
        {
            idle,
            run,
            taunt,
            hide,
            ability
        }

        public States currentState;
        public States previousState;

        public Dictionary<States, int> animLengths = new()
        {
            {States.idle, 3},
            {States.run, 8},
            {States.taunt, 10},
            {States.hide, 7},
            {States.ability, 4},

        };

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true ;
        }

        public override void AI()
        {
            base.AI();

            owner.AddBuff(ModContent.BuffType<HellcapBuff>(), 2);

            //Main.NewText($"AI0: {Projectile.ai[0]} AI1: {Projectile.ai[3]} AI2: {Projectile.ai[4]}");
            /* Vanilla uses of projectile.AI[] || our use
             * AI 0: 0 if walking / idle. 1 if flying || Unused
             * Ai 1: Projectile velocity || unused
             * AI 2: unused || Special Rotation
             * AI 3: unused || animation timer
             * AI 4: unused || animation frame
             * AI 5: unused || ability timer
             * AI 6: unused || standing still timer
             */
            //Main.NewText(Projectile.ai[5]);


            if (previousState != currentState)
            {
                Projectile.ai[4] = 0;
                Projectile.ai[3] = 0;
                Projectile.ai[2] = 0;
            }

            if (Projectile.ai[5]++ >= 60 * 20 && currentState != States.ability)
            {
                Projectile.ai[5] = 0;
                currentState = States.ability;
                SoundEngine.PlaySound(new SoundStyle("DTZ/Assets/Sounds/HellcapMushion3") with { PitchVariance = .16f }, Projectile.Center);
            }

            if (currentState == States.ability)
            {
                Projectile.ai[0] = 0;
                Projectile.ai[1] = 0;
                Projectile.velocity *= 0;

                if (Projectile.ai[3]++ >= 8)
                {
                    Projectile.ai[4]++;
                    Projectile.ai[3] = 0;

                    if (Projectile.ai[4] >= animLengths[currentState])
                    {
                        Projectile.ai[4] = 0;
                        Projectile.ai[3] = 0;
                        currentState = States.idle;
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    if (Main.rand.NextBool(10))
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror);
                    }
                }

                Projectile.rotation = 0;
            }
            else if (Projectile.ai[0] == 0)
            {
                if (currentState == States.taunt)
                {
                    if (Projectile.ai[4] == 0 && Projectile.ai[3] == 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle("DTZ/Assets/Sounds/HellcapMushion2") with { PitchVariance = .16f }, Projectile.Center);
                    }

                    if (Projectile.ai[3]++ >= 8)
                    {
                        Projectile.ai[4]++;
                        Projectile.ai[3] = 0;

                        if (Projectile.ai[4] >= animLengths[currentState])
                        {
                            Projectile.ai[4] = 0;
                            Projectile.ai[3] = 0;
                            Projectile.ai[6]++;

                            if (Projectile.ai[6] >= 3)
                            {
                                currentState = States.idle;
                                Projectile.ai[6] = 0;
                            }
                        }
                    }
                }
                else
                {
                    if (Projectile.velocity.Length() <= 1)
                    {
                        currentState = States.idle;
                    }
                    else
                    {
                        currentState = States.run;
                    }

                    if (Projectile.ai[4] >= animLengths[currentState])
                    {
                        Projectile.ai[4] = 0;
                        Projectile.ai[3] = 0;
                    }

                    if (currentState == States.idle)
                    {
                        if (Projectile.velocity.Length() < 2)
                        {
                            Projectile.ai[6]++;
                        }
                        else
                        {
                            Projectile.ai[6] = 0;
                        }

                        if (Projectile.ai[6] > 60 * 3)
                        {
                            if (Main.rand.NextBool(50) && Projectile.ai[4] == 0)
                            {
                                currentState = States.taunt;
                                Projectile.ai[6] = 0;
                            }
                        }
                        else if (Main.rand.NextBool(200) && Projectile.ai[4] == 0)
                        {
                            Projectile.ai[4] = 1;
                            SoundEngine.PlaySound(new SoundStyle("DTZ/Assets/Sounds/IceliumMushion1") with { PitchVariance = .16f }, Projectile.Center);
                        }

                        if (Projectile.ai[4] == 1 || Projectile.ai[4] == 2)
                        {
                            if (Projectile.ai[3]++ >= 8)
                            {
                                Projectile.ai[4]++;
                                Projectile.ai[3] = 0;

                                if (Projectile.ai[4] >= animLengths[currentState])
                                {
                                    Projectile.ai[4] = 0;
                                    Projectile.ai[3] = 0;
                                }
                            }
                        }
                    }

                    if (currentState == States.run)
                    {
                        if (Projectile.ai[3]++ >= 5)
                        {
                            Projectile.ai[4]++;
                            Projectile.ai[3] = 0;

                            if (Projectile.ai[4] >= animLengths[currentState])
                            {
                                Projectile.ai[4] = 0;
                                Projectile.ai[3] = 0;
                            }
                        }
                    }
                }

                Projectile.rotation = 0;
            }
            else
            {
                currentState = States.hide;

                if (Projectile.ai[3]++ >= 5)
                {
                    Projectile.ai[4]++;
                    Projectile.ai[3] = 0;

                    if (Projectile.ai[4] >= animLengths[currentState])
                    {
                        Projectile.ai[4] = animLengths[currentState] - 1;
                        Projectile.ai[3] = 0;
                    }
                }

                Projectile.ai[2] += MathHelper.ToRadians(MathF.Sign(Projectile.velocity.X) * 10f);

                Projectile.rotation = Projectile.ai[2];
            }

            if (Projectile.ai[5] == (60 * 20) - 2)
            {
                Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HellcapMushionProj>(), 0, 0, 0, ai2: owner.whoAmI);
            }

            previousState = currentState;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var tex = TextureAssets.Projectile[Type].Value;
            var frame = new Rectangle(32 * (int)Projectile.ai[4], 32 * (int)currentState, 32, 32);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, new Vector2(16,16), Projectile.scale, Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DTZ/Assets/Sounds/MushionPerish") with { PitchVariance = .16f }, Projectile.Center);

            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
            }
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            }

            for (int i = 0; i < 3; i++)
            {
                Gore.NewGore(null, Projectile.Center, Vector2.Zero, Main.rand.Next(new int[3] { GoreID.Smoke1, GoreID.Smoke2, GoreID.Smoke3 }));
            }
        }
    }
}