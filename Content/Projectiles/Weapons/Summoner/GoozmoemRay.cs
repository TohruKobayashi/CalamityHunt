using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Arch.Core.Extensions;
using CalamityHunt.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Summoner
{
    public class GoozmoemRay : ModProjectile
    {
        public override string Texture => $"{Mod.Name}/Assets/Textures/SharpSpark";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 80;
            Projectile.manualDirectionChange = true;
            Projectile.hide = true;
        }

        public ref float StartAngle => ref Projectile.ai[0];
        public ref float EndAngle => ref Projectile.ai[1];

        public override void AI()
        {
            if (Projectile.ai[2] < 0)
            {
                Projectile.Kill();
                return;
            }

            Projectile host = Main.projectile[(int)Projectile.ai[2]];

            if (!host.active || host.type != ModContent.ProjectileType<Goozmoem>() || host.owner != Projectile.owner)
            {
                Projectile.Kill();
                return;
            }

            Projectile.rotation = Utils.AngleLerp(StartAngle, EndAngle, 1f - Projectile.timeLeft / 80f);
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.Center = host.Center + new Vector2(0, -4).RotatedBy(host.rotation) + new Vector2(5, 0).RotatedBy(Projectile.rotation);
            host.direction = Projectile.direction;
            host.velocity *= 0.85f;
            (host.ModProjectile as Goozmoem).eyeOffset = new Vector2(5, 0).RotatedBy(Projectile.rotation);

            Projectile.localAI[0]++;

            if (Projectile.localAI[0] % 3 == 0)
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2CircularEdge(1, 1), 2f, 5, 7, 1000));

            for (int i = 0; i < 15; i++)
            {
                float grow = Utils.GetLerpValue(80, 70, Projectile.timeLeft, true) * Utils.GetLerpValue(-15, 25, Projectile.timeLeft, true);
                float progress = Main.rand.NextFloat(900);
                Color color = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] + (progress / 1000f) * 60) * grow * Utils.GetLerpValue(0, 25, Projectile.timeLeft, true);
                color.A = 0;
                Vector2 position = Projectile.Center + new Vector2(progress, 0).RotatedBy(Utils.AngleLerp(StartAngle, EndAngle, 1f - (Projectile.timeLeft - 5) / 80f));
                var smoke = ParticleBehavior.NewParticle(ModContent.GetInstance<CosmicSmokeParticleBehavior>(), position, Projectile.velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(20f, 25f), color, (1.5f + (progress / 1222f)) * grow * Utils.GetLerpValue(-15, 25, Projectile.timeLeft, true));
                smoke.Add(new ParticleDrawBehindEntities());
            }

            //HandleSound();
        }

        public LoopingSound raySound;
        public float volume;
        public float pitch;

        public void HandleSound()
        {
            volume = 0.7f;
            pitch = 0.5f - Projectile.timeLeft / 80f / 3f;

            if (raySound == null)
                raySound = new LoopingSound(AssetDirectory.Sounds.Goozma.FusionRayLoop, new ProjectileAudioTracker(Projectile).IsActiveAndInGame);

            raySound.Update(() => Projectile.Center, () => volume, () => pitch);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float grow = Utils.GetLerpValue(80, 70, Projectile.timeLeft, true) * Utils.GetLerpValue(-15, 25, Projectile.timeLeft, true);
            return targetHitbox.IntersectsConeFastInaccurate(Projectile.Center, 1100f * (0.5f + grow * 0.5f), Projectile.rotation, 0.3f * grow);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = AssetDirectory.Textures.Extras.FusionRay[0].Value;
            Texture2D textureBits = AssetDirectory.Textures.Extras.FusionRay[1].Value;
            Texture2D textureGlow = AssetDirectory.Textures.Extras.FusionRay[2].Value;
            Texture2D textureSecond = AssetDirectory.Textures.Extras.FusionRay[3].Value;

            Color startColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            startColor.A = 0;

            Vector2[] positions = new Vector2[1000];
            float[] rotations = new float[1000];
            for (int i = 0; i < 1000; i++)
            {
                positions[i] = Projectile.Center + new Vector2(1000 * (i / 1000f), 0).RotatedBy(Projectile.rotation);
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
            lightningEffect.Parameters["uFreq"].SetValue(1f);
            lightningEffect.CurrentTechnique.Passes[0].Apply();
            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            return false;
        }

        public Color StripColor(float progress)
        {
            Color color = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] + progress * 60f);
            color.A = 0;
            return color;
        }

        public float StripWidth(float progress)
        {
            float size = Utils.GetLerpValue(80, 70, Projectile.timeLeft, true) * Utils.GetLerpValue(0, 16, Projectile.timeLeft, true);
            float start = (float)Math.Pow(progress, 0.66f);
            float cap = (float)Math.Cbrt(Utils.GetLerpValue(1f, 0.9f, progress, true));
            return start * cap * size * 200 * (1.1f + (float)Math.Cos(Projectile.localAI[0]) * (0.08f - progress * 0.06f));
        }
    }
}
