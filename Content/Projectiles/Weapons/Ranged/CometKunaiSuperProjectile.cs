using System;
using System.Collections.Generic;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public class CometKunaiSuperProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                DamageClass d;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind("RogueDamageClass", out d);
                Projectile.DamageType = d;
            }
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
            Projectile.manualDirectionChange = true;
            Projectile.extraUpdates = 0;
        }

        public ref float Mode => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];
        public ref float MiscTime => ref Projectile.ai[2];

        public override void AI()
        {
            for (int i = 0; i < 4; i++) {
                Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat()) with { A = 0 };
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(18), 36, 36, DustID.SparkForLightDisc, 0, 0, 0, randomColor);
                dust.noGravity = true;
                if (Mode != 1) {
                    dust.velocity += Main.rand.NextVector2Circular(12, 12);
                }

                if (Main.rand.NextBool(50)) {
                    CalamityHunt.Particles.Add(Particle.Create<SmokeSplatterParticle>(particle => {
                        particle.position = Projectile.Center + Projectile.velocity + Main.rand.NextVector2Circular(20, 20);
                        particle.velocity = Projectile.velocity * 0.5f;
                        particle.scale = Main.rand.NextFloat(0.5f, 1f);
                        particle.color = Color.Lerp(Color.Blue, Color.RoyalBlue, 0.3f) with { A = 20 };
                        particle.maxTime = 90;
                        particle.fadeColor = Color.Blue with { A = 20 };
                    }));
                }
            }

            //Projectile.scale = MathHelper.Lerp(Projectile.scale, 1.1f, 0.1f);

            Player player = Main.player[Projectile.owner];
            bool button = ModLoader.HasMod(HUtils.CalamityMod) ? player.channel : PlayerInput.MouseInfo.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
            if (Main.myPlayer == Projectile.owner) {
                if (button && Mode == 0) {
                    Projectile.scale = 0.16f + MathF.Sqrt(Utils.GetLerpValue(0, 60, MiscTime, true)) * 0.5f;
                    player.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
                    Projectile.Center = Vector2.Lerp(Projectile.Center, player.MountedCenter + new Vector2(10 * player.direction, -30), 0.5f);
                    if (Projectile.Distance(Main.MouseWorld) > 30) {
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero), 0.1f);
                    }

                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, MathHelper.Pi - MathHelper.PiOver2 * player.direction * 0.3f + MathF.Sin(Projectile.localAI[0] * 0.22f) * 0.2f);
                    player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, MathHelper.Pi - MathHelper.PiOver2 * player.direction * 0.3f + MathF.Sin(Projectile.localAI[0] * 0.22f) * 0.2f);
                    player.SetDummyItemTime(2);
                    player.itemTime = 2;
                    player.itemAnimation = 2;

                    Projectile.timeLeft = 240;
                    MiscTime++;
                    if (MiscTime == 60) {
                        CalamityHunt.Particles.Add(Particle.Create<CrossSparkle>(particle => {
                            particle.position = Projectile.Center;
                            particle.velocity = MathHelper.PiOver4.ToRotationVector2();
                            particle.scale = 3f;
                            particle.color = new Color(50, 180, 255, 0);
                            particle.anchor = () => Projectile.velocity * 0.2f;
                        }));
                        CalamityHunt.Particles.Add(Particle.Create<CrossSparkle>(particle => {
                            particle.position = Projectile.Center;
                            particle.velocity = Vector2.Zero;
                            particle.scale = 2f;
                            particle.color = new Color(50, 180, 255, 0);
                            particle.anchor = () => Projectile.velocity * 0.2f;
                        }));

                        //sound
                    }

                    Projectile.netUpdate = true;
                }
                else if (Mode == 0) {
                    if (MiscTime < 60) {
                        Projectile.Kill();
                    }

                    Mode = 2;
                    MiscTime = 0;
                    Projectile.netUpdate = true;
                }
                else if (Mode == 2) {
                    Projectile.extraUpdates = 4;
                    Projectile.velocity *= 1.12f;
                    //Projectile.Center = Vector2.Lerp(Projectile.Center, player.MountedCenter + new Vector2(-1 * player.direction, -50), 0.5f);

                    player.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);

                    float throwDown = MathF.Pow(MiscTime / 15f, 3f);
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi - (MathHelper.PiOver2 - throwDown * 4f) * player.direction * 0.3f + MathF.Sin(Projectile.localAI[0] * 0.22f) * 0.2f * throwDown);

                    if (MiscTime++ >= 15) {
                        Mode = 1;
                        MiscTime = 0;
                        Projectile.netUpdate = true;
                    }
                }

                if (Mode == -1 && MiscTime++ % 4 == 0) {
                    Vector2 position = Projectile.Center - new Vector2(0, 400) + Main.rand.NextVector2Circular(500, 300);
                    Vector2 velocity = position.DirectionTo(Projectile.Center).RotatedByRandom(0.12f).SafeNormalize(Vector2.Zero) * position.Distance(Projectile.Center) * 0.024f;
                    velocity.X *= 0.9f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, ModContent.ProjectileType<CometKunaiStarfall>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                }

                if (Target > -1) {
                    NPC target = Main.npc[(int)Target];
                    {
                        if (target.active) {
                            Projectile.Center += (target.position - target.oldPosition) / (Projectile.extraUpdates + 1);
                            Projectile.netUpdate = true;
                        }
                        else {
                            Projectile.Kill();
                        }
                    }
                }
            }

            if (Projectile.velocity.X != 0) {
                Projectile.direction = Math.Sign(Projectile.velocity.X);
            }
            if (Mode != -1) {
                Projectile.rotation += Projectile.direction * 0.2f;
            }

            if (Projectile.frameCounter++ > 1) {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 3;
            }

            Projectile.localAI[0]++;
        }

        private void SetCollided(bool stick)
        {
            Projectile.extraUpdates = 1;
            Mode = -1;
            Projectile.localAI[1] = 1f;
            Projectile.timeLeft = stick ? 180 : 30;

            for (int i = 0; i < 3; i++) {
                CalamityHunt.Particles.Add(Particle.Create<SmokeSplatterParticle>(particle => {
                    particle.position = Projectile.Center + Projectile.velocity + Main.rand.NextVector2Circular(20, 20);
                    particle.velocity = Vector2.Zero;
                    particle.scale = Main.rand.NextFloat(2f, 4f);
                    particle.color = Color.Lerp(Color.Blue, Color.RoyalBlue, 0.3f) with { A = 20 };
                    particle.maxTime = 90;
                    particle.fadeColor = Color.Blue with { A = 20 };
                    particle.anchor = () => Projectile.velocity;
                }));
            }

            if (stick && Main.myPlayer == Projectile.owner) {
                Projectile.netUpdate = true;
                Projectile.velocity = Vector2.Zero;
                Projectile.tileCollide = false;
                SoundStyle attachSound = AssetDirectory.Sounds.GoozmaMinions.StellarConstellationForm with { MaxInstances = 0, Pitch = 0.8f, PitchVariance = 0.1f, Volume = 0.4f };
                SoundEngine.PlaySound(attachSound, Projectile.Center);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Mode == 1) {
                SetCollided(true);
                Projectile.Center += oldVelocity;
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Mode > 0) {
                Target = target.whoAmI;
                SetCollided(true);
                Projectile.Center += Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
            }
        }

        public override bool? CanDamage() => Mode > 0;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Target < 0) {
                behindNPCsAndTiles.Add(index);
            }
            else {
                Projectile.hide = false;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 18; i++) {
                Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat());
                randomColor.A = 0;
                Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(5), 10, 10, DustID.RainbowRod, newColor: randomColor);
                d.noGravity = true;
                d.velocity += Main.rand.NextVector2Circular(14, 14);
            }

            for (int i = 0; i < 6; i++) {
                Color randomColor = Color.Lerp(Color.Goldenrod, Color.Gold, Main.rand.NextFloat());
                randomColor.A = 0;
                Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(5), 10, 10, DustID.RainbowRod, newColor: randomColor);
                d.noGravity = true;
                d.velocity += Main.rand.NextVector2Circular(6, 6);
            }

            if (Mode != 0) {
                for (int i = 0; i < 5; i++) {
                    Vector2 velocity = new Vector2(0, -10).RotatedBy(Projectile.rotation + MathHelper.TwoPi / 5f * i);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<CometKunaiGhostProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Target);
                }
            }

            SoundStyle killSound = SoundID.DD2_ExplosiveTrapExplode with { MaxInstances = 0, Pitch = -1 };
            SoundEngine.PlaySound(killSound, Projectile.Center);
        }

        public static Texture2D fireTexture;

        public override void Load()
        {
            fireTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Fire").Value;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = AssetDirectory.Textures.Glow[0].Value;
            Rectangle fireFrame = fireTexture.Frame(1, 3, 0, Projectile.frame);

            if (Mode == 0) {
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.RoyalBlue, Projectile.rotation, texture.Size() * new Vector2(0.5f, 0.55f), Projectile.scale * Utils.GetLerpValue(0, 60, MiscTime, true) * 2, 0, 0);
            }
            else {
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.RoyalBlue, Projectile.rotation, texture.Size() * new Vector2(0.5f, 0.55f), Projectile.scale * 2, 0, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(0, 70, 100, 0), Projectile.rotation, texture.Size() * new Vector2(0.5f, 0.55f), Projectile.scale * 1.3f, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White, Projectile.rotation, texture.Size() * new Vector2(0.5f, 0.55f), Projectile.scale, 0, 0);

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), new Color(5, 10, 60, 0), Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 1.2f, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), new Color(0, 5, 30, 0), Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 2f, 0, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(0, 20, 200, 20), Projectile.rotation * -0.3f, texture.Size() * new Vector2(0.5f, 0.55f), Projectile.scale * 1.5f, 0, 0);

            if (Mode == 2) {
                float progress = Utils.GetLerpValue(5, 30, Projectile.localAI[0], true);
                double transparency = Utils.Lerp(0, 20, 1);
                Main.EntitySpriteDraw(fireTexture, Projectile.Center - Main.screenPosition, fireFrame, Color.Black * progress * 0.1f, Projectile.velocity.ToRotation() - MathHelper.PiOver2, fireFrame.Size() * new Vector2(0.5f, 0.75f), Projectile.scale * 1.2f, 0, 0);
                Main.EntitySpriteDraw(fireTexture, Projectile.Center - Main.screenPosition, fireFrame, new Color(0, 20, 200, 20) * progress, Projectile.velocity.ToRotation() - MathHelper.PiOver2, fireFrame.Size() * new Vector2(0.5f, 0.75f), Projectile.scale * 1.3f, 0, 0);
            }

            return false;
        }
    }
}
