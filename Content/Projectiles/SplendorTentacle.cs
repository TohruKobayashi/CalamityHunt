using System;
using System.Collections.Generic;
using System.Linq;
using CalamityHunt.Common.Players;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles
{
    public class SplendorTentacle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.manualDirectionChange = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public ref float Time => ref Projectile.ai[0];
        //TODO: sync
        public float TimeInner;
        public ref float Attack => ref Projectile.ai[1];
        public ref float Index => ref Projectile.ai[2];

        public ref Player Player => ref Main.player[Projectile.owner];

        public override void AI()
        {
            if (Player.dead || !Player.active || !Player.GetModPlayer<SplendorJamPlayer>().active) {
                Projectile.Kill();
                return;
            }


            int count = Main.projectile.Count(n => n.active && n.type == Type && n.owner == Player.whoAmI && n.whoAmI != Projectile.whoAmI);
            Index = 0;

            foreach (Projectile proj in Main.projectile.Where(n => n.active && n.owner == Player.whoAmI && n.type == Type)) {
                if (proj.whoAmI > Projectile.whoAmI) {
                    Index++;
                }

                if (proj.whoAmI == Projectile.whoAmI) {
                    continue;
                }

                //if (Projectile.Distance(proj.Center) < 40)
                //{
                //    Projectile.velocity -= Projectile.DirectionFrom(proj.Center).SafeNormalize(Vector2.Zero) * 13;
                //    proj.velocity -= proj.DirectionFrom(Projectile.Center).SafeNormalize(Vector2.Zero) * 13;
                //}
            }

            if (Time == 0 && Main.netMode != NetmodeID.MultiplayerClient) {
                Attack = (int)(Index % 2);
                Projectile.netUpdate = true;
            }

            Projectile.timeLeft = 20;
            Projectile.rotation = Projectile.AngleFrom(Player.MountedCenter) - MathHelper.PiOver2;
            NPC target = Projectile.FindTargetWithinRange(320);

            float trackSpeed = 0.05f;
            Vector2 homePos = Player.MountedCenter - new Vector2(110, 0).RotatedBy(-MathHelper.PiOver2 - MathHelper.PiOver2 * Player.direction * Math.Min(count, 1) + (MathHelper.Pi / Math.Max(1f, count) * Index + MathHelper.PiOver2) * Player.direction)
                - Player.velocity * 10 + new Vector2(MathF.Sin(Time * 0.05f + Index * 1.5f), MathF.Cos(Time * 0.05f + Index * 1.5f)) * 5f;

            if (target != null) {
                Projectile.rotation = Player.AngleTo(target.Center) - MathHelper.PiOver2;

                switch (Attack) {
                    case 0:

                        trackSpeed = 1f;

                        if (Projectile.Distance(target.Center) < 300) {
                            TimeInner++;
                        }

                        homePos = target.Center + new Vector2(MathHelper.SmoothStep(-200, 200, MathF.Sqrt(TimeInner / 25f)) * Projectile.direction, 0).RotatedBy(Projectile.rotation);

                        if (TimeInner > 25 && Main.netMode != NetmodeID.MultiplayerClient) {
                            TimeInner = 0;

                            if (Main.rand.NextBool(2)) {
                                Attack++;
                            }

                            Projectile.direction *= -1;

                            Projectile.netUpdate = true;

                        }

                        break;

                    case 1:

                        TimeInner++;

                        homePos = target.Center;

                        if (Projectile.Distance(homePos) < 30) {
                            //LIFESTEAL!!!!!!!!!
                        }

                        if ((TimeInner > 120 || Projectile.Distance(target.Center) > 300) && Main.netMode != NetmodeID.MultiplayerClient) {
                            TimeInner = 0;

                            Attack++;

                            Projectile.netUpdate = true;
                        }

                        break;

                    default:

                        Attack = 0;

                        break;
                }
            }
            if (target == null) {
                Projectile.Center += Player.velocity * 0.5f;
                TimeInner = 0;
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(homePos) * Projectile.Distance(homePos) * 0.1f, trackSpeed);

            if (Projectile.Distance(homePos) < 3f) {
                Projectile.Center = homePos;
            }

            Time++;

            if (tentacle == null) {
                tentacle = new Rope(Projectile.Center, Player.MountedCenter, 30, 10f, Vector2.Zero, 0.05f, 30);
            }

            tentacle.StartPos = Projectile.Center;
            tentacle.EndPos = Player.MountedCenter;
            tentacle.gravity = -Vector2.UnitX * Player.direction * 0.05f - Vector2.UnitY * 0.01f;
            tentacle.damping = Utils.GetLerpValue(20, 0, Player.velocity.Length(), true) * 0.05f;
            tentacle.Update();

            if (Projectile.Distance(Player.MountedCenter) > 300) {
                Projectile.Center = Player.MountedCenter + new Vector2(300, 0).RotatedBy(Projectile.AngleFrom(Player.MountedCenter));
            }
        }

        public Rope tentacle;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Color light = Lighting.GetColor(Player.Center.ToTileCoordinates());

            if (tentacle != null) {
                List<Vector2> points = new List<Vector2>();
                points.AddRange(tentacle.GetPoints());
                points.Add(Player.MountedCenter);

                for (int i = points.Count - 1; i > 0; i--) {
                    Rectangle tentacleFrame = texture.Frame(2, 10, 0, 7 - (int)((float)i / points.Count * 6));
                    Rectangle tentacleGlowFrame = texture.Frame(2, 10, 1, 7 - (int)((float)i / points.Count * 6));

                    float rot = points[i].AngleTo(points[i - 1]) - MathHelper.PiOver2;
                    Vector2 stretch = new Vector2((1.1f - (float)i / points.Count * 0.6f) * Projectile.scale, i > points.Count - 2 ? points[i].Distance(points[i - 1]) / (tentacleFrame.Height - 2f) : 1.1f);

                    if (i == 1) {
                        tentacleFrame = texture.Frame(2, 5, 0, 4);
                        tentacleGlowFrame = texture.Frame(2, 5, 1, 4);
                    }
                    if (i == points.Count - 1) {
                        tentacleFrame = texture.Frame(2, 5, 0, 0);
                        tentacleGlowFrame = texture.Frame(2, 5, 1, 0);
                        stretch = new Vector2(Projectile.scale * 0.6f, points[i].Distance(points[i - 1]) / (tentacleFrame.Height - 2f) * 1.2f);
                    }
                    Main.EntitySpriteDraw(texture, points[i] - Main.screenPosition, tentacleFrame, Color.Lerp(light, Color.White, 1f - (float)i / points.Count), rot, tentacleFrame.Size() * new Vector2(0.5f, 0f), stretch, 0, 0);

                    Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.5f, 0.5f).ValueAt(Main.GlobalTimeWrappedHourly * 150 + i * 10) * 0.5f;
                    glowColor.A /= 2;
                    Main.EntitySpriteDraw(texture, points[i] - Main.screenPosition, tentacleGlowFrame, glowColor.MultiplyRGBA(Color.Lerp(light, Color.White, 1f - (float)i / points.Count)) * 1.5f, rot, tentacleFrame.Size() * new Vector2(0.5f, 0f), stretch, 0, 0);
                }

                //Utils.DrawBorderString(Main.spriteBatch, Index.ToString(), Projectile.Center - Vector2.UnitY * 50 - Main.screenPosition, Color.White);
            }

            return false;
        }
    }
}
