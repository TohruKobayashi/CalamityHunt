using System;
using System.IO;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Melee
{
    public class ScytheOfTheOldGodHeld : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 9;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.noEnchantmentVisuals = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float SwingStyle => ref Projectile.ai[1];

        public ref Player Player => ref Main.player[Projectile.owner];

        private float shakeStrength;
        private float slashRotation;
        private float swingPercent;
        private float swingPercent2;

        public override void AI()
        {
            if (!Player.active || Player.dead || Player.noItems || Player.CCed) {
                Projectile.Kill();
                return;
            }

            Projectile.scale = Player.GetAdjustedItemScale(Player.HeldItem);

            if (Time <= 0) {
                Projectile.localAI[0] = Main.rand.NextFloat(20f);

                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++) {
                    Projectile.oldRot[i] = Projectile.rotation;
                }

                Projectile.velocity = Player.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 5f;
                Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
            }

            float rotation = 0;
            float addRot = 0;

            SoundStyle swingSound = AssetDirectory.Sounds.Weapons.ScytheOfTheOldGodSwing;
            swingSound.Volume *= 0.8f;
            SoundStyle strongSound = AssetDirectory.Sounds.Weapons.ScytheOfTheOldGodSwingStrong;
            strongSound.Volume *= 0.8f;

            float speed = (Player.itemAnimationMax / 36f) / (0.9f + Player.GetTotalAttackSpeed(DamageClass.Melee) * 0.1f);

            switch (SwingStyle) {
                default:
                case 0:

                    Projectile.spriteDirection = Projectile.direction;
                    swingPercent2 = MathF.Pow(Utils.GetLerpValue((int)(-20 * speed), (int)(60 * speed), Time, true), 1.5f);
                    swingPercent = SwingProgress(MathHelper.SmoothStep(0, 1, swingPercent2));
                    rotation = MathHelper.Lerp(-3f, 2f, swingPercent);

                    if (Time == (int)(35 * speed)) {
                        shakeStrength = 1f;
                        SoundEngine.PlaySound(swingSound.WithPitchOffset(-0.3f), Projectile.Center);
                    }

                    if (Time > (int)(60 * speed)) {
                        Projectile.Kill();
                    }

                    break;

                case 1:

                    Projectile.spriteDirection = -Projectile.direction;
                    swingPercent2 = MathF.Pow(Utils.GetLerpValue((int)(0 * speed), (int)(70 * speed), Time, true), 1.1f);
                    swingPercent = SwingProgress(MathHelper.SmoothStep(0, 1, swingPercent2));
                    rotation = MathHelper.Lerp(2f, -3f, swingPercent);
                    addRot = -MathHelper.TwoPi * MathHelper.SmoothStep(0, 1, MathHelper.SmoothStep(0, 1, Utils.GetLerpValue((int)(-10 * speed), (int)(60 * speed), Time, true)));

                    if (Time == (int)(22 * speed)) {
                        SoundEngine.PlaySound(strongSound.WithPitchOffset(-0.05f), Projectile.Center);
                    }

                    if (Time == (int)(35 * speed)) {
                        shakeStrength = 1f;
                    }


                    if (Time > (int)(60 * speed)) {
                        Projectile.Kill();
                    }

                    break;

                case 2:

                    Projectile.scale *= 1.2f;
                    Projectile.spriteDirection = Projectile.direction;
                    swingPercent2 = MathF.Pow(Utils.GetLerpValue(0, (int)(60 * speed), Time, true), 1.1f);
                    swingPercent = SwingProgress(swingPercent2);
                    rotation = MathHelper.Lerp(-2f, 6f, swingPercent);
                    addRot = -MathHelper.Pi * MathF.Sqrt(Utils.GetLerpValue(0, (int)(60 * speed), Time, true));

                    if (Time == (int)(30 * speed)) {
                        shakeStrength = 2f;
                        SoundEngine.PlaySound(strongSound.WithPitchOffset(0.25f), Projectile.Center);
                    }

                    if (Time > (int)(55 * speed)) {
                        Time = -1;
                        SwingStyle--;
                        if (Main.myPlayer == Projectile.owner) {
                            Projectile.Kill();
                        }
                    }

                    break;
            }

            Vector2 scytheEnd = Projectile.Center + Projectile.rotation.ToRotationVector2() * 110 * (Projectile.scale + (1f - swingPercent) * swingPercent * 4f);
            Vector2 swingEnd = Projectile.Center + Projectile.rotation.ToRotationVector2() * 200 * (Projectile.scale + (1f - swingPercent) * swingPercent * 4f);

            if (swingPercent2 > 0.45f && swingPercent < 0.964f) {
                Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.1f, 0.1f).ValueAt(Projectile.localAI[0]) with { A = 128 };

                for (int i = 0; i < 2 + (int)((swingPercent - 0.1) * 12f); i++) {
                    CalamityHunt.particles.Add(Particle.Create<FusionFlameParticle>(particle => {
                        particle.position = Vector2.Lerp(scytheEnd, swingEnd, Main.rand.NextFloat(0.5f, 0.6f)) + Main.rand.NextVector2Circular(10, 120).RotatedBy(Projectile.rotation + (i / 5f * 0.1f) * Projectile.spriteDirection);
                        particle.velocity = (Projectile.rotation + (MathHelper.PiOver2 + i / 5f * 0.01f) * Projectile.spriteDirection).ToRotationVector2().RotatedByRandom(0.2f) * Main.rand.NextFloat(3f, 6f);
                        particle.rotation = particle.velocity.ToRotation();
                        particle.scale = (swingPercent - 0.1f) * 5f + Main.rand.NextFloat(1f, 2f);
                        particle.maxTime = Main.rand.Next(25, 40);
                        particle.color = glowColor * 1.5f;
                        particle.fadeColor = glowColor * 0.4f;
                        particle.anchor = () => Player.velocity * 0.4f;
                        particle.gravity = particle.position.DirectionTo(Projectile.Center) * 0.1f;
                    }));
                }

                for (int i = 0; i < 2; i++) {
                    float scale = 0.1f + Main.rand.NextFloat() + swingPercent2;
                    Dust light = Dust.NewDustPerfect(scytheEnd + Main.rand.NextVector2Circular(30, 70).RotatedBy(Projectile.rotation), DustID.AncientLight, (Projectile.rotation + MathHelper.PiOver2 * Projectile.spriteDirection).ToRotationVector2().RotatedByRandom(0.5f) * Main.rand.NextFloat(-2f, 10f), 0, glowColor, scale);
                    light.noGravity = true;
                    light.noLightEmittence = true;
                }

                for (int i = 0; i < 4; i++) {
                    float scale = 0.1f + Main.rand.NextFloat() + swingPercent2;
                    Dust light = Dust.NewDustPerfect(swingEnd + Main.rand.NextVector2Circular(50, 100).RotatedBy(Projectile.rotation), DustID.AncientLight, (Projectile.rotation + MathHelper.PiOver4 * Projectile.spriteDirection).ToRotationVector2().RotatedByRandom(0.5f) * Main.rand.NextFloat(-2f, 20f), 0, glowColor, scale);
                    light.noGravity = true;
                    light.noLightEmittence = true;
                }

                Projectile.EmitEnchantmentVisualsAt(scytheEnd - new Vector2(75), 150, 150);

                if (Main.rand.NextBool(2)) {
                    CalamityHunt.particles.Add(Particle.Create<CrossSparkle>(particle => {
                        particle.position = scytheEnd + Main.rand.NextVector2Circular(70, 70);
                        particle.velocity = Vector2.Zero;
                        particle.scale = Main.rand.NextFloat(1.5f);
                        particle.color = glowColor;
                    }));
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + (rotation + MathHelper.WrapAngle(addRot)) * Projectile.direction;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation * Player.gravDir - MathHelper.PiOver2);
            Vector2 position = Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            position.Y += Player.gfxOffY;
            Projectile.Center = position;

            Player.SetDummyItemTime(2);
            Player.itemTime = 2;
            Player.ChangeDir(Projectile.direction);
            Player.heldProj = Projectile.whoAmI;

            Time++;
            Projectile.localAI[0]++;

            Vector2 ribbonStart = Projectile.Center + new Vector2(45, -11 * Projectile.spriteDirection).RotatedBy(Projectile.rotation) * (Projectile.scale + (1f - swingPercent) * swingPercent * 4f);
            ribbon ??= new Rope(ribbonStart, 20, 10f, Vector2.Zero, 0.5f);
            ribbon.StartPos = ribbonStart;
            for (int i = 0; i < ribbon.segments.Count; i++) {
                ribbon.segments[i].position += (Projectile.rotation - (1.3f + i * 0.01f) * Projectile.spriteDirection).ToRotationVector2() * 3f;
            }
            ribbon.Update();

            shakeStrength = MathHelper.Clamp(shakeStrength, 0f, 2.4f);
            if (shakeStrength >= 0f) {
                shakeStrength *= 0.9f;
            }

            if (shakeStrength < 0.2f) {
                shakeStrength = 0f;
            }

            if (Main.myPlayer == Projectile.owner && shakeStrength > 0f) {
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f), shakeStrength * 4f, 10, 10));
            }

            slashRotation = Projectile.rotation;
            for (int i = ProjectileID.Sets.TrailCacheLength[Type] - 1; i > 0; i--) {
                Projectile.oldRot[i] = Projectile.oldRot[i].AngleLerp(Projectile.oldRot[i - 1], 0.8f);
            }

            Projectile.oldRot[0] = Projectile.oldRot[0].AngleLerp(slashRotation, 0.8f);
        }

        public float SwingProgress(float x)
        {
            float[] functions = new float[]
            {
                0.2f * MathF.Sqrt(x),
                18f * MathF.Pow(x - 0.25f, 4f) + 0.1f,
                0.3f * MathF.Cbrt(x - 0.7f) + 0.85f,
                0.2f * x + 0.85f
            };

            if (x < 0.05f) {
                return MathHelper.Lerp(0f, functions[0], Utils.GetLerpValue(0f, 0.05f, x, true));
            }

            if (x < 0.3f) {
                return MathHelper.Lerp(functions[0], functions[1], Utils.GetLerpValue(0.2f, 0.3f, x, true));
            }

            if (x < 0.71f) {
                return MathHelper.Lerp(functions[1], functions[2], Utils.GetLerpValue(0.701f, 0.71f, x, true));
            }
            else {
                return MathHelper.Lerp(functions[2], functions[3], Utils.GetLerpValue(0.8f, 1f, x, true));
            }
        }

        private Rope ribbon;

        //public void RibbonPhysics()
        //{
        //    int length = 12;
        //    if (ribbonVels != null)
        //    {
        //        for (int i = 0; i < ribbonVels.Length; i++)
        //        {
        //            ribbonVels[i] = (Projectile.rotation - (1.3f + i * 0.01f) * Projectile.spriteDirection).ToRotationVector2() * 8f;
        //        }
        //    }
        //    else
        //        ribbonVels = new Vector2[length];

        //    if (ribbonPoints != null)
        //    {
        //        float drawScale = Projectile.scale + (1f - swingPercent) * swingPercent * 4f;
        //        ribbonPoints[0] = Projectile.Center + new Vector2(49, -13 * Projectile.spriteDirection).RotatedBy(Projectile.rotation) * drawScale;

        //        for (int i = 1; i < ribbonPoints.Length; i++)
        //        {
        //            ribbonPoints[i] += ribbonVels[i];
        //            if (ribbonPoints[i].Distance(ribbonPoints[i - 1]) > 10)
        //                ribbonPoints[i] = Vector2.Lerp(ribbonPoints[i], ribbonPoints[i - 1] + new Vector2(10, 0).RotatedBy(ribbonPoints[i - 1].AngleTo(ribbonPoints[i])), 0.8f);
        //        }
        //    }
        //    else
        //    {
        //        ribbonPoints = new Vector2[length];
        //        for (int i = 0; i < ribbonPoints.Length; i++)
        //            ribbonPoints[i] = Projectile.Center;
        //    }
        //}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.1f, 0.1f).ValueAt(Time) with { A = 128 };
            CalamityHunt.particles.Add(Particle.Create<CrossSparkle>(particle => {
                particle.position = Main.rand.NextVector2FromRectangle(target.Hitbox);
                particle.velocity = Vector2.Zero;
                particle.scale = Main.rand.NextFloat(1f, 3f);
                particle.color = glowColor;
            }));
            target.AddBuff(ModContent.BuffType<FusionBurn>(), 300);
            shakeStrength *= 1.5f;

            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.1f, 0.1f).ValueAt(Time) with { A = 128 };

            CalamityHunt.particles.Add(Particle.Create<CrossSparkle>(particle => {
                particle.position = Main.rand.NextVector2FromRectangle(target.Hitbox);
                particle.velocity = Vector2.Zero;
                particle.scale = Main.rand.NextFloat(1f, 3f);
                particle.color = glowColor;
            }));

            target.AddBuff(ModContent.BuffType<FusionBurn>(), 300);
            shakeStrength *= 1.5f;

            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            float itemScale = Player.GetAdjustedItemScale(Player.HeldItem);
            int size = (int)(450 * itemScale);

            hitbox.Width = size;
            hitbox.Height = size;
            hitbox.Location = (Projectile.Center + Projectile.rotation.ToRotationVector2() * 60f + Projectile.velocity.SafeNormalize(Vector2.Zero) * 100 * itemScale - new Vector2(size * 0.5f)).ToPoint();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (swingPercent2 > 0.5f && swingPercent2 < 0.7f || (SwingStyle == 1 && swingPercent2 > 0.25f && swingPercent2 < 0.35f)) {
                return base.Colliding(projHitbox, targetHitbox);
            }

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(swingPercent2);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            swingPercent2 = reader.ReadSingle();
        }

        public override void Load()
        {
            glowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.ImmediateLoad).Value;
            bladeTexture = ModContent.Request<Texture2D>(Texture + "Blade", AssetRequestMode.ImmediateLoad).Value;
            ribbonTexture = ModContent.Request<Texture2D>(Texture + "Ribbon", AssetRequestMode.ImmediateLoad).Value;
        }

        public static Texture2D glowTexture;
        public static Texture2D bladeTexture;
        public static Texture2D ribbonTexture;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Time < 1) {
                return false;
            }

            lightColor = Color.Lerp(lightColor, Color.White, 0.4f);

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D swingTexture = AssetDirectory.Textures.SwordSwing[2].Value;

            Vector2 endOrigin = new Vector2(0.35f, 0.5f + 0.3f * Projectile.spriteDirection);
            Vector2 origin = endOrigin;
            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.1f, 0.1f).ValueAt(Projectile.localAI[0]) with { A = 0 };

            float drawScale = Projectile.scale + (1f - swingPercent) * swingPercent * 4f;
            float swingStrength = Utils.GetLerpValue(0.1f, 0.7f, swingPercent, true) * Utils.GetLerpValue(0.9f, 0.6f, swingPercent2, true);

            switch (SwingStyle) {
                case 1:

                    //origin = Vector2.Lerp(endOrigin, new Vector2(0.45f, 0.5f + 0.08f * Projectile.spriteDirection), Utils.GetLerpValue(0.6f, 0.55f, swingPercent2, true));
                    swingStrength = Utils.GetLerpValue(0.1f, 0.7f, swingPercent2, true) * Utils.GetLerpValue(0.9f, 0.6f, swingPercent2, true);

                    break;
            }

            if (swingStrength > 0f) {

                Vector2 slashOrigin = new Vector2(0.5f, 0.49f);
                for (int i = 0; i < 4; i++) {
                    Color trailBackColor = (new GradientColor(SlimeUtils.GoozOilColors, 0.1f, 0.1f).ValueAt(Projectile.localAI[0] - i * 5f) * (1f - (float)i / Projectile.oldRot.Length)) with { A = 200 };

                    Rectangle frame = swingTexture.Frame(1, 4, 0, i);
                    Main.EntitySpriteDraw(swingTexture, Projectile.Center - Main.screenPosition, frame, trailBackColor * 0.8f * swingStrength * (1f - i / 4f) * swingStrength, Projectile.oldRot[i] - (0.5f + i * 0.3f) * Projectile.spriteDirection, frame.Size() * slashOrigin, drawScale + 2.8f, spriteEffects, 0);
                }
                for (int i = 0; i < 4; i++) {
                    Color trailGlowColor = (new GradientColor(SlimeUtils.GoozOilColors, 0.1f, 0.1f).ValueAt(Projectile.localAI[0] - i * 5f) * swingStrength * (1f - (float)i / Projectile.oldRot.Length) * 2f) with { A = 128 };
                    Rectangle frame = swingTexture.Frame(1, 4, 0, i);
                    Main.EntitySpriteDraw(swingTexture, Projectile.Center - Main.screenPosition, frame, trailGlowColor * 0.5f * (1f - i / 4f) * swingStrength, Projectile.oldRot[i * 2] - (0.3f + i * 0.1f) * Projectile.spriteDirection, frame.Size() * slashOrigin, drawScale + 2.8f, spriteEffects, 0);
                    Main.EntitySpriteDraw(swingTexture, Projectile.Center - Main.screenPosition, frame, trailGlowColor * 0.5f * (1f - i / 4f) * swingStrength, Projectile.oldRot[i] - (1f + i * 0.5f) * Projectile.spriteDirection, frame.Size() * slashOrigin, drawScale + 2.8f, spriteEffects, 0);
                }

                Main.EntitySpriteDraw(swingTexture, Projectile.Center - Main.screenPosition, swingTexture.Frame(1, 4, 0, 3), new Color(50, 50, 50, 0) * swingStrength, Projectile.rotation - 0.3f * Projectile.spriteDirection, swingTexture.Frame(1, 4, 0, 3).Size() * 0.5f, drawScale + 1.85f, spriteEffects, 0);
                Main.EntitySpriteDraw(swingTexture, Projectile.Center - Main.screenPosition, swingTexture.Frame(1, 4, 0, 3), new Color(150, 150, 150, 0) * swingStrength, Projectile.rotation - 0.5f * Projectile.spriteDirection, swingTexture.Frame(1, 4, 0, 3).Size() * 0.5f, drawScale + 2.55f, spriteEffects, 0);

            }

            //outer slash
            //Vector2 waveOff = new Vector2(swingPercent * 200, 0).RotatedBy(Projectile.velocity.ToRotation());
            //float waveSrength = Utils.GetLerpValue(0.5f, 0.8f, swingPercent2, true) * Utils.GetLerpValue(1f, 0.9f, swingPercent, true);
            //Main.EntitySpriteDraw(swingTexture, Projectile.Center + waveOff - Main.screenPosition, swingTexture.Frame(1, 4, 0, 0), glowColor * waveSrength * 0.2f, Projectile.velocity.ToRotation(), swingTexture.Frame(1, 4, 0, 1).Size() * 0.5f, drawScale + swingPercent * 5f, spriteEffects, 0);
            //Main.EntitySpriteDraw(swingTexture, Projectile.Center + waveOff - Main.screenPosition, swingTexture.Frame(1, 4, 0, 2), glowColor * waveSrength * 0.1f, Projectile.velocity.ToRotation(), swingTexture.Frame(1, 4, 0, 1).Size() * 0.5f, drawScale + swingPercent * 3f, spriteEffects, 0);

            float sparkStrength = (1f - swingPercent2) * swingPercent2 * 4f * Utils.GetLerpValue(0.3f, 0.5f, swingPercent, true);

            DrawRibbon(lightColor);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), lightColor, Projectile.rotation + MathHelper.PiOver4 * Projectile.spriteDirection, texture.Size() * origin, drawScale, spriteEffects, 0);
            Main.EntitySpriteDraw(glowTexture, Projectile.Center - Main.screenPosition, glowTexture.Frame(), glowColor, Projectile.rotation + MathHelper.PiOver4 * Projectile.spriteDirection, texture.Size() * origin, drawScale, spriteEffects, 0);
            Main.EntitySpriteDraw(glowTexture, Projectile.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.5f, Projectile.rotation + MathHelper.PiOver4 * Projectile.spriteDirection, texture.Size() * origin, drawScale * 1.01f, spriteEffects, 0);

            DrawSpark(Projectile.Center + new Vector2(80, 0).RotatedBy(Projectile.rotation + 0.2f * Projectile.spriteDirection) * (drawScale + 2.7f), glowColor * 0.5f * sparkStrength, sparkStrength);
            DrawSpark(Projectile.Center + new Vector2(80, 0).RotatedBy(Projectile.rotation - 0.5f * Projectile.spriteDirection) * (drawScale + 2.7f), glowColor * 0.7f * sparkStrength, 2f * sparkStrength);
            DrawSpark(Projectile.Center + new Vector2(80, 0).RotatedBy(Projectile.rotation - 1f * Projectile.spriteDirection) * (drawScale + 2.7f), glowColor * sparkStrength, 1.5f * sparkStrength);
            DrawSpark(Projectile.Center + new Vector2(80, 0).RotatedBy(Projectile.rotation - 1.4f * Projectile.spriteDirection) * (drawScale + 2.7f), glowColor * 0.7f * sparkStrength, sparkStrength);
            DrawSpark(Projectile.Center + new Vector2(80, 0).RotatedBy(Projectile.rotation - 1.8f * Projectile.spriteDirection) * (drawScale + 2.7f), glowColor * 0.3f * sparkStrength, 0.5f * sparkStrength);

            return false;
        }

        public void DrawSpark(Vector2 position, Color color, float scale)
        {
            Texture2D texture = AssetDirectory.Textures.Sparkle.Value;
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(), color, MathHelper.PiOver4, texture.Size() * 0.5f, new Vector2(1f, 2f) * scale, 0, 0);
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(), color, -MathHelper.PiOver4, texture.Size() * 0.5f, new Vector2(1f, 2f) * scale, 0, 0);
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(), new Color(200, 200, 200, 0), MathHelper.PiOver4, texture.Size() * 0.5f, new Vector2(0.5f, 1f) * scale, 0, 0);
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(), new Color(200, 200, 200, 0), -MathHelper.PiOver4, texture.Size() * 0.5f, new Vector2(0.5f, 1f) * scale, 0, 0);
        }

        private void DrawRibbon(Color lightColor)
        {
            if (ribbon != null) {
                Vector2[] ribbonPoints = ribbon.GetPoints().ToArray();
                for (int i = 0; i < ribbonPoints.Length - 1; i++) {
                    int style = 0;
                    if (i == ribbonPoints.Length - 3) {
                        style = 1;
                    }

                    if (i > ribbonPoints.Length - 3) {
                        style = 2;
                    }

                    Rectangle frame = ribbonTexture.Frame(1, 3, 0, style);
                    float rotation = ribbonPoints[i].AngleTo(ribbonPoints[i + 1]);
                    Vector2 stretch = new Vector2(0.5f + Utils.GetLerpValue(0, ribbonPoints.Length - 2, i, true) * 0.8f, ribbonPoints[i].Distance(ribbonPoints[i + 1]) / (frame.Height - 5));
                    Main.EntitySpriteDraw(ribbonTexture, ribbonPoints[i] - Main.screenPosition, frame, lightColor.MultiplyRGBA(Color.Lerp(Color.DimGray, Color.White, (float)i / ribbonPoints.Length)), rotation - MathHelper.PiOver2, frame.Size() * new Vector2(0.5f, 0f), stretch, 0, 0);
                }
            }
        }
    }
}
