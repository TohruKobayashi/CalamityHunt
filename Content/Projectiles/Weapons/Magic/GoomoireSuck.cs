using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Particles;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Magic
{
    public class GoomoireSuck : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.DamageType = DamageClass.Magic;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Size => ref Projectile.ai[1];

        public ref Player Owner => ref Main.player[Projectile.owner];

        public override void AI()
        {
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            bool canKill = false;

            Owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
            Owner.SetDummyItemTime(4);

            Owner.heldProj = Projectile.whoAmI;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity.SafeNormalize(Vector2.Zero), Owner.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero), 0.08f) * Owner.HeldItem.shootSpeed;
            Projectile.Center = Owner.MountedCenter + Projectile.velocity.SafeNormalize(Vector2.Zero) * 25 + new Vector2(0, Owner.gfxOffY) + Main.rand.NextVector2Circular(2, 2) * Projectile.ai[2];
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Time % (5 + (int)(Owner.itemAnimationMax)) == 1)
                Owner.CheckMana(15, true);

            //if ((Time - 8) % 5 == 1)
            //{
            //    Vector2 piercerVelocity = Projectile.velocity;

            //    if (Main.myPlayer == Projectile.owner)
            //    {
            //        piercerVelocity = (Main.MouseWorld - Projectile.Center).RotatedByRandom(0.2f) * (0.06f / MathHelper.E);
            //        Projectile.netUpdate = true;
            //    }
            //    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 5f + Main.rand.NextVector2Circular(10, 10), piercerVelocity, ModContent.ProjectileType<CrystalPiercer>(), Owner.HeldItem.damage, 1f, Owner.whoAmI, ai1: Projectile.whoAmI);
            //}

            if ((!Owner.channel || !Owner.CheckMana(15)))
                canKill = true;

            if (!canKill)
                Projectile.timeLeft = 10000;

            if (canKill && Projectile.timeLeft > 30)
                Projectile.timeLeft = 30;

            Time++;

            Projectile.localAI[0] += Projectile.ai[2];

            Projectile.ai[2] = MathF.Sqrt(Utils.GetLerpValue(0, 50, Time, true) * Utils.GetLerpValue(10, 30, Projectile.timeLeft, true));
            Size = 500;
            Projectile.spriteDirection = Owner.direction;

            if (Time < 10)
                newRot = Projectile.rotation;

            newRot = Utils.AngleLerp(newRot, Projectile.rotation, 0.05f);

            HandleSound();
            RibbonPhysics();

            if (Projectile.frameCounter++ > 5)
            {
                if (Projectile.ai[2] > 0.7f)
                {
                    if (++Projectile.frame > 7)
                        Projectile.frame = 3;
                    Projectile.frameCounter = 0;
                }
                else
                    Projectile.frame = (int)(Projectile.ai[2] * 3);
            }

            foreach (Gore gore in Main.gore.Where(n => n.active))
            {
                Rectangle goreRec = new Rectangle((int)(gore.position.X - gore.scale * 10), (int)(gore.position.Y - gore.scale * 10), (int)(gore.scale * 20), (int)(gore.scale * 20));
                if (Utils.IntersectsConeFastInaccurate(goreRec, Projectile.Center, Size, Projectile.rotation, MathHelper.Pi / 8f))
                    gore.velocity = Vector2.Lerp(gore.velocity, gore.position.DirectionTo(Projectile.Center).SafeNormalize(Vector2.Zero) * 15, 0.2f);

                if (gore.position.Distance(Projectile.Center) < 70)
                    gore.scale *= 0.9f;
                if (gore.position.Distance(Projectile.Center) < 40)
                {
                    gore.active = false;
                    Poof();
                }
            }

            foreach (Dust dust in Main.dust.Where(n => n.active && !n.noGravity))
            {
                Rectangle dustRec = new Rectangle((int)(dust.position.X - dust.scale * 3), (int)(dust.position.Y - dust.scale * 3), (int)(dust.scale * 6), (int)(dust.scale * 6));
                if (Utils.IntersectsConeFastInaccurate(dustRec, Projectile.Center, Size, Projectile.rotation, MathHelper.Pi / 8f))
                    dust.velocity = Vector2.Lerp(dust.velocity, dust.position.DirectionTo(Projectile.Center).SafeNormalize(Vector2.Zero) * 15, 0.2f);

                if (dust.position.Distance(Projectile.Center) < 50)
                    dust.scale *= 0.9f;                
                if (dust.position.Distance(Projectile.Center) < 30)
                    dust.active = false;

            }

            foreach (Item item in Main.item.Where(n => n.active))
            {
                if (Utils.IntersectsConeFastInaccurate(item.Hitbox, Projectile.Center, Size, Projectile.rotation, MathHelper.Pi / 8f))
                    item.velocity = Vector2.Lerp(item.velocity, item.DirectionTo(Projectile.Center).SafeNormalize(Vector2.Zero) * 15, 0.2f);
            }

            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Time + 10);

            Vector2 inward = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.5f) * Main.rand.NextFloat(Size * Projectile.ai[2] * 1.5f);
            ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Projectile.Center + inward, -inward * 0.03f, glowColor, 1f + Main.rand.NextFloat());
            Dust d = Dust.NewDustPerfect(Projectile.Center + inward, DustID.Sand, -inward * 0.04f, 10, Color.Black, 1f + Main.rand.NextFloat());
            d.noGravity = true;
        }

        public void Poof()
        {
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Time + 10);

            for (int i = 0; i < 5; i++)
                ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Projectile.Center, Main.rand.NextVector2Circular(3, 3), glowColor, 2f);

        }

        private Vector2[] ribbonPoints;
        private Vector2[] ribbonVels;

        public void RibbonPhysics()
        {
            int length = 6;
            if (ribbonVels != null)
            {
                for (int i = 0; i < ribbonVels.Length; i++)
                    ribbonVels[i] = (Projectile.rotation - (1.5f + i * 0.2f) * Projectile.spriteDirection).ToRotationVector2() * 3f;
            }
            else
                ribbonVels = new Vector2[length];

            if (ribbonPoints != null)
            {
                float drawScale = Projectile.scale;
                ribbonPoints[0] = Projectile.Center + new Vector2(4, -5 * Projectile.spriteDirection).RotatedBy(Projectile.rotation) * drawScale;

                for (int i = 1; i < ribbonPoints.Length; i++)
                {
                    ribbonPoints[i] += ribbonVels[i];
                    if (ribbonPoints[i].Distance(ribbonPoints[i - 1]) > 10)
                        ribbonPoints[i] = Vector2.Lerp(ribbonPoints[i], ribbonPoints[i - 1] + new Vector2(10, 0).RotatedBy(ribbonPoints[i - 1].AngleTo(ribbonPoints[i])), 0.8f);
                }
            }
            else
            {
                ribbonPoints = new Vector2[length];
                for (int i = 0; i < ribbonPoints.Length; i++)
                    ribbonPoints[i] = Projectile.Center;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utils.IntersectsConeFastInaccurate(targetHitbox, Projectile.Center, (Size * 1.33f) * Projectile.ai[2], Projectile.rotation, MathHelper.Pi / 8f);
        }

        public LoopingSound windSoundLoop;

        public void HandleSound()
        {
            if (windSoundLoop == null)
                windSoundLoop = new LoopingSound(AssetDirectory.Sounds.Weapon.GoomoireWindLoop, new ProjectileAudioTracker(Projectile).IsActiveAndInGame);
           
            windSoundLoop.Update(() => Projectile.Center, () => Projectile.ai[2] * 0.5f, () => Projectile.ai[2] - 0.9f);
        }

        public static Asset<Texture2D> ribbonTexture;
        public static Asset<Texture2D> laserTexture;
        public static Asset<Texture2D> laserTexture2;

        public override void Load()
        {
            ribbonTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Ribbon");
            laserTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Laser" + 0);
            laserTexture2 = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Laser" + 1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(1, 8, 0, Projectile.frame);

            SpriteEffects bookEffect = Owner.direction * Owner.gravDir > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            DrawRibbon(lightColor);

            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(-2, -2 * Owner.direction * Owner.gravDir).RotatedBy(Projectile.rotation) - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() * 0.5f, 1f, bookEffect, 0);

            DrawLaserCone();

            return false;
        }


        private void DrawRibbon(Color lightColor)
        {
            if (ribbonPoints != null)
            {
                for (int i = 0; i < ribbonPoints.Length - 1; i++)
                {
                    int style = 0;
                    if (i == ribbonPoints.Length - 3)
                        style = 1;
                    if (i > ribbonPoints.Length - 3)
                        style = 2;
                    Rectangle frame = ribbonTexture.Value.Frame(1, 3, 0, style);
                    float rotation = ribbonPoints[i].AngleTo(ribbonPoints[i + 1]);
                    Vector2 stretch = new Vector2(0.5f + Utils.GetLerpValue(0, ribbonPoints.Length - 2, i, true) * 0.4f, ribbonPoints[i].Distance(ribbonPoints[i + 1]) / (frame.Height - 5));
                    Main.EntitySpriteDraw(ribbonTexture.Value, ribbonPoints[i] - Main.screenPosition, frame, lightColor.MultiplyRGBA(Color.Lerp(Color.DimGray, Color.White, (float)i / ribbonPoints.Length)), rotation - MathHelper.PiOver2, frame.Size() * new Vector2(0.5f, 0f), stretch, 0, 0);
                }
            }
        }

        private float newRot;

        public void DrawLaserCone()
        {
            Vector2 sparklePos = Projectile.Center + new Vector2(6, 0).RotatedBy(Projectile.rotation);
            Texture2D sparkle = AssetDirectory.Textures.Sparkle.Value;
            Color sparkleColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Time + 10);
            sparkleColor.A = 0;

            Vector2 sparkleScaleX = new Vector2(0.5f, 0.9f) * Projectile.ai[2];
            Vector2 sparkleScaleY = new Vector2(0.5f, 1.33f) * Projectile.ai[2];
            Main.EntitySpriteDraw(sparkle, sparklePos - Main.screenPosition, sparkle.Frame(), Color.Black * 0.3f, 0f, sparkle.Size() * 0.5f, sparkleScaleX, 0, 0);
            Main.EntitySpriteDraw(sparkle, sparklePos - Main.screenPosition, sparkle.Frame(), Color.Black * 0.3f, MathHelper.PiOver2, sparkle.Size() * 0.5f, sparkleScaleY, 0, 0);

            Vector2[] positions = new Vector2[500];
            float[] rotations = new float[500];
            for (int i = 0; i < 500; i++)
            {
                rotations[i] = Utils.AngleLerp(newRot, Projectile.rotation, MathF.Sqrt(i / 500f)) + MathF.Sin(Time * 0.2f - i / 50f) * 0.1f * (1f - i / 500f) * Projectile.ai[2];
                positions[i] = sparklePos + new Vector2((Size * 1.4f) * (i / 500f) * Projectile.ai[2], 0).RotatedBy(rotations[i]);
            }

            VertexStrip strip = new VertexStrip();
            strip.PrepareStripWithProceduralPadding(positions, rotations, StripColor, StripWidth, -Main.screenPosition, true);

            Effect lightningEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/GoomoireSuckEffect", AssetRequestMode.ImmediateLoad).Value;
            lightningEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            lightningEffect.Parameters["uTexture0"].SetValue(laserTexture.Value);
            lightningEffect.Parameters["uTexture1"].SetValue(laserTexture2.Value);
            lightningEffect.Parameters["uTime"].SetValue(Projectile.localAI[0] * 0.021f);
            lightningEffect.Parameters["uFreq"].SetValue(1f);
            lightningEffect.Parameters["uMiddleBrightness"].SetValue(1.3f);
            lightningEffect.Parameters["uBackPhaseShift"].SetValue(0.5f);
            lightningEffect.Parameters["uSlant"].SetValue(2f);
            lightningEffect.CurrentTechnique.Passes[0].Apply();
            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            Main.EntitySpriteDraw(sparkle, sparklePos - Main.screenPosition, sparkle.Frame(), sparkleColor, 0f, sparkle.Size() * 0.5f, sparkleScaleX, 0, 0);
            Main.EntitySpriteDraw(sparkle, sparklePos - Main.screenPosition, sparkle.Frame(), sparkleColor, MathHelper.PiOver2, sparkle.Size() * 0.5f, sparkleScaleY, 0, 0);

            Main.EntitySpriteDraw(sparkle, sparklePos - Main.screenPosition, sparkle.Frame(), new Color(255, 255, 255, 0), 0f, sparkle.Size() * 0.5f, sparkleScaleX * 0.5f, 0, 0);
            Main.EntitySpriteDraw(sparkle, sparklePos - Main.screenPosition, sparkle.Frame(), new Color(255, 255, 255, 0), MathHelper.PiOver2, sparkle.Size() * 0.5f, sparkleScaleY * 0.5f, 0, 0);
        }

        public Color StripColor(float progress)
        {
            Color color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Time + 10 + progress * 40f) * 0.45f;
            color.A /= 2;
            return color * Projectile.ai[2];
        }

        public float StripWidth(float progress)
        {
            float start = MathHelper.SmoothStep(MathF.Pow(progress, 0.6f) * 0.2f, 1, progress);
            float grow = (float)Math.Pow(Projectile.ai[2], 3f);
            return start * grow * Size * 0.4667f;
        }
    }
}
