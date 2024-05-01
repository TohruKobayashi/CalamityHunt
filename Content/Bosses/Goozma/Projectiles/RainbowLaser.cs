using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Arch.Core.Extensions;
using CalamityHunt.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class RainbowLaser : ModProjectile, IDieWithGoozma
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 200;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Owner => ref Projectile.ai[1];
        public ref float Speed => ref Projectile.ai[2];

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(0, 2, Time, true) * Utils.GetLerpValue(0, 50, Projectile.timeLeft, true));

            if (Owner < 0)
            {
                Projectile.active = false;
                return;
            }
            else if (!Main.npc[(int)Owner].active || (Main.npc[(int)Owner].type != ModContent.NPCType<Goozmite>() && Main.npc[(int)Owner].type != ModContent.NPCType<Goozma>()))
            {
                Projectile.active = false;
                return;
            }

            if (Time < 1)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[(int)Owner].GetTargetData().Center) * Projectile.oldVelocity.Length(), 0.05f);

                Projectile.Center = Main.npc[(int)Owner].Center;
                Projectile.oldVelocity = Projectile.velocity;
                Projectile.direction = Main.rand.NextBool() ? -1 : 1;
                Speed = Projectile.Distance(Main.npc[(int)Owner].GetTargetData().Center) * 0.015f * Main.rand.NextFloat(0.7f, 1.3f);
            }

            if (Time == 1)
            {
                SoundStyle shootSound = AssetDirectory.Sounds.Goozma.GoozmiteShoot;
                SoundEngine.PlaySound(shootSound.WithVolumeScale(0.5f), Projectile.Center);
            }

            int target = -1;
            if (Main.player.Any(n => n.active && !n.dead))
                target = Main.player.First(n => n.active && !n.dead).whoAmI;

            if (target > -1 && Time >= 0)
            {
                Projectile.extraUpdates = Main.npc[(int)Owner].type == ModContent.NPCType<Goozma>() ? 10 : 7;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Speed * Utils.GetLerpValue(-10, 150, Projectile.timeLeft, true);
                Projectile.velocity = Projectile.velocity.RotatedBy(0.0015f * Projectile.direction);
            }

            if (Main.rand.NextBool(20))
            {
                var hue = ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Projectile.Center, Projectile.velocity * (Time / 150f), Color.White, 1f);
                hue.Add(new ParticleData<float> { Value = Projectile.localAI[0] });
            }

            Time++;
            Projectile.localAI[0]++;
            Projectile.localAI[1]++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 1)
                return base.Colliding(projHitbox, targetHitbox);

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;          
            Texture2D bloom = AssetDirectory.Textures.Glow.Value;
            Texture2D ray = AssetDirectory.Textures.GlowRay.Value;

            Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            glowColor.A = 0;

            float tellStrength = (float)Math.Sqrt(Utils.GetLerpValue(0, 40, Projectile.localAI[1], true));
            Vector2 telegraphScale = new Vector2(3f + tellStrength * 3f, (1f - tellStrength) * 0.2f);
            Main.EntitySpriteDraw(ray, Main.npc[(int)Owner].Center - Main.screenPosition, ray.Frame(), glowColor * 0.3f, Projectile.rotation, ray.Size() * new Vector2(0f, 0.5f), telegraphScale, 0, 0);
            Main.EntitySpriteDraw(ray, Main.npc[(int)Owner].Center - Main.screenPosition, ray.Frame(), glowColor * Utils.GetLerpValue(0, 20, Projectile.localAI[1], true), Projectile.rotation, ray.Size() * new Vector2(0f, 0.5f), telegraphScale * new Vector2(1f, 0.4f), 0, 0);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float prog = i / (float)ProjectileID.Sets.TrailCacheLength[Type];
                Color trailColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] - i * 5) * (1f - prog);
                trailColor.A = 0;
                float addRot = (-0.003f * i * Projectile.direction);
                Vector2 oldPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Projectile.velocity.RotatedBy(addRot) * i * Utils.GetLerpValue(0, 60, Time, true); //Projectile.Distance(Projectile.oldPos[i] + Projectile.Size * 0.5f) * 0.1f;
                Main.EntitySpriteDraw(texture, oldPos - Main.screenPosition, texture.Frame(), trailColor * 0.2f, Projectile.oldRot[i] + MathHelper.PiOver2 + addRot, texture.Size() * new Vector2(0.5f, 0.15f), Projectile.scale * new Vector2(2f, 4f) * (1f - (float)i / ProjectileID.Sets.TrailCacheLength[Type]), 0, 0);
                Main.EntitySpriteDraw(texture, oldPos - Main.screenPosition, texture.Frame(), trailColor, Projectile.oldRot[i] + MathHelper.PiOver2 + addRot, texture.Size() * new Vector2(0.5f, 0.05f), Projectile.scale * new Vector2(0.5f, Projectile.position.Distance(Projectile.oldPos[i]) * 0.2f) * (1.2f - prog), 0, 0);
                Main.EntitySpriteDraw(bloom, oldPos - Main.screenPosition, bloom.Frame(), trailColor * 0.05f, Projectile.oldRot[i] + addRot, bloom.Size() * 0.5f, Projectile.scale * new Vector2(Projectile.position.Distance(Projectile.oldPos[i]) * 0.2f, 1f) * (1f - prog), 0, 0);
                Main.EntitySpriteDraw(bloom, oldPos - Main.screenPosition, bloom.Frame(), trailColor * 0.1f, Projectile.oldRot[i] + addRot, bloom.Size() * 0.5f, Projectile.scale * new Vector2(Projectile.position.Distance(Projectile.oldPos[i]) * 0.2f, 0.5f) * (1f - prog), 0, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), glowColor * 0.5f, Projectile.rotation + MathHelper.PiOver2, texture.Size() * new Vector2(0.5f, 0.2f), Projectile.scale * new Vector2(2f, 3f), 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), glowColor * 0.5f, Projectile.rotation, texture.Size() * new Vector2(0.5f, 0.25f), Projectile.scale * 2, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(255, 255, 255, 0), Projectile.rotation + MathHelper.PiOver2, texture.Size() * new Vector2(0.5f, 0.15f), Projectile.scale * new Vector2(1f, 4f) * 0.7f, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(255, 255, 255, 0), Projectile.rotation, texture.Size() * new Vector2(0.5f, 0.25f), Projectile.scale * new Vector2(0.6f, 1.2f), 0, 0);
            Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, bloom.Frame(), glowColor * 0.1f, Projectile.rotation, bloom.Size() * 0.5f, Projectile.scale * 4f, 0, 0);
            Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, bloom.Frame(), glowColor * 0.5f, Projectile.rotation, bloom.Size() * 0.5f, Projectile.scale * 1.5f, 0, 0);

            return false;
        }
    }
}
