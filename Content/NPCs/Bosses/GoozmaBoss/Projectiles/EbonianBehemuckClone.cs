using System;
using System.Collections.Generic;
using System.Linq;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles
{
    public class EbonianBehemuckClone : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ebonian Behemuck");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 700;
            initializedLocal = false;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float WhichOne => ref Projectile.ai[1];
        private ref float CupGameRotation => ref Projectile.ai[2];

        private Vector2 saveTarget;

        private List<int> eyeType;

        public override void OnSpawn(IEntitySource source)
        {
            
        }

        public bool initializedLocal = false;

        public override void AI()
        {
            if (!initializedLocal) {
                Projectile.scale = 0.01f;
                List<int> eyeTypes = new List<int>();
                int eyeCount = 8;
                for (int i = 0; i < eyeCount; i++) {
                    eyeTypes.Add(i);
                }

                eyeType = new List<int>();
                for (int i = 0; i < eyeCount; i++) {
                    int rand = Main.rand.Next(eyeTypes.Count);
                    eyeType.Add(rand);
                    eyeTypes.RemoveAt(rand);
                }

                initializedLocal = true;
            }
            int owner = -1;
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<EbonianBehemuck>() && n.active)) {
                Projectile.active = false;
                return;
            }
            else {
                owner = Main.npc.First(n => n.type == ModContent.NPCType<EbonianBehemuck>() && n.active).whoAmI;
            }

            int slimeCount = 5;

            NPCAimedTarget target = Main.npc[owner].GetTargetData();
            if (Time < 0) {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.8f, 0.1f);
                Projectile.rotation = Projectile.AngleTo(Main.npc[owner].Center) - MathHelper.PiOver2;

                float distanceOut = (220 + (float)Math.Pow(Utils.GetLerpValue(-30, 0, Time, true), 2f) * 200);
                Vector2 outerTarget = Main.npc[owner].Center + new Vector2(distanceOut * (float)Math.Sqrt(Utils.GetLerpValue(-150, -80, Time + WhichOne % slimeCount * 40, true)), 0).RotatedBy(MathHelper.TwoPi / slimeCount * WhichOne + CupGameRotation);

                if (Projectile.ai[1] >= slimeCount) {
                    outerTarget = Main.npc[owner].Center - new Vector2(distanceOut * (float)Math.Sqrt(Utils.GetLerpValue(-150, -90, Time + WhichOne % slimeCount * 40, true)), 0).RotatedBy(MathHelper.TwoPi / slimeCount * WhichOne + CupGameRotation);
                }

                CupGameRotation += Main.npc[owner].velocity.X * 0.001f;
                Vector2 baseVelocity = Projectile.DirectionTo(outerTarget).SafeNormalize(Vector2.Zero) * Projectile.Distance(outerTarget) * 0.2f;
                Projectile.velocity = baseVelocity;
                saveTarget = target.Center;
                squish = Vector2.SmoothStep(Vector2.One, new Vector2(1.3f, 0.8f), Utils.GetLerpValue(-20, 0, Time, true));
            }
            else if (Projectile.ai[1] >= 0) {
                float squishX = (float)Utils.Lerp(1.3f, 0.6f, Utils.GetLerpValue(0, 20, Time, true));
                float squishY = (float)Utils.Lerp(0.8f, 1.5f, Utils.GetLerpValue(0, 20, Time, true));
                squish = new Vector2(squishX, squishY);
                if (Time >= 20)
                    Projectile.velocity = Projectile.DirectionTo(saveTarget).SafeNormalize(Vector2.Zero) * Utils.GetLerpValue(20, 30, Time, true) * (float)Math.Pow(Utils.GetLerpValue(0, 30, Time, true), 1.5f) * 70;
                else
                    Projectile.velocity = Projectile.DirectionTo(saveTarget).SafeNormalize(Vector2.Zero) * -Utils.GetLerpValue(0, 20, Time, true) * Utils.GetLerpValue(0, 20, Time, true) * 20;

                if (Projectile.Distance(saveTarget) < 60) {
                    Projectile.Center = saveTarget;// + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
                    Projectile.velocity = Vector2.Zero;
                    Projectile.ai[1] = -1;
                    Time = 0;
                    for (int i = 0; i < Main.rand.Next(30, 40); i++) {
                        Vector2 velocity = Main.rand.NextVector2Circular(16, 16) - Projectile.rotation.ToRotationVector2() * 10f;
                        Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(30, 30) + new Vector2(velocity.X * 15f, 32f);
                        CalamityHunt.particles.Add(Particle.Create<EbonGelChunk>(particle => {
                            particle.position = position;
                            particle.velocity = velocity;
                            particle.scale = Main.rand.NextFloat(0.1f, 2.1f);
                            particle.color = Color.White;
                        }));
                    }

                    for (int i = 0; i < Main.rand.Next(1, 3); i++) {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Main.rand.NextVector2Circular(5, 5), ModContent.ProjectileType<ToxicSludge>(), Projectile.damage / 2, 0);
                    }

                    SoundEngine.PlaySound(SoundID.Item167.WithPitchOffset(0.2f), Projectile.Center);

                }
            }
            else {
                Projectile.velocity = Vector2.Zero;
                squish = Vector2.Lerp(new Vector2(0.6f, 1.5f), Vector2.One, Utils.GetLerpValue(15, 1, Time, true)) * Utils.GetLerpValue(20, 8, Time, true);
                if (Time > 20) {
                    Projectile.Kill();
                }
            }

            if (Main.rand.NextBool(3)) {
                Dust.NewDustPerfect(Projectile.Center + new Vector2(0, Projectile.height / 2).RotatedBy(Projectile.rotation) * Projectile.scale * squish + Main.rand.NextVector2Circular(60, 48).RotatedBy(Projectile.rotation), 4, Projectile.velocity * 0.5f, 150, Color.DarkOrchid, 2f).noGravity = true;
            }

            Projectile.frameCounter++;

            Time++;
            Projectile.localAI[0]++;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5) {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 4;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 0) {
                return base.Colliding(projHitbox, targetHitbox);
            }

            return false;
        }

        Vector2 squish = Vector2.One;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCs.Add(index);

        public static Texture2D ballTexture;

        public override void Load()
        {
            ballTexture = ModContent.Request<Texture2D>(Texture + "Ball", AssetRequestMode.ImmediateLoad).Value;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D eyeTexture = EbonianBehemuck.eyeTexture;
            Rectangle frame = texture.Frame(1, 4, 0, Projectile.frame);

            Texture2D tell = AssetDirectory.Textures.Glow[0].Value;

            float scale = 1f;

            if (Time < -10) {
                scale = 1.33f;
                texture = ballTexture;
                frame = texture.Frame();
            }

            if (eyeType != null) {


                for (int i = 0; i < Projectile.ai[1] % 3 + 1; i++) {
                    Rectangle eyeFrame = eyeTexture.Frame(8, 1, eyeType[i], 0);
                    Vector2 off = new Vector2((float)Math.Sin((Projectile.localAI[0] * 0.05f + i * 2f) % MathHelper.TwoPi) * 10 * (i % 2 == 0 ? 1 : -1), 0).RotatedBy((Projectile.localAI[0] * 0.008f + i * 2.4) % MathHelper.TwoPi);
                    Main.EntitySpriteDraw(eyeTexture, Projectile.Center + off * squish - new Vector2(0, 45).RotatedBy(Projectile.rotation) * Projectile.scale * squish - Main.screenPosition, eyeFrame, new Color(110, 50, 255, 20), Projectile.rotation + Projectile.localAI[0] * 0.1f * (i % 2 == 0 ? 1 : -1), eyeFrame.Size() * 0.5f, Projectile.scale * 1.2f, 0, 0);
                    Main.EntitySpriteDraw(eyeTexture, Projectile.Center + off * squish - new Vector2(0, 45).RotatedBy(Projectile.rotation) * Projectile.scale * squish - Main.screenPosition, eyeFrame, Color.White, Projectile.rotation + Projectile.localAI[0] * 0.1f * (i % 2 == 0 ? 1 : -1), eyeFrame.Size() * 0.5f, Projectile.scale, 0, 0);
                }
            }
            for (int i = 0; i < 4; i++) {
                Vector2 off = new Vector2(15 + (float)Math.Sin(Projectile.localAI[0] * 0.05f + i * 2f), 0).RotatedBy(MathHelper.TwoPi / 4f * i + Projectile.localAI[0] * 0.08f);
                Main.EntitySpriteDraw(texture, Projectile.Center + off - Main.screenPosition, frame, Color.SlateBlue * 0.1f, Projectile.rotation, frame.Size() * new Vector2(0.5f, 0.9f), Projectile.scale * squish * scale, 0, 0);
            }
            if (Projectile.ai[1] >= 0) {
                float tellFade = (float)Math.Sqrt(Utils.GetLerpValue(-90, 20, Time, true) * Utils.GetLerpValue(30, 0, Time, true)) * 2f;
                Main.EntitySpriteDraw(tell, Projectile.Center - Main.screenPosition, tell.Frame(), new Color(18, 8, 40, 0) * 0.8f * tellFade, Projectile.rotation, tell.Size() * new Vector2(0.5f, 0.6f), Projectile.scale * (7f + Utils.GetLerpValue(-20, 10, Time, true)), 0, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White * 0.8f, Projectile.rotation, frame.Size() * new Vector2(0.5f, 0.9f), Projectile.scale * squish, 0, 0);

            return false;
        }
    }
}
