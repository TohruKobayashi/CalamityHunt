using System;
using System.Linq;
using CalamityHunt.Common.Graphics.RenderTargets;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Common.Utilities.Interfaces;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles
{
    public class StellarBlackHole : ModProjectile, ISubjectOfNPC<Goozma>, ISubjectOfNPC<StellarGeliath>
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];
        public ref float Owner => ref Projectile.ai[2];

        public override void OnSpawn(IEntitySource source)
        {
            Owner = -1;
        }

        public override void AI()
        {
            if (Time == 0) {
                Projectile.netUpdate = true;

                SoundStyle invocation = AssetDirectory.Sounds.GoozmaMinions.StellarBlackHoleSummon;
                SoundEngine.PlaySound(invocation.WithVolumeScale(1.5f), Projectile.Center);
            }

            if (Owner < 0) {
                Projectile.velocity *= 0.9f;
                Projectile.scale *= 0.8f;
                if (Projectile.scale < 0.01f) {
                    Projectile.Kill();
                    return;
                }

            }
            else if (!Main.npc[(int)Owner].active || Main.npc[(int)Owner].type != ModContent.NPCType<StellarGeliath>()) {
                Projectile.velocity *= 0.9f;
                Projectile.scale *= 0.8f;
                if (Projectile.scale < 0.01f) {
                    Projectile.Kill();
                }

                return;
            }

            Projectile.scale = (float)Math.Sqrt(MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(10, MaxTime * 0.2f, Time, true) * Utils.GetLerpValue(MaxTime, MaxTime - 20, Time, true))) * 0.75f;
            Projectile.velocity = Projectile.DirectionTo(Main.npc[(int)Owner].Center).SafeNormalize(Vector2.Zero) * Projectile.Distance(Main.npc[(int)Owner].Center) * 0.2f;

            HandleSound();

            Time++;
            if (Time > MaxTime) {
                Projectile.Kill();
            }

            Projectile.localAI[0]++;

            if (Math.Abs(Projectile.velocity.X) > 2) {
                Projectile.direction = Math.Sign(Projectile.velocity.X);
            }

            Projectile.rotation += 0.1f * Projectile.direction;

            if (Time < 50 || Time > 520) {
                for (int i = 0; i < 6; i++) {
                    CosmosMetaball.particles.Add(Particle.Create<SmokeSplatterMetaball>(particle => {
                        particle.position = Projectile.Center;
                        particle.velocity = Main.rand.NextVector2Circular(90, 90) * Utils.GetLerpValue(0, 50, Time, true);
                        particle.scale = Main.rand.NextFloat(20f, 30f) * Utils.GetLerpValue(-30, 40, Time, true);
                        particle.maxTime = Main.rand.Next(70, 100);
                        particle.color = Color.White;
                        particle.fadeColor = Color.White;
                        particle.anchor = () => Projectile.velocity * 0.66f;
                    }));
                }
            }
        }

        public LoopingSound holeSound;
        public LoopingSound windSound;

        public float volume;
        public float pitch;

        public void HandleSound()
        {
            volume = Math.Clamp(1f + Projectile.velocity.Length() * 0.0001f - Main.LocalPlayer.Distance(Projectile.Center) * 0.0002f, 0, 1) * (0.6f + Projectile.scale * 0.5f);
            pitch = (float)Math.Sqrt(Utils.GetLerpValue(0, MaxTime * 0.55f, Time, true) * Utils.GetLerpValue(MaxTime, MaxTime * 0.99f, Time, true)) - 1f;

            holeSound ??= new LoopingSound(AssetDirectory.Sounds.GoozmaMinions.StellarBlackHoleLoop, new ProjectileAudioTracker(Projectile).IsActiveAndInGame);
            windSound ??= new LoopingSound(AssetDirectory.Sounds.Goozma.WindLoop, new ProjectileAudioTracker(Projectile).IsActiveAndInGame);

            holeSound.PlaySound(() => Projectile.Center, () => volume * 1f, () => pitch);
            windSound.PlaySound(() => Projectile.Center, () => volume * 0.5f, () => 0.1f + pitch * 0.3f);
        }

        public override void OnKill(int timeLeft)
        {
            holeSound.StopSound();
            windSound.StopSound();
            Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].Deactivate();
        }

        public override void Load()
        {
            On_Main.UpdateAudio += QuietMusic;
        }

        private void QuietMusic(On_Main.orig_UpdateAudio orig, Main self)
        {
            orig(self);

            if (Main.projectile.Any(n => n.active && n.type == ModContent.ProjectileType<StellarBlackHole>())) {
                Projectile projectile = Main.projectile.FirstOrDefault(n => n.active && n.type == ModContent.ProjectileType<StellarBlackHole>());

                for (int i = 0; i < Main.musicFade.Length; i++) {
                    float volume = Main.musicFade[i] * Main.musicVolume * (1f - projectile.scale * 0.3f);
                    float tempFade = Main.musicFade[i];
                    Main.audioSystem.UpdateCommonTrackTowardStopping(i, volume, ref tempFade, Main.musicFade[i] > 0.15f);
                    Main.musicFade[i] = tempFade;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = AssetDirectory.Textures.Glow[1].Value;
            Texture2D ring = AssetDirectory.Textures.ShockRing.Value;

            float intensity = (float)Math.Sqrt(Utils.GetLerpValue(0.2f, 0.6f, Projectile.scale, true));

            float scaleWobble = MathF.Sin(Main.GlobalTimeWrappedHourly * 40f) * 0.033f;
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), (Color.Gold * 0.8f) with { A = 0 }, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 8f + scaleWobble, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), (Color.Red * 0.6f) with { A = 0 }, -Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 20f + scaleWobble * 2f, 0, 0);

            for (int i = 0; i < 7; i++) {
                float exist = Utils.GetLerpValue(20, 60, Time + i * 3f, true) * intensity;
                float comeIn = (float)Math.Pow(1f - (Time + i / 7f * 70) % 70 / 70f, 2f);
                Main.EntitySpriteDraw(ring, Projectile.Center - Main.screenPosition, ring.Frame(), Color.Black * Utils.PingPongFrom01To010(comeIn) * exist, (Time + i * 5f) * 0.3f, ring.Size() * 0.5f, 150f * comeIn, 0, 0);
                Main.EntitySpriteDraw(ring, Projectile.Center - Main.screenPosition, ring.Frame(), Color.Black * Utils.PingPongFrom01To010(comeIn) * exist, (Time + i * 5f) * 0.3f, ring.Size() * 0.5f, 80f * comeIn, 0, 0);
            }

            if (Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].Active) {
                Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].GetShader()
                    .UseColor(Color.Orange)
                    .UseTargetPosition(Projectile.Center)
                    .UseProgress(Main.GlobalTimeWrappedHourly * 0.1f % 1f)
                    .UseIntensity(intensity);
                Effect shader = Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].GetShader().Shader;
                shader.Parameters["uPower"].SetValue(24f * intensity);
                shader.Parameters["uSize"].SetValue((0.125f + MathF.Sin(Main.GlobalTimeWrappedHourly * 32f) * 0.002f) * intensity);
                shader.Parameters["uAngle"].SetValue((-40f + MathF.Sin(Main.GlobalTimeWrappedHourly * 32f) * 2f) * intensity);
            }
            else {
                Filters.Scene.Activate("HuntOfTheOldGods:StellarBlackHole", Projectile.Center);
            }

            return false;
        }
    }
}
