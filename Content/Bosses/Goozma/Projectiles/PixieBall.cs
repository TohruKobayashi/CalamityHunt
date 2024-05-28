using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
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
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class PixieBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Cooldown => ref Projectile.ai[1];
        public ref float HitCount => ref Projectile.ai[2];

        private int owner;
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * 0.01f * (Projectile.velocity.X > 0 ? 1 : -1);
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(0, 17, Time, true) * Utils.GetLerpValue(480, 460, Time, true)) * 1.3f;
            owner = -1;
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<DivineGargooptuar>() && n.active))
            {
                Projectile.active = false;
                return;
            }
            else
                owner = Main.npc.First(n => n.type == ModContent.NPCType<DivineGargooptuar>() && n.active).whoAmI;

            if (Time < 60)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Main.npc[owner].GetTargetData().Velocity, 0.9f);

            if (HitCount < 0 || HitCount == 1)
            {
                Projectile.velocity = Projectile.DirectionTo(Main.npc[owner].GetTargetData().Center).SafeNormalize(Vector2.Zero) * 36f;
                Cooldown = 0;
            }
            else
            {
                if (Main.rand.NextBool(5))
                    Projectile.velocity += Main.rand.NextVector2Circular(3, 3);

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[owner].Center).SafeNormalize(Vector2.Zero) * 3f, 0.03f) * Utils.GetLerpValue(500, 470, Time, true);
                Projectile.velocity += Projectile.DirectionTo(Main.npc[owner].Center).SafeNormalize(Vector2.Zero) * (0.3f + Utils.GetLerpValue(500, 800, Projectile.Distance(Main.npc[owner].Center), true));
            }

            if (Cooldown <= 0)
            {
                if (Projectile.Distance(Main.npc[owner].Center) < 84 && Time > 60)
                {
                    HitCount++;
                    Main.npc[owner].localAI[1]++;
                    Projectile.velocity = Projectile.DirectionTo(Main.npc[owner].GetTargetData().Center).SafeNormalize(Vector2.Zero) * (Main.npc[owner].GetTargetData().Velocity.Length() * 0.2f + Projectile.velocity.Length());
                    Cooldown = 15;
                    for (int i = 0; i < 40; i++)
                    {
                        Color glowColor = Main.hslToRgb(Projectile.localAI[0] * 0.01f % 1f, 1f, 0.5f, 0);
                        glowColor.A /= 2;
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(36, 36), DustID.AncientLight, Main.rand.NextVector2Circular(15, 15) + Projectile.velocity, 0, glowColor, 1f + Main.rand.NextFloat(2f)).noGravity = true;
                    }
                }

                foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(Projectile.Center) < 64))
                {
                    if (HitCount < 0 || HitCount == 1)
                    {
                        HitCount++;
                        Cooldown += 15;
                        Projectile.velocity = -Vector2.UnitY * 10;
                    }
                    else
                        Projectile.velocity = Projectile.DirectionFrom(player.Center).SafeNormalize(Vector2.Zero) * (14f + Projectile.velocity.Length() + player.velocity.Length());
                    
                    Cooldown += 15;

                    if (Time > 40)
                        SoundEngine.PlaySound(AssetDirectory.Sounds.Slime.PixieBallBounce, Projectile.Center);

                    for (int i = 0; i < 40; i++)
                    {
                        Color glowColor = Main.hslToRgb(Projectile.localAI[0] * 0.01f % 1f, 1f, 0.5f, 0);
                        glowColor.A /= 2;
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(36, 36), DustID.AncientLight, Main.rand.NextVector2Circular(15, 15) + Projectile.velocity, 0, glowColor, 1f + Main.rand.NextFloat(2f)).noGravity = true;
                        
                        if (Main.rand.NextBool(3))
                            ParticleBehavior.NewParticle(ModContent.GetInstance<PrettySparkleParticleBehavior>(), Projectile.Center + Main.rand.NextVector2Circular(54, 54), Main.rand.NextVector2Circular(10, 10) + Projectile.velocity * 0.1f, Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.7f, 0) * 0.5f, 0.5f + Main.rand.NextFloat());
                    }

                    break;
                }

                //foreach(NPC goozma in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<Goozma>() && n.Distance(Projectile.Center) < 64))
                //{
                //    Projectile.velocity += Projectile.DirectionTo(goozma.GetTargetData().Center).SafeNormalize(Vector2.Zero) * (12f + Projectile.velocity.Length());
                //    Cooldown = 15;
                //    for (int i = 0; i < 40; i++)
                //    {
                //        Color glowColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f + i / 120f) % 1f, 1f, 0.6f, 0);
                //        glowColor.A /= 2;
                //        Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Main.rand.NextVector2Circular(5, 5) + Projectile.velocity, 0, glowColor, 1f + Main.rand.NextFloat(2f)).noGravity = true;
                //    }
                //}
            }
            if (Cooldown > 0)
            {
                Cooldown--;

                ParticleBehavior.NewParticle(ModContent.GetInstance<CosmicSmokeParticleBehavior>(), Projectile.Center, Main.rand.NextVector2Circular(3, 3) + Projectile.velocity.RotatedByRandom(0.1f) * 0.5f, Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.7f, 0), Projectile.scale + Main.rand.NextFloat(0.7f));
            }

            if (HitCount > 2)
            {
                Main.npc[owner].ai[0] = 0;
                Main.npc[owner].ai[1] = -1;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    Main.npc[owner].netUpdate = true;
                Projectile.Kill();
            }

            if (Time > 400)
                Projectile.velocity *= 0.9f;

            if (Time > 480)
                Projectile.Kill();

            if (Main.rand.NextBool(5))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(36, 36), DustID.AncientLight, Main.rand.NextVector2Circular(3, 3), 0, Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.6f, 128), 1f + Main.rand.NextFloat(2f)).noGravity = true;

            if (Main.rand.NextBool(2))
                ParticleBehavior.NewParticle(ModContent.GetInstance<PrettySparkleParticleBehavior>(), Projectile.Center + Main.rand.NextVector2Circular(54, 54), Main.rand.NextVector2Circular(7, 7), Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.7f, 0), 0.2f + Main.rand.NextFloat());

            //Particle.NewParticle(ModContent.GetInstance<CosmicSmoke>(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.1f) * 0.8f, Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.7f, 0), Projectile.scale + Main.rand.NextFloat(0.7f));

            for (int i = ProjectileID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
                Projectile.oldRot[i] = Projectile.oldRot[i - 1];
            }
            Projectile.oldPos[0] = Projectile.Center;
            Projectile.oldRot[0] = Projectile.rotation;

            if (Time > 400)
                for (int i = 0; i < 40 - (Time / 2); i++)
                {
                    Color glowColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.5f, 0);
                    glowColor.A /= 2;
                    Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Main.rand.NextVector2Circular(25 - Time / 4f, 25 - Time / 4f), 0, glowColor, 2f + Main.rand.NextFloat(2f)).noGravity = true;
                }

            HandleSound();

            Projectile.localAI[1] += (float)Math.Sqrt(Utils.GetLerpValue(1000, 0, Projectile.Distance(Main.npc[owner].Center), true) * 5f);
            Projectile.localAI[0]++;
            Time++;

            UpdateHitMeSign();
        }

        public LoopingSound auraSound;
        public float volume;

        public void HandleSound()
        {
            if (Projectile.localAI[1] > 10f)
            {
                Projectile.localAI[1] = 0;
                float warningPitch = Math.Clamp(1f - Projectile.Distance(Main.npc[owner].Center) * 0.0006f, -2f, 2f) * 1.1f - 1f;
                float warningVolume = Math.Clamp(1f - Projectile.Distance(Main.npc[owner].Center) * 0.001f, 0.05f, 2f) * Projectile.scale;
                SoundEngine.PlaySound(AssetDirectory.Sounds.Slime.Warning.WithPitchOffset(warningPitch).WithVolumeScale(warningVolume * 0.15f), Projectile.Center);
            }

            volume = Math.Clamp(1f + Projectile.velocity.Length() * 0.0001f - Main.LocalPlayer.Distance(Projectile.Center) * 0.0005f, 0, 1) * Projectile.scale;

            if (auraSound == null)
                auraSound = new LoopingSound(AssetDirectory.Sounds.Slime.PixieBallLoop, new ProjectileAudioTracker(Projectile).IsActiveAndInGame);
            auraSound.Update(() => Projectile.Center, () => volume, () => 0f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(oldVelocity.X - Projectile.velocity.X) > 1)
                Projectile.velocity.X = -oldVelocity.X;
            if (Math.Abs(oldVelocity.Y - Projectile.velocity.Y) > 1)
                Projectile.velocity.Y = -oldVelocity.Y;

            return false;
        }

        public override bool PreKill(int timeLeft)
        {
            //bool active = SoundEngine.TryGetActiveSound(auraSound, out ActiveSound sound);
            //if (active)
            //    sound.Stop();

            return true;
        }

        public static Asset<Texture2D> beachBallOverlay;
        public static Asset<Texture2D> hitMeSign;
        public static Asset<Texture2D> hitMeHand;

        public override void Load()
        {
            beachBallOverlay = AssetDirectory.Textures.Goozma.PixieBeachBall;
            hitMeSign = AssetDirectory.Textures.Goozma.PixieHitMeSign;
            hitMeHand = AssetDirectory.Textures.Goozma.PixieHitMeHand;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D sparkle = TextureAssets.Extra[98].Value;
            Texture2D ring = AssetDirectory.Textures.GlowRing.Value;
            Texture2D glow = AssetDirectory.Textures.Glow.Value;

            Color bloomColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.7f, 0);
            SpriteEffects direction = Projectile.velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.Black * 0.1f, Projectile.rotation * 0.5f, texture.Size() * 0.5f, Projectile.scale * 1.5f, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), bloomColor, Projectile.rotation * 1.5f, texture.Size() * 0.5f, Projectile.scale * 0.8f, 0, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), bloomColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.9f, direction, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(200, 200, 200, 0), Projectile.rotation + 0.2f, texture.Size() * 0.5f, Projectile.scale * 0.8f, direction, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(255, 255, 255, 0), Projectile.rotation + 0.2f, texture.Size() * 0.5f, Projectile.scale * 0.7f, direction, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), bloomColor * 0.2f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale, direction, 0);

            Color overlayColor = bloomColor * 0.6f;
            overlayColor.A = 100;
            Main.EntitySpriteDraw(beachBallOverlay.Value, Projectile.Center - Main.screenPosition, beachBallOverlay.Value.Frame(), overlayColor * 0.1f, Projectile.rotation * 0.7f, beachBallOverlay.Value.Size() * 0.5f, Projectile.scale * 0.9f, 0, 0);

            float lensAngle = Projectile.AngleFrom(Main.LocalPlayer.Center) + MathHelper.PiOver2;
            float lensPower = 1f + Projectile.Distance(Main.LocalPlayer.Center) * 0.003f;

            float sparkRotation = Projectile.velocity.X * 0.01f;
            float wobble = 1f + (float)Math.Sin(Projectile.localAI[0] * 0.5f) * 0.05f;
            Vector2 sparkleScale = new Vector2(0.5f, 6f) * wobble * Utils.GetLerpValue(0.3f, 1f, Projectile.scale, true);
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, null, bloomColor * 0.1f, sparkRotation + MathHelper.PiOver2 + 0.2f, sparkle.Size() * 0.5f, sparkleScale, 0, 0);
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), sparkRotation + MathHelper.PiOver2 + 0.2f, sparkle.Size() * 0.5f, sparkleScale * 0.4f, 0, 0);
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, null, bloomColor * 0.1f, sparkRotation + MathHelper.PiOver2 - 0.3f, sparkle.Size() * 0.5f, sparkleScale * 0.7f, 0, 0);
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), sparkRotation + MathHelper.PiOver2 - 0.3f, sparkle.Size() * 0.5f, sparkleScale * 0.3f, 0, 0);

            float ringWobble0 = 1.05f + (float)Math.Sin(Projectile.localAI[0] * 0.1f + 0.6f) * 0.01f;
            float ringWobble1 = 1.05f + (float)Math.Sin(Projectile.localAI[0] * 0.1f + 0.3f) * 0.01f;
            Vector2 middleRingOff = new Vector2(0, 50 * lensPower).RotatedBy(lensAngle - 0.3f);

            Main.EntitySpriteDraw(ring, Projectile.Center + middleRingOff - Main.screenPosition, null, bloomColor * 0.05f, sparkRotation + MathHelper.PiOver2 + 0.2f, ring.Size() * 0.5f, new Vector2(1f, 1.05f) * Projectile.scale * ringWobble0, 0, 0);
            Main.EntitySpriteDraw(ring, Projectile.Center - Main.screenPosition, null, bloomColor * 0.1f, sparkRotation + MathHelper.PiOver2 + 0.2f, ring.Size() * 0.5f, new Vector2(1f, 1.05f) * Projectile.scale * 1.4f * ringWobble1, 0, 0);

            Vector2 bottomRingOff = new Vector2(0, 40).RotatedBy(lensAngle + 0.2f) * lensPower;
            Main.EntitySpriteDraw(ring, Projectile.Center + bottomRingOff - Main.screenPosition, null, bloomColor * 0.1f, sparkRotation + MathHelper.PiOver2 + 0.2f, ring.Size() * 0.5f, new Vector2(1f, 1.05f) * Projectile.scale * 0.4f, 0, 0);
            Vector2 topRingOff = new Vector2(0, -60).RotatedBy(lensAngle) * lensPower;
            Main.EntitySpriteDraw(ring, Projectile.Center + topRingOff - Main.screenPosition, null, bloomColor * 0.1f, sparkRotation + MathHelper.PiOver2 + 0.2f, ring.Size() * 0.5f, new Vector2(1f, 1.05f) * Projectile.scale * 0.8f, 0, 0);

            Vector2 topFlareOff = topRingOff * ringWobble0 * 1.1f;
            for (int i = 0; i < 3; i++)
                Main.EntitySpriteDraw(sparkle, Projectile.Center + topFlareOff - Main.screenPosition, null, bloomColor * 0.2f, MathHelper.TwoPi / 3f * i, sparkle.Size() * 0.5f, new Vector2(0.3f, 1.5f), 0, 0);

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, bloomColor * (0.3f / lensPower), Projectile.rotation * 0.5f, glow.Size() * 0.5f, Projectile.scale * 4f, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, bloomColor * (0.08f / lensPower), Projectile.rotation * 0.5f, glow.Size() * 0.5f, Projectile.scale * 8f, 0, 0);

            DrawHitMeSign();

            return false;
        }

        public int signFrameCounter;
        public int signFrame;

        public Vector2 handPosition;
        public float handRotation;
        public int handFrame;
        public int handDir;

        public void UpdateHitMeSign()
        {
            if (signFrameCounter++ > 4)
            {
                signFrame = (signFrame + 1) % 8;
                signFrameCounter = 0;
            }

            Vector2 homePos = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2.7f);

            handDir = homePos.X > Projectile.Center.X - Main.screenPosition.X ? 1 : -1;

            if (Main.getGoodWorld)
            {
                handFrame = 5;
                handPosition = Vector2.SmoothStep(Projectile.Center - Main.screenPosition, homePos, Utils.GetLerpValue(0, 60, Time, true));
                handRotation = MathF.Sin(Time * 0.4f) * Utils.GetLerpValue(45, 50, Time, true) * 0.6f;
            }
            else
            {
                if (Time < 70)
                {
                    handFrame = 1 + (int)(Utils.GetLerpValue(0, 20, Time, true) * 2f);
                    handPosition = Vector2.SmoothStep(Projectile.Center - Main.screenPosition, homePos, Utils.GetLerpValue(0, 60, Time, true));
                    handRotation = MathF.Sin(Time * 0.4f) * Utils.GetLerpValue(60, 55, Time, true);
                }
                else// if (HitCount < 1)
                {
                    handPosition = homePos + (HitCount > 0 ? Main.rand.NextVector2Circular(4, 4) : Vector2.Zero);
                    handRotation = Utils.AngleLerp(handRotation, handPosition.AngleTo(Projectile.Center - Main.screenPosition) + MathF.Sin(Time * 0.3f) * 0.1f + MathHelper.PiOver2, 0.2f);
                    handFrame = 3 + (int)(Utils.GetLerpValue(70, 85, Time, true) * 2);
                }
            }

        }

        public void DrawHitMeSign()
        {
            Color bloomColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.7f, 0) * 0.3f;
            
            float influence = Utils.GetLerpValue(0, 10, Time, true);
            float wobble = (0.7f + MathF.Sin(Time * 0.3f) * 0.1f);

            float handScale = Utils.GetLerpValue(0.5f, 0.9f, Projectile.scale, true);
            float signScale = handScale * Utils.GetLerpValue(40, 60, Time, true);
            Vector2 signPosition = Projectile.Center - Main.screenPosition - new Vector2(0, 100 * Projectile.scale);

            Rectangle hitMeFrame = hitMeSign.Value.Frame(1, 8, 0, signFrame);

            Main.EntitySpriteDraw(hitMeSign.Value, signPosition, hitMeFrame, new Color(255, 255, 255, 128) * influence, MathF.Sin(Time * 0.15f) * 0.1f, hitMeFrame.Size() * 0.5f, signScale, 0, 0);
            for (int i = 0; i < 8; i++)
            {
                Vector2 offset = new Vector2(2, 0).RotatedBy(MathHelper.TwoPi / 8f * i);
                Main.EntitySpriteDraw(hitMeSign.Value, signPosition + offset, hitMeFrame, bloomColor * influence * wobble, MathF.Sin(Time * 0.15f) * 0.1f, hitMeFrame.Size() * 0.5f, signScale, 0, 0);
            }

            Rectangle pointerFrame = hitMeHand.Value.Frame(1, 6, 0, handFrame);

            SpriteEffects handEffect = handDir < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(hitMeHand.Value, handPosition, pointerFrame, new Color(255, 255, 255, 128) * influence, handRotation, pointerFrame.Size() * new Vector2(0.5f, 0.9f), handScale, handEffect, 0);
            for (int i = 0; i < 8; i++)
            {
                Vector2 offset = new Vector2(2, 0).RotatedBy(MathHelper.TwoPi / 8f * i);
                Main.EntitySpriteDraw(hitMeHand.Value, handPosition + offset, pointerFrame, bloomColor * influence * wobble, handRotation, pointerFrame.Size() * new Vector2(0.5f, 0.9f), handScale, handEffect, 0);
            }
        }
    }
}
