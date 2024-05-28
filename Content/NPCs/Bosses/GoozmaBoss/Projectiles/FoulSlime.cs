using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles
{
    public class FoulSlime : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Foul Slime");
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 36;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 210;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] = Main.rand.NextFloat(0.8f, 1.1f);
        }

        public override void AI()
        {
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(-15, 30, Time, true)) * Projectile.localAI[0];
            Projectile.velocity.Y += 0.6f;

            int target = -1;
            if (Projectile.velocity.Y > 1) {
                if (Main.player.Any(n => n.active && !n.dead)) {
                    target = Main.player.First(n => n.active && !n.dead).whoAmI;
                }

                if (target > -1) {
                    if (Projectile.Center.Y > Main.player[target].MountedCenter.Y) {
                        Projectile.velocity.Y = -Projectile.velocity.Y * Main.rand.NextFloat(0.7f, 0.8f);
                        Projectile.velocity.X *= 0.9f;
                    }

                    if (Projectile.Center.Y > Main.player[target].MountedCenter.Y + 20) {
                        Projectile.velocity.Y = -20;
                    }
                }
            }

            if (Main.rand.NextBool(19)) {
                Color color = Color.Lerp(Color.DarkGreen, Color.MediumOrchid, Main.rand.NextFloat());
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20), DustID.ToxicBubble, Projectile.velocity * 0.5f, 0, color, 1f).noGravity = true;
            }

            if (Time > 200) {
                for (int i = 0; i < 5; i++) {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12), DustID.Corruption, Main.rand.NextVector2Circular(5, 5), 0, Color.White, 1.5f).noGravity = true;
                }
            }

            Time++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ReLogic.Content.Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Projectile.rotation = 0;

            Color glowColor = Color.Indigo;
            glowColor.A = 0;

            Vector2 squish = new Vector2(1f - Math.Abs(Projectile.velocity.X + Projectile.velocity.Y * 0.1f) * 0.01f, 1f + Math.Abs(Projectile.velocity.Y) * 0.04f);

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * squish, 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, glowColor * 0.4f, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * squish * 1.66f, 0, 0);

            return false;
        }
    }
}
