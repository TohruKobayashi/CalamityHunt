using System.Collections.Generic;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Summoner
{
    public class GobfloggerProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.Segments = 22;
        }

        public override bool PreAI()
        {
            List<Vector2> points = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, points);

            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] * 2f - Projectile.WhipSettings.Segments * 5f);
            float scale = 0.1f + Utils.GetLerpValue(-150, 250, points[points.Count - 1].Distance(Main.player[Projectile.owner].Center) * 0.5f);
            Dust light = Dust.NewDustPerfect(points[points.Count - 3] + Main.rand.NextVector2Circular(12, 12), DustID.AncientLight, Projectile.velocity * Main.rand.NextFloat(5f) + Main.rand.NextVector2Circular(12, 12), 0, glowColor, scale * (1f + Main.rand.NextFloat()));
            light.noGravity = true;
            light.noLightEmittence = true;

            for (int i = 0; i < 2; i++) {
                Dust dark = Dust.NewDustPerfect(points[points.Count - 3] + Main.rand.NextVector2Circular(12, 12), DustID.TintableDust, Projectile.velocity * Main.rand.NextFloat(5f) + Main.rand.NextVector2Circular(7, 7), 50, Color.Black, scale * (1f + Main.rand.NextFloat()));
                dark.noGravity = true;
            }

            Projectile.localAI[0] = Main.GlobalTimeWrappedHourly * 170f;

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Gobbed>(), 300);
            if (!Main.player[Projectile.owner].HasBuff<Absorption>()) {
                Main.player[Projectile.owner].AddBuff(ModContent.BuffType<Absorption>(), 300);
            }

            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        }

        private void DrawLine(List<Vector2> points)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width * 0.5f, 2);

            Vector2 drawPosition = points[0];
            for (int i = 0; i < points.Count - 2; i++) {
                Vector2 change = points[i + 1] - points[i];

                float rotation = change.ToRotation() - MathHelper.PiOver2;
                Vector2 scale = new Vector2(1, (change.Length() + 4) / frame.Height);

                Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] - i * 5f);
                glowColor.A /= 2;
                Main.EntitySpriteDraw(texture, drawPosition - Main.screenPosition, frame, glowColor, rotation, origin, scale, 0, 0);

                drawPosition += change;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> points = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, points);

            DrawLine(points);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 drawPosition = points[0];

            for (int i = 0; i < points.Count - 1; i++) {
                int frameY = 0;
                float scale = 0.6f + Utils.GetLerpValue(0, points.Count, i, true) * 0.8f;

                if (i == points.Count - 2) {
                    frameY = 4;

                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Projectile.ai[0] / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > points.Count * 3 / 5) {
                    frameY = 3;
                }
                else if (i > points.Count / 3) {
                    frameY = 2;
                }
                else if (i > 0) {
                    frameY = 1;
                }

                Vector2 change = points[i + 1] - points[i];

                float rotation = change.ToRotation() - MathHelper.PiOver2;
                Rectangle frame = texture.Frame(1, 5, 0, frameY);

                Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] - i * 5f);
                glowColor.A /= 2;
                Main.EntitySpriteDraw(glow, drawPosition - Main.screenPosition, frame, Color.Black * 0.5f, rotation, frame.Size() * new Vector2(0.5f, 0.4f), scale * 1.1f, spriteEffects, 0);
                Main.EntitySpriteDraw(texture, drawPosition - Main.screenPosition, frame, Lighting.GetColor(points[i].ToTileCoordinates()), rotation, frame.Size() * new Vector2(0.5f, 0.4f), scale, spriteEffects, 0);
                Main.EntitySpriteDraw(glow, drawPosition - Main.screenPosition, frame, Color.CornflowerBlue * 0.33f, rotation, frame.Size() * new Vector2(0.5f, 0.4f), scale, spriteEffects, 0);
                Main.EntitySpriteDraw(glow, drawPosition - Main.screenPosition, frame, glowColor, rotation, frame.Size() * new Vector2(0.5f, 0.4f), scale * 1.15f, spriteEffects, 0);

                drawPosition += change;
            }

            return false;
        }
    }
}
