using System;
using System.Collections.Generic;
using System.Linq;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Common.Utilities.Interfaces;
using CalamityHunt.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles
{
    public class GooLightning : ModProjectile, ISubjectOfNPC<Goozma>
    {
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
        }

        public Vector2 endPoint;

        public float colOffset;

        public ref float Time => ref Projectile.ai[0];
        public ref float Length => ref Projectile.ai[1];
        public ref float Collides => ref Projectile.ai[2];

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.velocity == Vector2.Zero) {
                Projectile.velocity = Vector2.UnitY;
            }
            Projectile.localAI[1] = Main.rand.Next(20);
            colOffset = Main.rand.Next(100);
        }

        public List<Vector2> points;

        public override void AI()
        {
            int owner = -1;
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active)) {
                Projectile.Kill();
                return;
            }
            else {
                owner = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).whoAmI;
            }

            Projectile.Center = Main.npc[owner].Center + new Vector2(13 * Main.npc[owner].direction, -20f);

            if (Time < 0) {
                if (Collides == 0 || Collides == 2) {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[owner].GetTargetData().Center).SafeNormalize(Vector2.Zero), 0.066f * Utils.GetLerpValue(-10, -30, Time, true));
                }
                else if (Collides == 1) {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[owner].GetTargetData().Center).SafeNormalize(Vector2.Zero), 0.01f * Utils.GetLerpValue(0, -20, Time, true));
                }
            }

            if (Time == 0) {
                if (Collides != 0) {
                    for (int i = 0; i < (int)(Length / 16f); i++) {
                        endPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.05f) * 16 * i;
                        if (WorldGen.InWorld(endPoint.ToTileCoordinates().X, endPoint.ToTileCoordinates().Y)) {
                            if (Main.npc[owner].GetTargetData().Center.Distance(endPoint) > 20) {
                                continue;
                            }

                            if (WorldGen.SolidTile(endPoint.ToTileCoordinates())) {
                                break;
                            }
                        }
                    }
                    SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.SmallThunder, Projectile.Center);
                }
                else {
                    endPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.05f) * Length;
                    SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.BigThunder, Projectile.Center);
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Time >= 0) {
                Projectile.rotation = Projectile.Center.AngleTo(endPoint);

                if (Time % 3 == 0 && Collides == 0) {
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Circular(1, 1), 5f, 15, 20));
                }

                if (points == null) {
                    Vector2 midPoint = Vector2.Lerp(Projectile.Center, endPoint, 0.5f) + Main.rand.NextVector2Circular(64, 64);
                    points = new BezierCurve(new List<Vector2>()
                    {
                        Projectile.Center, midPoint, endPoint
                    }).GetPoints(64);
                }

                for (int i = 0; i < points.Count; i++) {
                    float progress = (float)i / points.Count;
                    float size = Collides == 0 ? Collides == 2 ? 1f : 1.66f : 0.8f;
                    Vector2 newOffset = new Vector2(0, MathF.Sin(progress * (5 + Time / 8f) - Time * 0.33f) * 80 * size * progress).RotatedBy(Projectile.rotation);
                    points[i] = Vector2.Lerp(points[i], Vector2.Lerp(Projectile.Center, endPoint, progress) + newOffset, MathF.Pow(Time / 50f - progress * 0.2f, 2f));
                }

                Projectile.velocity = Vector2.Zero;
            }

            if (Time > 50) {
                Projectile.Kill();
            }

            Projectile.localAI[0] = Main.npc[owner].localAI[0];
            Projectile.localAI[1] += 0.5f;
            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float discard = 0;
            float small = Collides == 0 ? 1.1f : 0.7f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPoint, 50f * small * (1f - Time / 60f), ref discard)) {
                return Time > 0 && Time < 40;
            }

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<FusionBurn>(), 180);
        }

        public VertexStrip vertexStrip;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Time <= 0) {
                float small = Collides == 0 ? 1f : 0.7f;
                Texture2D tell = TextureAssets.Extra[178].Value;
                Color color = new GradientColor(SlimeUtils.GoozColors, 0.1f, 0.1f).ValueAt(Time + colOffset) with { A = 0 };
                Main.EntitySpriteDraw(tell, Projectile.Center - Main.screenPosition, null, color * small * 1.2f * Utils.GetLerpValue(-50, 0, Time, true), Projectile.rotation, Vector2.UnitY, new Vector2(1f * small, 2f), 0, 0);
            }

            if (points != null) {
                vertexStrip ??= new VertexStrip();
                float[] rotations = new float[points.Count];
                for (int i = 0; i < points.Count - 1; i++) {
                    rotations[i] = points[i].AngleFrom(points[i + 1]);
                }
                rotations[^1] = rotations[^2];

                vertexStrip.PrepareStrip(points.ToArray(), rotations, ColorFunction, WidthFunction, -Main.screenPosition, points.Count / 2, true);

                Effect lightningEffect = AssetDirectory.Effects.GooLightning.Value;
                lightningEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                lightningEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Goozma.Lightning.Value);
                lightningEffect.Parameters["uTexture1"].SetValue(AssetDirectory.Textures.Goozma.LightningGlow.Value);
                lightningEffect.Parameters["uNoiseTexture"].SetValue(AssetDirectory.Textures.Noise[7].Value);
                lightningEffect.Parameters["uTime"].SetValue(Projectile.localAI[1] * 0.05f);
                lightningEffect.CurrentTechnique.Passes[0].Apply();
                
                vertexStrip.DrawTrail();

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                Texture2D glow = AssetDirectory.Textures.Glow[1].Value;
                float scale = Projectile.scale * (float)Math.Sqrt(1f - Time / 51f);
                Color endColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Time + colOffset + 48f) with { A = 0 };
                Main.EntitySpriteDraw(glow, points[^1] - Main.screenPosition, null, endColor, Projectile.rotation, glow.Size() * 0.5f, scale, 0, 0);
                Main.EntitySpriteDraw(glow, points[^1] - Main.screenPosition, null, Color.Lerp(endColor, Color.White, 0.4f) with { A = 0 }, Projectile.rotation, glow.Size() * 0.5f, scale * 0.5f, 0, 0);
                Main.EntitySpriteDraw(glow, points[^1] - Main.screenPosition, null, Color.Lerp(endColor, Color.White, 0.7f) with { A = 0 }, Projectile.rotation, glow.Size() * 0.5f, scale * 0.3f, 0, 0);
                Main.EntitySpriteDraw(glow, points[^1] - Main.screenPosition, null, Color.Lerp(endColor, Color.White, 0.7f) with { A = 0 }, Projectile.rotation, glow.Size() * 0.5f, scale * 0.1f, 0, 0);
            }

            return false;
        }

        public Color ColorFunction(float progress)
        {
            Color color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Time + colOffset + progress * 50) with { A = 40 };
            float size = Collides == 0 ? 1.5f : 1f;
            return color * (float)Math.Pow(1f - Time / 60f, 0.6f) * size;
        }

        public float WidthFunction(float progress)
        {
            float width = 160f * MathF.Sqrt(1f - progress * 0.44f) * MathF.Pow(0.7f + progress * 0.3f, 2) * (float)Math.Sqrt(1f - Time / 51f);
            float size = Collides == 0 ? Collides == 2 ? 1f : 1.66f : 0.8f;
            return width * size;
        }
    }
}
