using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Mycology.Content.Buffs;
using System.Diagnostics;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Mycology.Content.Projectiles
{
    public class ZinklebotPet : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.height = 44;
            Projectile.width = 32;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 4;

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
            fly,
        }

        public States currentState;
        public States previousState;

        public Dictionary<States, int> animLengths = new()
        {
            {States.idle, 4},
            {States.run, 11},
            {States.taunt, 17},
            {States.fly, 27},

        };

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override void AI()
        {
            base.AI();
            if (Main.player[Projectile.owner].HasBuff<ZinklebotPetBuff>()) Projectile.timeLeft = 2;

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

            if (Projectile.ai[0] == 0)
            {
                if (currentState == States.taunt)
                {
                    if (Projectile.ai[4] == 0 && Projectile.ai[3] == 0)
                    {

                        SoundEngine.PlaySound(new SoundStyle("Mycology/Assets/Sounds/ToadMushion2") with { PitchVariance = .16f }, Projectile.Center);
                    }

                    if (Projectile.ai[3]++ >= 6)
                    {
                        Projectile.ai[4]++;
                        Projectile.ai[3] = 0;

                        if (Projectile.ai[4] >= animLengths[currentState])
                        {
                            Projectile.ai[4] = 0;
                            Projectile.ai[3] = 0;
                            Projectile.ai[6]++;

                            if (Projectile.ai[6] >= 1)
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
                            SoundEngine.PlaySound(new SoundStyle("Mycology/Assets/Sounds/ToadMushion1") with { PitchVariance = .16f }, Projectile.Center);
                        }

                        if (Projectile.ai[4] == 1 || Projectile.ai[4] == 2 || Projectile.ai[4] == 3 || Projectile.ai[4] == 4)
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
                currentState = States.fly;

                if (Projectile.ai[3]++ >= 1)
                {
                    Projectile.ai[4]++;
                    Projectile.ai[3] = 0;

                    if (Projectile.ai[4] >= animLengths[currentState])
                    {
                        Projectile.ai[4] = 0;
                        Projectile.ai[3] = 0;
                    }
                }

                Projectile.ai[2] = MathHelper.SmoothStep(Projectile.ai[2], Math.Sign(Projectile.velocity.X), 0.15f);
                Projectile.rotation = (Projectile.ai[2] * Math.Abs(Projectile.ai[2])) * MathHelper.ToRadians(20);
                //Projectile.ai[2] += MathHelper.ToRadians(MathF.Sign(Projectile.velocity.X) * 10f);
            }

            previousState = currentState;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var tex = TextureAssets.Projectile[Type].Value;
            var frame = new Rectangle(32 * (int)Projectile.ai[4], 44 * (int)currentState, 32, 44);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, new Vector2(16, 22), Projectile.scale, Projectile.direction != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("Mycology/Assets/Sounds/MushionPerish") with { PitchVariance = .16f }, Projectile.Center);

            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
            }
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RuneWizard);
            }

            for (int i = 0; i < 3; i++)
            {
                Gore.NewGore(null, Projectile.Center, Vector2.Zero, Main.rand.Next(new int[3] { GoreID.Smoke1, GoreID.Smoke2, GoreID.Smoke3 }));
            }
        }
    }
}