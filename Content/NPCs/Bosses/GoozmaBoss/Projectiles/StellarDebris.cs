using System;
using System.Linq;
using CalamityHunt.Common.Graphics.RenderTargets;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles
{
    public class StellarDebris : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float TopTime => ref Projectile.ai[1];
        public ref float Owner => ref Projectile.ai[2];

        public override void OnSpawn(IEntitySource source)
        {
            WeightedRandom<int> typeOfStarBit = new WeightedRandom<int>();
            typeOfStarBit.Add(0, 0.2f);
            typeOfStarBit.Add(1, 0.3f);
            typeOfStarBit.Add(2, 0.3f);
            typeOfStarBit.Add(3, 0.6f);
            Projectile.frame = typeOfStarBit;
            Projectile.scale *= Main.rand.NextFloat(0.7f, 1.2f);
            Projectile.rotation = Main.rand.NextFloat(-0.3f, 0.3f);
            Projectile.direction = Main.rand.NextBool() ? -1 : 1;
            TopTime = 5;

            Owner = -1;
        }

        public override void AI()
        {
            if (Owner < 0) {
                Projectile.active = false;
                return;
            }
            else if (!Main.npc[(int)Owner].active || Main.npc[(int)Owner].type != ModContent.NPCType<StellarGeliath>()) {
                Projectile.active = false;
                return;
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[(int)Owner].GetTargetData().Center).SafeNormalize(Vector2.Zero) * 15, 0.0001f);

            foreach (Projectile otherBit in Main.projectile.Where(n => n.active && n.type == Type && n.whoAmI != Projectile.whoAmI && n.Distance(Projectile.Center) < 200)) {
                otherBit.velocity += otherBit.DirectionFrom(Projectile.Center).SafeNormalize(Vector2.Zero) * 0.1f;
                Projectile.velocity += Projectile.DirectionFrom(otherBit.Center).SafeNormalize(Vector2.Zero) * 0.1f;
            }

            if (Time % TopTime == 1) {
                Projectile.direction *= -1;
                TopTime = Main.rand.Next(80, 140);
            }

            if (Time % TopTime > TopTime * 0.7f) {
                Projectile.velocity = Projectile.velocity.RotatedBy(0.05f * Projectile.direction);
            }
            else {
                Projectile.velocity = Projectile.velocity.RotatedBy((Projectile.velocity.ToRotation() - Projectile.AngleTo(Main.npc[(int)Owner].GetTargetData().Center)) * 0.01f * Projectile.direction);
            }

            Projectile.velocity *= 0.99f;

            if (Time > 500) {
                Projectile.damage = 0;
                Projectile.scale = Utils.GetLerpValue(40, 120, Projectile.Distance(Main.npc[(int)Owner].Center), true);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[(int)Owner].Center).SafeNormalize(Vector2.Zero) * Projectile.Distance(Main.npc[(int)Owner].GetTargetData().Center) * 0.2f * Utils.GetLerpValue(500, 600, Time, true), 0.2f * Utils.GetLerpValue(500, 600, Time, true)).RotatedBy(0.01f * Projectile.direction);
            }

            if (Time > 600 || Projectile.scale < 0.1f) {
                Projectile.Kill();
            }

            CosmosMetaball.particles.Add(Particle.Create<SmokeSplatterMetaball>(particle => {
                particle.position = Projectile.Center + Projectile.velocity * 2f + Main.rand.NextVector2Circular(24, 24);
                particle.velocity = Main.rand.NextVector2Circular(5, 5) + Projectile.velocity * 0.2f;
                particle.scale = Main.rand.NextFloat(1f, 2f) * Projectile.scale;
                particle.maxTime = Main.rand.Next(10, 30);
                particle.color = Color.White;
                particle.fadeColor = Color.White;
            }));

            if (Main.rand.NextBool(50)) {
                CalamityHunt.particles.Add(Particle.Create<PrettySparkle>(particle => {
                    particle.position = Projectile.Center + Main.rand.NextVector2Circular(24, 24) * Projectile.scale + Projectile.velocity;
                    particle.velocity = Main.rand.NextVector2Circular(3, 3);
                    particle.scale = Main.rand.NextFloat(0.2f, 1.2f) * Projectile.scale;
                    particle.color = new Color(30, 15, 10, 0);
                }));
            }
            Projectile.rotation += Projectile.velocity.Length() * Projectile.direction * 0.0015f;

            //Projectile.frameCounter++;
            //if (Projectile.frameCounter > 15)
            //{
            //    Projectile.frameCounter = 0;
            //    Projectile.frame = (Projectile.frame + 1) % 3;
            //}

            Time++;
            Projectile.localAI[0]++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 60) {
                return base.Colliding(projHitbox, targetHitbox);
            }
            return false;
        }

        public static Texture2D auraTexture;

        public override void Load()
        {
            auraTexture = ModContent.Request<Texture2D>(Texture + "Aura", AssetRequestMode.ImmediateLoad).Value;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = AssetDirectory.Textures.Glow[0].Value;
            Rectangle frame = texture.Frame(4, 1, Projectile.frame, 0);

            for (int i = 0; i < 3; i++) {
                float wobble = (float)Math.Sin(Projectile.localAI[0] * 0.15f + i * 1.3f) * 0.05f;
                float rotation = Projectile.rotation * (0.5f + i * 0.1f);
                Main.EntitySpriteDraw(auraTexture, Projectile.Center - Main.screenPosition, auraTexture.Frame(), new Color(20, 100, 150, 0) * 0.5f * (i > 0 ? 0.1f / i : 1f), rotation, auraTexture.Size() * 0.5f, Projectile.scale * (0.8f + wobble + i * 0.3f), 0, 0);
            }
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), new Color(10, 30, 110, 0), Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 2f, 0, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, new Color(70, 40, 35, 0), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * 1.3f, 0, 0);

            return false;
        }
    }
}
