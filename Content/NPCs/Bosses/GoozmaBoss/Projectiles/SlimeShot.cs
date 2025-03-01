using System;
using System.Linq;
using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Systems.ParticlesOld;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles
{
    public class SlimeShot : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 140;
            initializedLocal = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            
        }

        public ref float Time => ref Projectile.ai[0];

        private bool initializedLocal = false;

        public override void AI()
        {
            if (!initializedLocal) {
                Projectile.localAI[0] = Main.rand.Next(40);
                Projectile.frame = Main.rand.Next(3);

                initializedLocal = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(0, 15, Time, true) * Utils.GetLerpValue(0, 20, Projectile.timeLeft, true));
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, 0.001f + Utils.GetLerpValue(50, 165, Time, true) * 0.05f) * (1f + 0.115f * Utils.GetLerpValue(50, 0, Time, true));

            int target = -1;
            if (Main.player.Any(n => n.active && !n.dead)) {
                target = Main.player.First(n => n.active && !n.dead).whoAmI;
            }

            if (target > -1) {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.player[target].MountedCenter).SafeNormalize(Vector2.Zero) * Projectile.oldVelocity.Length(), 0.015f);
            }

            if (Main.rand.NextBool(10)) {
                var hue = ParticleBehavior.NewParticle(ModContent.GetInstance<ChromaticEnergyDust2Behavior>(), Projectile.Center + Main.rand.NextVector2Circular(20, 20), Projectile.velocity * 0.4f, Color.White, 1f);
                hue.Add(new ParticleData<float> { Value = Projectile.localAI[0] });
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8) {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 3;
            }

            Projectile.localAI[0]++;
            Time++;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++) {
                var hue = ParticleBehavior.NewParticle(ModContent.GetInstance<ChromaticEnergyDust2Behavior>(), Projectile.Center, Main.rand.NextVector2Circular(6, 6), Color.White, 1f);
                hue.Add(new ParticleData<float> { Value = Projectile.localAI[0] });

                if (!Main.rand.NextBool(2)) {
                    Dust.NewDustPerfect(Projectile.Center, 4, Main.rand.NextVector2Circular(3, 3), 100, Color.Black, 1.5f).noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Microsoft.Xna.Framework.Graphics.Texture2D texture = TextureAssets.Projectile[Type].Value;
            Microsoft.Xna.Framework.Graphics.Texture2D glow = AssetDirectory.Textures.Glow[0].Value;
            Vector2 squishFactor = new Vector2(1f - Projectile.velocity.Length() * 0.0045f, 1f + Projectile.velocity.Length() * 0.0075f);

            Rectangle baseFrame = texture.Frame(3, 3, 0, Projectile.frame);
            Rectangle glowFrame = texture.Frame(3, 3, 1, Projectile.frame);
            Rectangle outlineFrame = texture.Frame(3, 3, 2, Projectile.frame);

            Color bloomColor = (new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]) * Utils.GetLerpValue(0, 25, Time, true)) with { A = 0 };

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, outlineFrame, bloomColor, Projectile.rotation, outlineFrame.Size() * 0.5f, Projectile.scale * 1.15f * squishFactor, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, outlineFrame, new Color(200, 200, 200, 0) * Utils.GetLerpValue(15, 30, Time, true), Projectile.rotation, outlineFrame.Size() * 0.5f, Projectile.scale * 1.1f * squishFactor, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center + Projectile.velocity * 0.2f - Main.screenPosition, null, bloomColor * 0.15f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 2f * squishFactor, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, baseFrame, lightColor * Utils.GetLerpValue(15, 30, Time, true), Projectile.rotation, baseFrame.Size() * 0.5f, Projectile.scale * squishFactor, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, glowFrame, bloomColor, Projectile.rotation, glowFrame.Size() * 0.5f, Projectile.scale * squishFactor, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, glowFrame, bloomColor * 0.8f, Projectile.rotation, glowFrame.Size() * 0.5f, Projectile.scale * 1.05f * squishFactor, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center + Projectile.velocity * 0.1f - Main.screenPosition, null, bloomColor * 0.05f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 1.2f * squishFactor, 0, 0);

            return false;
        }
    }
}
