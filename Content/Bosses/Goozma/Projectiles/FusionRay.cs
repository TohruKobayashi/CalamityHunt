using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.Particles;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Arch.Core.Extensions;
using CalamityHunt.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class FusionRay : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 5000;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Mode => ref Projectile.ai[1];
        public ref float Owner => ref Projectile.ai[2];

        private static readonly int ChargeTime = 300;
        private static readonly int LaserDuration = 800;

        public float targetRotation;
        public float targetSize;
        public float realSize;

        public override void AI()
        {
            if (Owner < 0)
            {
                Projectile.active = false;
                return;
            }
            else if (!Main.npc[(int)Owner].active || Main.npc[(int)Owner].type != ModContent.NPCType<Goozma>())
            {
                Projectile.active = false;
                return;
            }

            Main.npc[(int)Owner].direction = Math.Sign(Projectile.rotation.ToRotationVector2().X);
            Projectile.Center = Main.npc[(int)Owner].Center + new Vector2(13 * Main.npc[(int)Owner].direction, -20f);
            (Main.npc[(int)Owner].ModNPC as Goozma).drawVelocity = Projectile.rotation.ToRotationVector2() * 12f;
            Main.npc[(int)Owner].velocity += Projectile.rotation.ToRotationVector2() * -0.1f;
            Main.npc[(int)Owner].velocity *= 0.98f;

            switch (Mode)
            {
                default:

                    if (Time % 4 == 0 && Time > ChargeTime && Time < ChargeTime + LaserDuration)
                    {
                        float shakeStrength = Utils.GetLerpValue(ChargeTime - LaserDuration * 0.5f, ChargeTime + LaserDuration, Time, true);
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2CircularEdge(1, 1), shakeStrength * 8f, 12, 20, 5000));
                    }

                    //float firstWave = MathHelper.SmoothStep(-1.5f, 0.6f, Utils.GetLerpValue(ChargeTime, ChargeTime + LaserDuration * 0.3f, Time, true));
                    //float secondWave = MathHelper.SmoothStep(0f, -0.7f, Utils.GetLerpValue(ChargeTime + LaserDuration * 0.3f, ChargeTime + LaserDuration * 0.8f, Time, true)) - MathHelper.SmoothStep(0, MathHelper.TwoPi, Utils.GetLerpValue(ChargeTime + LaserDuration * 0.3f, ChargeTime + LaserDuration * 0.8f, Time, true));
                    //float thirdWave = MathHelper.SmoothStep(1f, 0f, Utils.GetLerpValue(ChargeTime, ChargeTime + LaserDuration * 0.3f, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration, ChargeTime + LaserDuration * 0.9f, Time, true));

                    //float totalOffRot = (firstWave + secondWave) * thirdWave * Projectile.direction + (float)Math.Sin(Time * 0.1f) * 0.03f * Utils.GetLerpValue(ChargeTime - 10, ChargeTime + 10, Time, true);
                    float totalOffRot = (float)Math.Cos((Time - ChargeTime) / LaserDuration * MathHelper.TwoPi) * 2.5f * Utils.GetLerpValue(ChargeTime * 0.8f, ChargeTime + LaserDuration * 0.1f, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration, ChargeTime + LaserDuration * 0.9f, Time, true);
                    if (Time < 3)
                        Projectile.direction = Main.rand.NextBool() ? -1 : 1;

                    targetRotation = targetRotation.AngleTowards(Projectile.AngleTo(Main.npc[(int)Owner].GetTargetData().Center), Utils.GetLerpValue(ChargeTime + LaserDuration * 0.1f, ChargeTime, Time, true) * 0.04f + Utils.GetLerpValue(ChargeTime + LaserDuration * 0.3f, ChargeTime + LaserDuration * 0.6f, Time, true) * 0.01f);
                    targetSize = (float)Math.Pow(Utils.GetLerpValue(ChargeTime - 20, ChargeTime + 40, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration, Time, true), 3f);

                    float waveChange = MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(ChargeTime * 0.8f, ChargeTime + 20, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration * 0.9f, ChargeTime + LaserDuration * 0.5f, Time, true));
                    Projectile.rotation = targetRotation + totalOffRot * Projectile.direction * waveChange;

                    if ((Main.npc[(int)Owner].ai[2] == 3 || Main.npc[(int)Owner].ai[2] == -2) && Time < ChargeTime + LaserDuration - 2)
                    {
                        targetSize = 0;
                        Time = ChargeTime + LaserDuration - 2;
                        Mode = -1;
                    }

                    break;

                case 1:

                    if (Time % 4 == 0 && Time > ChargeTime && Time < ChargeTime + LaserDuration)
                    {
                        float shakeStrength = Utils.GetLerpValue(ChargeTime - LaserDuration * 0.5f, ChargeTime + LaserDuration, Time, true);
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2CircularEdge(1, 1), shakeStrength * 8f, 12, 20, 5000));
                    }
                    
                    Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.AngleTo(Main.npc[(int)Owner].GetTargetData().Center), 0.03f);
                    targetSize = (float)Math.Pow(Utils.GetLerpValue(ChargeTime - 20, ChargeTime + 40, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration, Time, true), 3f);

                    if (Time > ChargeTime + LaserDuration - 4 && Time < ChargeTime + LaserDuration - 2)
                        Time = ChargeTime + LaserDuration - 10;

                    if (Main.npc[(int)Owner].ai[2] == 3 && Time < ChargeTime + LaserDuration - 2)
                    {
                        targetSize = 0;
                        Time = ChargeTime + LaserDuration - 2;
                        Mode = -1;
                    }

                    break;

                case -1:

                    //shrug, do nothing

                    break;
            }

            realSize = MathHelper.Lerp(realSize, targetSize, 0.1f);

            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.localAI[0] = Main.npc[(int)Owner].localAI[0];

            if (Time > ChargeTime * 0.7f)
            {
                float smokePower = Utils.GetLerpValue(ChargeTime * 0.7f, ChargeTime, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 70, ChargeTime + LaserDuration + 40, Time, true);
                Color smokeColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]) * (0.5f + smokePower * 0.5f);
                smokeColor.A = 0;
                int smokeCount = (int)(smokePower * 12f);
                for (int i = 0; i < smokeCount; i++)
                    ParticleBehavior.NewParticle(ModContent.GetInstance<CosmicSmokeParticleBehavior>(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 20, Projectile.rotation.ToRotationVector2().RotatedByRandom((float)Math.Sqrt(1f - smokePower * 0.5f)) * Main.rand.NextFloat(15f, 25f) * smokePower, smokeColor, 1f + Main.rand.NextFloat());
            }

            if (!Main.rand.NextBool((int)(Time * 0.5f + 15)))
            {
                var hue = ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Projectile.Center, Projectile.rotation.ToRotationVector2().RotatedByRandom(1f) * Main.rand.NextFloat(5f, 15f), Color.White, 2f);
                hue.Add(new ParticleData<float> { Value = Projectile.localAI[0] });
            }
            if (Time > ChargeTime - 15 && Time < ChargeTime + LaserDuration + 60)
            {
                for (int i = 0; i < 15; i++)
                {
                    float grow = Utils.GetLerpValue(ChargeTime - 15, ChargeTime + 40, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 50, ChargeTime + LaserDuration, Time, true);
                    float progress = Main.rand.NextFloat(3300);
                    Color color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] - (progress / 3500f) * 100) * grow;
                    color.A = 0;
                    Vector2 position = Projectile.Center + new Vector2(progress, Main.rand.NextFloat(-80f, 80f) * (progress / 3300f)).RotatedBy(Projectile.rotation);
                    var smoke = ParticleBehavior.NewParticle(ModContent.GetInstance<CosmicSmokeParticleBehavior>(), position, Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(15f, 45f), color, 1f + Main.rand.NextFloat(2f));
                    smoke.Add(new ParticleDrawBehindEntities());
                    if (Main.rand.NextBool(5))
                    {
                        var hue = ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), position, Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(15f, 45f), Color.White, 4f * grow);
                        hue.Add(new ParticleData<float> { Value = Projectile.localAI[0] - (progress / 3300f) * 60 });
                    }
                }
            }

            if (Time == ChargeTime + LaserDuration + 20)
                SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Sizzle, Projectile.Center);

            Time++;
            Projectile.localAI[1]++;
            Projectile.timeLeft = 10;

            HandleSound();

            if (Time > ChargeTime + LaserDuration + 100)
                Projectile.Kill();

            foreach (Player player in Main.player.Where(n => n.active && !n.dead))
            {
                if (ModLoader.HasMod("CalamityMod"))
                {
                    ModLoader.GetMod("CalamityMod").Call("ToggleInfiniteFlight", player, true);
                }
                else
                {
                    player.wingTime = player.wingTimeMax;
                }
            }
        }

        public LoopingSound raySound;
        public float volume;
        public float pitch;

        public void HandleSound()
        {
            volume = 1.3f * Utils.GetLerpValue(0, 0.05f, realSize, true);// * Math.Clamp(1f + Projectile.velocity.Length() * 0.0001f + Main.LocalPlayer.Distance(Projectile.Center) * 0.0005f, 0, 1) * Projectile.scale * Utils.GetLerpValue(ChargeTime - 30, ChargeTime + 20, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration + 30, Time, true);
            pitch = (MathHelper.SmoothStep(0.1f, 1f, Utils.GetLerpValue(ChargeTime - 45, ChargeTime + 50, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration, Time, true)) + Utils.GetLerpValue(ChargeTime, ChargeTime + LaserDuration, Time, true) * 0.5f * realSize) - 1f;

            if (raySound == null)
                raySound = new LoopingSound(AssetDirectory.Sounds.Goozma.FusionRayLoop, new ProjectileAudioTracker(Projectile).IsActiveAndInGame);

            raySound.Update(() => Projectile.Center, () => volume, () => pitch);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > ChargeTime + 20 && Time < ChargeTime + LaserDuration + 10)
            {
                float angle = 0.24f;
                float grow = (float)Math.Pow(Utils.GetLerpValue(ChargeTime - 20, ChargeTime + 40, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration, Time, true), 3f);

                //Dust.QuickDustLine(Projectile.Center, Projectile.Center + new Vector2(3500f * (0.5f + grow * 0.5f), 0).RotatedBy(Projectile.rotation + angle), 200, Color.Blue);
                //Dust.QuickDustLine(Projectile.Center, Projectile.Center + new Vector2(3500f * (0.5f + grow * 0.5f), 0).RotatedBy(Projectile.rotation - angle), 200, Color.Blue);
                //if (targetHitbox.IntersectsConeFastInaccurate(Projectile.Center, 3500f * (0.5f + grow * 0.5f), Projectile.rotation, angle * grow))
                //    Dust.NewDustPerfect(targetHitbox.Center(), DustID.TintableDust, Vector2.Zero, 0, Color.Black, 2f).noGravity = true;

                return targetHitbox.IntersectsConeFastInaccurate(Projectile.Center, 3500f * (0.5f + grow * 0.5f), Projectile.rotation, angle * grow);
            }
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<FusionBurn>(), 180);
        }

        public override void Load()
        {
            On_Main.UpdateAudio += QuietMusic;
        }

        private void QuietMusic(On_Main.orig_UpdateAudio orig, Main self)
        {
            orig(self);

            if (Main.projectile.Any(n => n.active && n.type == ModContent.ProjectileType<FusionRay>()))
            {
                Projectile projectile = Main.projectile.FirstOrDefault(n => n.active && n.type == ModContent.ProjectileType<FusionRay>());

                for (int i = 0; i < Main.musicFade.Length; i++)
                {
                    float grow = Utils.GetLerpValue(ChargeTime - 15, ChargeTime + 40, projectile.ai[0], true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 50, ChargeTime + LaserDuration, projectile.ai[0], true);

                    float volume = Main.musicFade[i] * Main.musicVolume * (1f - grow * 0.33f);
                    float tempFade = Main.musicFade[i];
                    Main.audioSystem.UpdateCommonTrackTowardStopping(i, volume, ref tempFade, Main.musicFade[i] > 0.15f);
                    Main.musicFade[i] = tempFade;
                }
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = AssetDirectory.Textures.Extras.FusionRay[0].Value;
            Texture2D textureBits = AssetDirectory.Textures.Extras.FusionRay[1].Value;
            Texture2D textureGlow = AssetDirectory.Textures.Extras.FusionRay[2].Value;
            Texture2D textureSecond = AssetDirectory.Textures.Extras.FusionRay[3].Value;
            Texture2D glow = AssetDirectory.Textures.Glow.Value;
            Texture2D ray = AssetDirectory.Textures.GlowRay.Value;

            Color startColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            startColor.A = 0;

            float laserCharge = Utils.GetLerpValue(50, ChargeTime, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 50, ChargeTime + LaserDuration + 20, Time, true);
            float laserRayCharge = Utils.GetLerpValue(0, ChargeTime, Projectile.localAI[1], true) * Utils.GetLerpValue(ChargeTime + 10, ChargeTime - 30, Time, true);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, startColor, 0, glow.Size() * 0.5f, 2f * laserCharge, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), new Color(255, 255, 255, 0) * laserCharge, 0, glow.Size() * 0.5f, 1.5f * laserCharge, 0, 0);
            Main.EntitySpriteDraw(ray, Projectile.Center - Main.screenPosition, ray.Frame(), startColor * laserRayCharge * 0.5f, Projectile.rotation, ray.Size() * new Vector2(0.02f, 0.5f), new Vector2(3f, 2f), 0, 0);

            if (Time >= ChargeTime - 20)
            {
                float laserActive = Utils.GetLerpValue(ChargeTime - 30, ChargeTime + 70, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration, Time, true);

                Vector2[] positions = new Vector2[1500];
                float[] rotations = new float[1500];
                for (int i = 0; i < 1500; i++)
                {
                    positions[i] = Projectile.Center + new Vector2(3500 * (i / 1500f) * (0.5f + laserActive * 0.5f), 0).RotatedBy(Projectile.rotation);
                    rotations[i] = Projectile.rotation;
                }

                VertexStrip strip = new VertexStrip();
                strip.PrepareStripWithProceduralPadding(positions, rotations, StripColor, StripWidth, -Main.screenPosition, true);

                Effect lightningEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/FusionRayEffect", AssetRequestMode.ImmediateLoad).Value;
                lightningEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                lightningEffect.Parameters["uTexture0"].SetValue(texture);
                lightningEffect.Parameters["uTexture1"].SetValue(textureSecond);
                lightningEffect.Parameters["uGlow"].SetValue(textureGlow);
                lightningEffect.Parameters["uBits"].SetValue(textureBits);
                lightningEffect.Parameters["uTime"].SetValue(-Projectile.localAI[0] * 0.025f);
                lightningEffect.Parameters["uFreq"].SetValue(1.5f);
                lightningEffect.CurrentTechnique.Passes[0].Apply();
                strip.DrawTrail();
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                //Color endColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] - 60f);
                //endColor.A = 0;

                //for (int i = 0; i < 12; i++)
                //{
                //    Main.EntitySpriteDraw(spark, positions[positions.Length - 1] - Main.screenPosition, null, endColor * 0.3f, (MathHelper.TwoPi / 12f * i) + (Time * 0.01f * Projectile.direction), spark.Size() * 0.5f, laserActive * 0.7f * new Vector2(1f, 2f), 0, 0);
                //    Main.EntitySpriteDraw(spark, positions[positions.Length - 1] - Main.screenPosition, null, endColor * 0.3f, (MathHelper.TwoPi / 12f * i) + (Time * 0.03f * Projectile.direction), spark.Size() * 0.5f, laserActive * new Vector2(1f, 2f), 0, 0);
                //    Main.EntitySpriteDraw(spark, positions[positions.Length - 1] - Main.screenPosition, null, new Color(255, 255, 255, 0), (MathHelper.TwoPi / 12f * i) + (Time * 0.01f * Projectile.direction), spark.Size() * 0.5f, laserActive * 0.33f * new Vector2(1f, 2f), 0, 0);
                //    Main.EntitySpriteDraw(glow, positions[positions.Length - 1] - Main.screenPosition, null, endColor * 0.3f, (MathHelper.TwoPi / 12f * i) + (Time * 0.01f * Projectile.direction), glow.Size() * 0.5f, laserActive * new Vector2(3f, 20f), 0, 0);

                //}
                //Main.EntitySpriteDraw(glow, positions[positions.Length - 1] - Main.screenPosition, null, endColor, 0, glow.Size() * 0.5f, laserActive * 15f, 0, 0);
            }
            return false;
        }

        public Color StripColor(float progress)
        {
            Color color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] - progress * 100f);
            color.A = 0;
            return color * realSize;
        }
        public float StripWidth(float progress)
        {
            float start = (float)Math.Pow(progress, 0.6f);
            float cap = (float)Math.Cbrt(Utils.GetLerpValue(1.01f, 0.7f, progress, true));
            return start * cap * realSize * 1650f * (1.1f + (float)Math.Cos(Time) * (0.08f - progress * 0.06f));
        }
    }
}
