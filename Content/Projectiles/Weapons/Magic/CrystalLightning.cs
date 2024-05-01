using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.UI;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using CalamityHunt.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Magic
{
    public class CrystalLightning : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 3;
            Projectile.extraUpdates = 3;
            Owner = -1;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Owner => ref Projectile.ai[1];
        public ref float Distance => ref Projectile.ai[2];

        public List<Vector2> points;
        public List<Vector2> offsets;
        public List<Vector2> velocities;

        public Vector2 midPoint;
        public Vector2 endPoint;

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void AI()
        {
            if (Owner < 0)
            {
                Projectile.Kill();
                return;
            }
            if (!Main.projectile[(int)Owner].active || (Main.projectile[(int)Owner].type != ModContent.ProjectileType<CrystalGauntletBall>() && Main.projectile[(int)Owner].type != ModContent.ProjectileType<CrystalGauntletBallThrown>()))
            {
                Projectile.Kill();
                return;
            }    
            
            Projectile.Center = Main.projectile[(int)Owner].Center;

            if (Time < 1)
            {
                endPoint = Projectile.Center;
                FindEndpoint();

                Vector2 midOff = Main.rand.NextVector2Circular(10, 350).RotatedBy(Projectile.AngleTo(endPoint)) * (0.1f + Utils.GetLerpValue(0, 2000, Projectile.Distance(endPoint)));
                midPoint = Vector2.Lerp(Projectile.Center, endPoint, 0.7f) + midOff;
            }

            if (Time == 1)
            {
                points = new List<Vector2>();
                offsets = new List<Vector2>();
                velocities = new List<Vector2>();

                points.Add(Projectile.Center);
                Vector2 mid0 = Vector2.Lerp(Projectile.Center, endPoint, 0.3f);
                Vector2 mid1 = Vector2.Lerp(Projectile.Center, endPoint + Main.rand.NextVector2CircularEdge(20, 400).RotatedBy(Projectile.AngleTo(endPoint)), 0.5f);
                Vector2 mid2 = Vector2.Lerp(Projectile.Center, endPoint, 0.9f);
                points = new BezierCurve(new List<Vector2>()
                {
                    Projectile.Center,
                    mid0,
                    mid1,
                    midPoint,
                    mid2

                }).GetPoints(Main.rand.Next(1, 4) + (int)(Projectile.Distance(endPoint) * 0.017f));
                points.Add(endPoint);

                for (int i = 0; i < points.Count; i++)
                {
                    offsets.Add(Main.rand.NextVector2Circular(5, 20).RotatedBy(Projectile.AngleTo(endPoint)) * Utils.GetLerpValue(1, points.Count * 0.3f, i, true) * Utils.GetLerpValue(points.Count - 1, points.Count * 0.7f, i, true));
                    velocities.Add(Projectile.DirectionTo(endPoint).RotatedByRandom(1.5f) * Main.rand.NextFloat(1f, 3f));
                }

                ParticleBehavior.NewParticle(ModContent.GetInstance<CrossSparkleParticleBehavior>(), endPoint, Vector2.Zero, Main.hslToRgb((Projectile.localAI[0] * 0.03f + 0.6f) % 1f, 0.5f, 0.5f, 128), 1f + Main.rand.NextFloat());
            }

            if (Time > 1)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    float prog = Utils.GetLerpValue(1, points.Count, i, true) * Utils.GetLerpValue(points.Count - 1, 0, i, true) * 3f;
                    points[i] += (Projectile.position - Projectile.oldPosition) * (1f - i / (float)points.Count);
                    offsets[i] += (velocities[i] + Main.rand.NextVector2Circular(4, 4)) * prog;
                    velocities[i] *= Main.rand.NextFloat(0.95f, 1f) - i * 0.002f;
                }

                for (int i = 1; i < points.Count; i++)
                {
                    if (Main.rand.NextBool(10))
                    {
                        Vector2 vel = (Projectile.DirectionTo(endPoint).SafeNormalize(Vector2.Zero).RotatedByRandom(0.5f) + offsets[i] * 0.05f) * Main.rand.NextFloat(2f);
                        Color color = Main.hslToRgb((Projectile.localAI[0] * 0.03f + i / (float)points.Count * 0.5f) % 1f, 0.5f, 0.5f, 0);
                        Dust sparkle = Dust.NewDustPerfect(points[i], DustID.PortalBolt, vel, 0, color, Main.rand.NextFloat(2f));
                        sparkle.noGravity = true;
                        sparkle.noLightEmittence = true;
                    }
                }

                if (Main.rand.NextBool(10))
                    ParticleBehavior.NewParticle(ModContent.GetInstance<CrossSparkleParticleBehavior>(), Main.rand.Next(points) + Main.rand.NextVector2Circular(10, 10), Vector2.Zero, Main.hslToRgb((Projectile.localAI[0] * 0.03f + 0.6f) % 1f, 0.5f, 0.5f, 128), 0.1f + Main.rand.NextFloat(0.6f));

                if (Time <= 17 && Time % 5 == 1)
                    ParticleBehavior.NewParticle(ModContent.GetInstance<CrossSparkleParticleBehavior>(), endPoint + Main.rand.NextVector2Circular(40, 40), Vector2.Zero, Main.hslToRgb((Projectile.localAI[0] * 0.03f + 0.6f) % 1f, 0.5f, 0.5f, 128), Main.rand.NextFloat(1.5f));
            }
            if (Time > 40)
                Projectile.Kill();

            Time++;
            Projectile.localAI[0] = Main.projectile[(int)Owner].ai[0];
            Projectile.rotation += 0.2f;
        }

        private void FindEndpoint()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 mouse = Main.MouseWorld;

                if (Distance > 10f)
                {
                    endPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Distance;
                    return;
                }

                if (mouse.Distance(Projectile.Center) > 1100)
                    mouse = Projectile.Center + Projectile.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 1100;

                int target = Projectile.FindTargetWithLineOfSight(1500);
                if (target >= 0)
                {
                    if (Main.npc[target].Distance(Main.MouseWorld) < 600)
                    {
                        endPoint = Main.rand.NextVector2FromRectangle(Main.npc[target].Hitbox);
                        Projectile.netUpdate = true;
                        return;
                    }
                }

                endPoint = mouse + Main.rand.NextVector2Circular(50, 50);
                Projectile.netUpdate = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Rectangle hitbox = new Rectangle((int)Projectile.Center.X - 30, (int)Projectile.Center.Y - 30, 60, 60);

            if (Time > 2 && Time < 30)
            {
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Vector2 center = Vector2.Lerp(points[i], points[i + 1], 0.5f);
                    hitbox.Location = (center - hitbox.Size() * 0.5f).ToPoint();

                    if (targetHitbox.Intersects(hitbox))
                        return true;
                }
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Main.projectile[(int)Owner].owner].GetModPlayer<GoozmaWeaponsPlayer>().CrystalGauntletsCharge += 0.0005f;
            Main.player[Main.projectile[(int)Owner].owner].GetModPlayer<GoozmaWeaponsPlayer>().crystalGauntletsWaitTime = 50;

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Main.player[Main.projectile[(int)Owner].owner].GetModPlayer<GoozmaWeaponsPlayer>().CrystalGauntletsCharge += 0.0005f;
            Main.player[Main.projectile[(int)Owner].owner].GetModPlayer<GoozmaWeaponsPlayer>().crystalGauntletsWaitTime = 50;
        }

        public static Texture2D glowTexture;

        public override void Load()
        {
            glowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.ImmediateLoad).Value;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Time > 1)
            {
                Texture2D texture = TextureAssets.Projectile[Type].Value;
                Texture2D bloom = AssetDirectory.Textures.Glow.Value;
                VertexStrip strip = new VertexStrip();

                Color StripColor(float progress) => Main.hslToRgb((Projectile.localAI[0] * 0.03f + progress) % 1f, 0.5f, 0.6f) * Utils.GetLerpValue(40, 10, Time, true);
                float StripWidth(float progress) => (Utils.GetLerpValue(0.1f, 0f, progress, true) * 1.6f + progress * 3f) * 30f * MathF.Sqrt(Utils.GetLerpValue(0, 20, Time, true));

                Vector2[] position = new Vector2[points.Count];
                float[] rotation = new float[points.Count];

                for (int i = 0; i < position.Length; i++)
                    position[i] = points[i] + offsets[i];

                for (int i = 0; i < position.Length; i++)
                    rotation[i] = Projectile.AngleTo(endPoint);

                rotation[position.Length - 1] = Projectile.AngleTo(endPoint);

                strip.PrepareStrip(position, rotation, StripColor, StripWidth, -Main.screenPosition, position.Length * 2, true);

                Effect lightningEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/CrystalLightningEffect", AssetRequestMode.ImmediateLoad).Value;
                lightningEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                lightningEffect.Parameters["uTexture"].SetValue(texture);
                lightningEffect.Parameters["uGlow"].SetValue(glowTexture);
                lightningEffect.Parameters["uColor"].SetValue(Vector3.One);
                lightningEffect.Parameters["uTime"].SetValue(Projectile.localAI[0] * 0.07f % 1f);
                lightningEffect.CurrentTechnique.Passes[0].Apply();

                strip.DrawTrail();

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                Color endPointColor = Main.hslToRgb((Projectile.localAI[0] * 0.03f + 0.5f) % 1f, 0.5f, 0.5f, 0) * Utils.GetLerpValue(40, 25, Time, true);
                Main.EntitySpriteDraw(bloom, endPoint - Main.screenPosition, bloom.Frame(), endPointColor * 0.6f, Projectile.rotation * 0.7f, bloom.Size() * 0.5f, Projectile.scale * 1.6f, 0, 0);
                Main.EntitySpriteDraw(bloom, endPoint - Main.screenPosition, bloom.Frame(), endPointColor, Projectile.rotation, bloom.Size() * 0.5f, Projectile.scale * 0.8f, 0, 0);
                Main.EntitySpriteDraw(bloom, endPoint - Main.screenPosition, bloom.Frame(), endPointColor, Projectile.rotation * 1.1f, bloom.Size() * 0.5f, Projectile.scale * 0.6f, 0, 0);
                Main.EntitySpriteDraw(bloom, endPoint - Main.screenPosition, bloom.Frame(), new Color(255, 255, 255, 0) * Utils.GetLerpValue(40, 30, Time, true), Projectile.rotation, bloom.Size() * 0.5f, Projectile.scale * 0.5f, 0, 0);
            }
            return false;
        }
    }
}
