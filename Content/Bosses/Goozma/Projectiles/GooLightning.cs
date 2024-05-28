using CalamityHunt.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using CalamityHunt.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class GooLightning : ModProjectile, IDieWithGoozma
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
        }

        public Vector2 endPoint;

        public float colOffset;

        public ref float Time => ref Projectile.ai[0];
        public ref float Length => ref Projectile.ai[1];
        public ref float Collides => ref Projectile.ai[2];

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.velocity == Vector2.Zero)
                Projectile.velocity = Vector2.UnitY;

            colOffset = Main.rand.Next(100);
        }

        public List<Vector2> points;
        public List<Vector2> pointVelocities;

        public override void AI()
        {
            int owner = -1;
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active))
            {
                Projectile.Kill();
                return;
            }
            else
                owner = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).whoAmI;
            
            Projectile.Center = Main.npc[owner].Center + new Vector2(13 * Main.npc[owner].direction, -20f);

            if (Time < 0)
            {
                if (Collides == 0 || Collides == 2)
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[owner].GetTargetData().Center).SafeNormalize(Vector2.Zero), 0.066f * Utils.GetLerpValue(-10, -30, Time, true));
                else if (Collides == 1)
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[owner].GetTargetData().Center).SafeNormalize(Vector2.Zero), 0.01f * Utils.GetLerpValue(0, -20, Time, true));
            }
            if (Time == 0)
            {
                if (Collides != 0)
                {
                    for (int i = 0; i < (int)(Length / 16f); i++)
                    {
                        endPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.05f) * 16 * i;
                        if (WorldGen.InWorld(endPoint.ToTileCoordinates().X, endPoint.ToTileCoordinates().Y))
                        {
                            if (Main.npc[owner].GetTargetData().Center.Distance(endPoint) > 20)
                                continue;
                            if (WorldGen.SolidTile(endPoint.ToTileCoordinates()))
                                break;
                        }

                    }
                }
                else
                    endPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.05f) * Length;

                if (Collides == 0)
                {
                    SoundStyle lightningSound = AssetDirectory.Sounds.Goozma.BigThunder;
                    SoundEngine.PlaySound(lightningSound, Projectile.Center);
                }
                else
                {
                    SoundStyle lightningMiniSound = AssetDirectory.Sounds.Goozma.SmallThunder;
                    SoundEngine.PlaySound(lightningMiniSound, Projectile.Center);
                }

                LightningData data = new LightningData(Projectile.Center, endPoint, 0.5f);
                points = data.Value;
                pointVelocities = new List<Vector2>();
                for (int i = 0; i < points.Count; i++)
                    pointVelocities.Add(Main.rand.NextVector2Circular(1, 1) * Math.Min(1f, Projectile.Distance(endPoint) * 0.0012f));

                //for (int i = 0; i < totalPoints; i++)
                //{
                //    Vector2 lerp = Vector2.Lerp(Projectile.Center, endPoint, (float)i / totalPoints);
                //    points.Add(lerp + pointVelocities[i] * 30 * Math.Min(1f, Projectile.Distance(endPoint) * 0.001f) * Utils.GetLerpValue(0, 2, i, true) * Utils.GetLerpValue(totalPoints, totalPoints - 2, i, true));
                //}
            }

            if (Time >= 0)
            {
                if (Time % 3 == 0 && Collides == 0)
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Circular(1, 1), 15f, 15, 20));

                Projectile.velocity = Vector2.Zero;

                for (int i = 0; i < points.Count - 1; i++)
                {
                    points[i] = Vector2.Lerp(points[i], Vector2.Lerp(Projectile.Center, endPoint, (float)i / points.Count) + pointVelocities[i] * 6, 0.6f * (1f - (float)i / points.Count));
                    pointVelocities[i] *= 1.07f * Math.Min(1f, Projectile.Distance(endPoint) * 0.003f) * Utils.GetLerpValue(0, 3, i, true) * Utils.GetLerpValue(points.Count, points.Count - 3, i, true);
                    points[i] += Main.rand.NextVector2Circular(2, 2) * Utils.GetLerpValue(0, 3, i, true) * Utils.GetLerpValue(points.Count, points.Count - 3, i, true); ;
                }

                if (!Main.dedServ)
                {
                    Dust start = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(4, 4), 0, new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Time * 3 + colOffset + 60), 1 + Main.rand.NextFloat());
                    start.noGravity = true;
                    Dust contact = Dust.NewDustPerfect(endPoint, DustID.FireworksRGB, Main.rand.NextVector2Circular(4, 4), 0, new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Time * 3 + colOffset + 60), 1 + Main.rand.NextFloat());
                    contact.noGravity = true;
                }
            }

            //if (Time == 0)
            //    SoundEngine.PlaySound(SoundID.Thunder.WithVolumeScale(0.7f), Projectile.Center);

            if (Time > 30)
                Projectile.Kill();

            Projectile.localAI[0] = Main.npc[owner].localAI[0];
            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float discard = 0;
            float small = Collides == 0 ? 1f : 0.6f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPoint, 50f * small * (1f - Time / 30f), ref discard))
                return Time > 0 && Time < 20;
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<FusionBurn>(), 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Time <= 0)
            {
                float small = Collides == 0 ? 1f : 0.7f;
                Color color = new GradientColor(SlimeUtils.GoozColors, 0.1f, 0.1f).ValueAt(Time + colOffset) * small;
                color.A = 0;
                Asset<Texture2D> tell = TextureAssets.Extra[178];
                Main.EntitySpriteDraw(tell.Value, Projectile.Center - Main.screenPosition, null, color * 0.7f * Utils.GetLerpValue(-50, 0, Time, true), Projectile.velocity.ToRotation(), Vector2.UnitY, new Vector2(2f * small, 5f), 0, 0);
                Main.EntitySpriteDraw(tell.Value, Projectile.Center - Main.screenPosition, null, color * 1.5f * Utils.GetLerpValue(-50, 0, Time, true), Projectile.velocity.ToRotation(), Vector2.UnitY, new Vector2(1f * small, 2f), 0, 0);
            }
            if (Time > 0)
            {
                VertexStrip strip = new VertexStrip();
                float[] rotations = new float[points.Count];
                for (int i = 0; i < points.Count - 1; i++)
                    rotations[i] = points[i].AngleTo(points[i + 1]);

                rotations[points.Count - 1] = points[points.Count - 2].AngleTo(endPoint);

                strip.PrepareStrip(points.ToArray(), rotations, ColorFunction, WidthFunction, -Main.screenPosition, points.Count, true);

                Effect lightningEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/GooLightningEffect", AssetRequestMode.ImmediateLoad).Value;
                lightningEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                lightningEffect.Parameters["uTexture"].SetValue(AssetDirectory.Textures.Goozma.Lightning.Value);
                lightningEffect.Parameters["uGlow"].SetValue(AssetDirectory.Textures.Goozma.LightningGlow.Value);
                lightningEffect.Parameters["uColor"].SetValue(Vector3.One);
                lightningEffect.Parameters["uTime"].SetValue(-Projectile.localAI[0] * 0.05f);
                lightningEffect.Parameters["uBackPower"].SetValue(0.5f);
                lightningEffect.CurrentTechnique.Passes[0].Apply();

                strip.DrawTrail();

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }

            return false;
        }

        public Color ColorFunction(float progress)
        {
            Color color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] + progress * 120);
            float small = Collides == 0 ? 1f : 0.7f;
            return color * (float)Math.Pow(1f - (Time / 30f), 0.6f) * small;
        }

        public float WidthFunction(float progress)
        {
            float width = (50f + (float)Math.Sqrt(Utils.GetLerpValue(0.1f, 0.5f, progress, true) * Utils.GetLerpValue(0.9f, 0.5f, progress, true)) * 100f) * (float)Math.Pow(1f - (Time / 30f), 0.8f);
            float small = Collides == 0 ? (Collides == 2 ? 1f : 1.4f) : 0.33f;
            return width * small;
        }
    }
}
