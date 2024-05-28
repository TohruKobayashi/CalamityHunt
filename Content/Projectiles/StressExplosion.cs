using System;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles
{
    public class StressExplosion : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<SlimeBomb>().Texture;

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }
        public ref float Time => ref Projectile.ai[0];
        public ref float Stressed => ref Projectile.ai[1];
        public override void AI()
        {
            Color gooColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).Value;
            if (Time == 1 && Stressed == 0) {
                SoundStyle explodeSound = AssetDirectory.Sounds.Goozma.BloatedBlastShoot;
                SoundEngine.PlaySound(explodeSound.WithVolumeScale(0.3f), Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.Center);
                for (int i = 0; i < 5; i++) {
                    Vector2 gooVelocity = new Vector2(1, 0).RotatedBy(MathHelper.TwoPi / 5f * i).RotatedByRandom(0.2f);
                    CalamityHunt.particles.Add(Particle.Create<ChromaticGooBurst>(particle => {
                        particle.position = Projectile.Center + gooVelocity * 2;
                        particle.velocity = gooVelocity;
                        particle.scale = Main.rand.NextFloat(0.5f, 1.5f);
                        particle.color = gooColor;
                    }));
                }
            }
            else if (Time == 1 && Stressed > 0) {
                SoundStyle explodeSound = AssetDirectory.Sounds.Goozma.BloatedBlastShoot;
                SoundEngine.PlaySound(explodeSound.WithVolumeScale(0.3f), Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.Center);
                for (int i = 0; i < 5; i++) {
                    Vector2 gooVelocity = new Vector2(1, 0).RotatedBy(MathHelper.TwoPi / 5f * i).RotatedByRandom(0.2f);
                    CalamityHunt.particles.Add(Particle.Create<ChromaticGooBurst>(particle => {
                        particle.position = Projectile.Center + gooVelocity * 2;
                        particle.velocity = gooVelocity;
                        particle.scale = Main.rand.NextFloat(1f, 1.5f);
                        particle.color = gooColor;
                    }));
                }
            }
            if (Time > 20) {
                Projectile.Kill();
            }

            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Stressed > 0) {
                return targetHitbox.Contains((Projectile.Center + new Vector2(Math.Min(Projectile.Distance(targetHitbox.Center()), 128), 0).RotatedBy(Projectile.Center.AngleTo(targetHitbox.Center()))).ToPoint());
            }

            return targetHitbox.Contains((Projectile.Center + new Vector2(Math.Min(Projectile.Distance(targetHitbox.Center()), 64), 0).RotatedBy(Projectile.Center.AngleTo(targetHitbox.Center()))).ToPoint());
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
