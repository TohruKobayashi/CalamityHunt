using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class CrimulanSmasher : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Smasher");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 500;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 700;
        }

        public ref float Time => ref Projectile.ai[0];

        public ref Player Target => ref Main.player[(int)Projectile.ai[1]];

        private Vector2 saveTarget;

        public override void AI()
        {
            if (Projectile.ai[1] < 0)
            {
                foreach (Player player in Main.player.Where(n => n.active && !n.dead))
                    Projectile.ai[1] = player.whoAmI;
            }

            if (Time < 1)
            {
                saveTarget = new Vector2(Projectile.Center.X, Projectile.FindSmashSpot(Target.Center).Y);

                Projectile.velocity.Y = 0;
                if (Projectile.ai[2] == 1)
                {
                    Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, (Target.Center.X - Projectile.Center.X) * 0.05f, 0.01f * Utils.GetLerpValue(0, -25, Time, true));
                    Projectile.velocity.X *= 0.98f * Utils.GetLerpValue(0, -25, Time, true);
                }
                else
                    Projectile.velocity.X = 0;
                Projectile.position.Y = saveTarget.Y - 1200 + (Projectile.height / 2);
            }
            else
                Projectile.velocity.X = 0;


            if (Time > 0)
                Projectile.Center = Vector2.Lerp(Projectile.Center, saveTarget, 0.001f + Utils.GetLerpValue(0, 10, Time, true));

            if (Time == 8)
                SoundEngine.PlaySound(SoundID.Item167.WithPitchOffset(0.2f), Projectile.Center);

            if (Time > 20)
                Projectile.Kill();

            Time++;
            Projectile.localAI[0]++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 2 && Time < 12)
                return new Rectangle(projHitbox.X, projHitbox.Y - 500, projHitbox.Width, projHitbox.Height + 500).Intersects(targetHitbox);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> trail = ModContent.Request<Texture2D>(Texture + "Trail");
            Asset<Texture2D> tell = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Projectiles/GoozmaDeathGodrays");

            if (Time < 0)
            {
                float width = (float)Math.Pow(Utils.GetLerpValue(0, 15, Projectile.localAI[0], true) * Utils.GetLerpValue(5, -60, Time, true), 0.5f) * Projectile.width * 0.015f;
                Color tellColor = new Color(180, 30, 0, 0);
                Main.EntitySpriteDraw(tell.Value, new Vector2(Projectile.Center.X - Main.screenPosition.X, width * 40 - 20), null, tellColor, Projectile.rotation + MathHelper.Pi, tell.Size() * new Vector2(0.5f, 0.66f), new Vector2(width, 14f - width), 0, 0);
                return false;

            }

            Vector2 squish = new Vector2(0.5f, 1.5f);
            Vector2 trailSquish = new Vector2(0.5f, 1.5f);

            if (Time > 9)
                squish = new Vector2(1f + Utils.GetLerpValue(18, 10, Time, true) * 0.7f, 1f - Utils.GetLerpValue(18, 10, Time, true) * 0.7f);

            float fadeOut = Utils.GetLerpValue(20, 15, Time, true) * Utils.GetLerpValue(-1, 5, Time, true);
            float trailOut = fadeOut * Utils.GetLerpValue(15, 10, Time, true) * 0.9f;

            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    float superProg = i / (float)ProjectileID.Sets.TrailCacheLength[Type];
                    float prog = j / 5f;

                    if (Time > 9)
                        trailSquish = new Vector2(1f + Utils.GetLerpValue(18, 10, Time, true) * 0.7f * (1f - superProg - prog * 0.2f), 1f - Utils.GetLerpValue(18, 10, Time, true) * 0.7f * (1f - superProg - prog * 0.2f));

                    Color trailColor = Color.Lerp(Color.Black * 0.33f, new Color(30, 4, 2, 10), superProg);
                    float trailProgress = Utils.GetLerpValue(0, ProjectileID.Sets.TrailCacheLength[Type], i + prog, true);
                    Vector2 position = Vector2.Lerp(Projectile.oldPos[i - 1], Projectile.oldPos[i], prog) + Projectile.Size * new Vector2(0.5f, 1f);
                    Main.EntitySpriteDraw(trail.Value, position - Main.screenPosition, null, trailColor * (1f - trailProgress) * trailOut, Projectile.rotation, texture.Size() * new Vector2(0.5f, 1f), Projectile.scale * trailSquish * (0.5f + trailOut * 0.5f), 0, 0);
                }
            }

            Main.EntitySpriteDraw(texture.Value, Projectile.Bottom - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() * new Vector2(0.5f, 1f), Projectile.scale * squish * (0.5f + fadeOut * 0.5f), 0, 0);

            return false;
        }
    }
}
