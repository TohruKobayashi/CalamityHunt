using System;
using System.Collections.Generic;
using System.Linq;
using CalamityHunt.Common.GlobalNPCs;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged;

public class AntiMassBioBall : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.timeLeft = 200;
        Projectile.tileCollide = true;
        Projectile.manualDirectionChange = true;
        Projectile.localNPCHitCooldown = 10;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.ignoreWater = true;
        Projectile.penetrate = 2;
    }

    public const float MAX_RANGE = 500;

    public ref float Time => ref Projectile.ai[0];

    public override void AI()
    {
        Projectile.velocity.Y += 0.001f;
        Projectile.velocity *= 0.99f;

        Projectile.scale = Utils.GetLerpValue(-15, 10, Time, true);

        if (Main.myPlayer == Projectile.owner) {
            if (Time > 10 && Time % 8 == 0) {

                List<int> doomedTargets = new List<int>();
                List<int> newTargets = new List<int>();
                bool anyTargets = false;
                foreach (NPC npc in Main.npc.Where(n => n.active && n.CanBeChasedBy(Projectile) && Projectile.Distance(n.Center) < MAX_RANGE)) {
                    if (npc.GetGlobalNPC<DoomedNPC>().doomCount > 0) {
                        doomedTargets.Add(npc.whoAmI);
                        anyTargets = true;
                    }
                    else {
                        newTargets.Add(npc.whoAmI);
                        anyTargets = true;
                    }
                }
                if (anyTargets) {
                    int nextTarget = -1;
                    if (newTargets.Count > 0) {
                        nextTarget = Main.rand.Next(newTargets);
                    }
                    else {
                        nextTarget = Main.rand.Next(doomedTargets);
                    }

                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Main.npc[nextTarget].Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 15f, ModContent.ProjectileType<AntiMassBioStrike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, nextTarget, Projectile.whoAmI);
                    Projectile.netUpdate = true;
                }
            }
        }

        Color color = Color.Lerp(AntiMassAccumulatorProj.MainColor, Color.Turquoise, (!Main.rand.NextBool(10)).ToInt()) with { A = 40 };
        Dust sparks = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), 278, Projectile.velocity.RotatedByRandom(0.2f) * 0.5f, 0, color * 0.7f, Main.rand.NextFloat(0.6f));
        sparks.noGravity = true;

        Time++;

        for (int i = 0; i < 3; i++) {
            CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                particle.position = Projectile.Center + Main.rand.NextVector2Circular(20, 20).RotatedBy(Projectile.rotation) * Projectile.scale;
                particle.velocity = Main.rand.NextVector2Circular(6, 6);
                particle.scale = Main.rand.NextFloat(0.2f, 0.9f) * Projectile.scale;
                particle.color = Color.Lerp(AntiMassAccumulatorProj.MainColor, Color.Turquoise, (!Main.rand.NextBool((int)(Time / 30f + 2))).ToInt()) with { A = 0 };
                particle.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                particle.anchor = () => Projectile.velocity;
            }));
        }

        Lighting.AddLight(Projectile.Center, Color.MediumTurquoise.ToVector3() * 0.6f);

        HandleSound();
    }

    public LoopingSound auraSound;

    public void HandleSound()
    {
        SoundStyle sound = AssetDirectory.Sounds.Weapons.ElectricLoop;

        auraSound ??= new LoopingSound(sound, new ProjectileAudioTracker(Projectile).IsActiveAndInGame);

        auraSound.PlaySound(() => Projectile.Center, () => Math.Max(0f, 0.33f - Projectile.Distance(Main.LocalPlayer.Center) * 0.0003f), () => -0.5f);
    }

    public override void OnKill(int timeLeft)
    {
        auraSound.StopSound();

        SoundStyle deathSound = SoundID.DD2_KoboldIgnite;
        deathSound.MaxInstances = 0;
        deathSound.PitchVariance = 0.1f;
        SoundEngine.PlaySound(deathSound.WithPitchOffset(-0.2f), Projectile.Center);

        for (int i = 0; i < 40; i++) {
            CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                particle.maxTime = Main.rand.Next(10, 18);
                particle.position = Projectile.Center + Main.rand.NextVector2Circular(10, 10).RotatedBy(Projectile.rotation) * Projectile.scale;
                particle.velocity = Main.rand.NextVector2Circular(36, 36) * (1.5f / particle.maxTime);
                particle.scale = Main.rand.NextFloat(0.5f, 1.5f) * Projectile.scale;
                particle.color = Color.Lerp(AntiMassAccumulatorProj.MainColor, Color.MediumAquamarine, (!Main.rand.NextBool((int)(Time / 30f + 8))).ToInt()) with { A = 0 };
                particle.rotation = particle.velocity.ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f);
                particle.anchor = () => Projectile.velocity * 0.01f;
                particle.flickerSpeed = 2.5f;
            }));

            if (!Main.rand.NextBool(4)) {
                Color color = Color.Lerp(AntiMassAccumulatorProj.MainColor, Color.Turquoise, (!Main.rand.NextBool(10)).ToInt()) with { A = 40 };
                Dust sparks = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), 278, Main.rand.NextVector2Circular(5, 5), 0, color * 0.7f, Main.rand.NextFloat());
                sparks.noGravity = true;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.GetGlobalNPC<DoomedNPC>().doomCount = 240;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Texture2D glow = AssetDirectory.Textures.Glow[0].Value;
        Texture2D sparkle = AssetDirectory.Textures.Sparkle.Value;

        float scaleWobble = Projectile.scale + MathF.Sin(Projectile.timeLeft / 1.2f) * 0.2f;

        Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), Color.Black * 0.4f, 0, glow.Size() * 0.5f, Projectile.scale * 1.5f, 0, 0);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.PaleTurquoise with { A = 200 }, 0, texture.Size() * 0.5f, (Projectile.scale + scaleWobble) / 2f, 0, 0);
        Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), (Color.Cyan * 0.3f) with { A = 0 }, 0, glow.Size() * 0.5f, scaleWobble, 0, 0);
        Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), (Color.MediumTurquoise * 0.2f) with { A = 0 }, 0, glow.Size() * 0.5f, Projectile.scale * 1.5f, 0, 0);
        Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, sparkle.Frame(), (Color.MediumTurquoise * 0.3f) with { A = 0 }, MathHelper.PiOver2, sparkle.Size() * 0.5f, Projectile.scale * new Vector2(0.8f, 2f), 0, 0);

        return false;
    }
}
