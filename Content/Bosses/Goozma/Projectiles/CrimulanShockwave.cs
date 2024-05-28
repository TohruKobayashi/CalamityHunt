using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class CrimulanShockwave : ModProjectile 
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 128;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Size => ref Projectile.ai[1];

        public override void AI()
        {
            Projectile.Resize((int)(MathF.Sqrt(Utils.GetLerpValue(0, 50, Time, true)) * Size), Projectile.height);

            if (Time > 60)
                Projectile.Kill();

            if (Time > 30)
                Projectile.damage = 0;

            if (Time > 0)
            {
                float prog = MathF.Pow(Utils.GetLerpValue(0, 60, Time, true), 0.7f);
                for (int i = 0; i < 2; i++)
                {
                    Vector2 off = new Vector2(prog * Size * 0.45f, 0).RotatedBy(Projectile.rotation);
                    Dust blood = Dust.NewDustPerfect(Projectile.Center + off + Main.rand.NextVector2Circular(70, 30).RotatedBy(Projectile.rotation), DustID.Blood, off.SafeNormalize(Vector2.Zero).RotatedByRandom(0.8f) * Main.rand.NextFloat(10, 25), 0, Color.DarkRed, 1f + Main.rand.NextFloat());
                    blood.noGravity = true;
                }
                for (int i = 0; i < 2; i++)
                {
                    Vector2 off = new Vector2(-prog * Size * 0.45f, 0).RotatedBy(Projectile.rotation);
                    Dust blood = Dust.NewDustPerfect(Projectile.Center + off + Main.rand.NextVector2Circular(70, 30).RotatedBy(Projectile.rotation), DustID.Blood, off.SafeNormalize(Vector2.Zero).RotatedByRandom(0.8f) * Main.rand.NextFloat(10, 25), 0, Color.DarkRed, 1f + Main.rand.NextFloat());
                    blood.noGravity = true;
                }
            }

            Time++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            //left
            for (int i = 0; i < 4; i++)
            {
                Rectangle frame = texture.Frame(1, 4, 0, 3 - i);
                Color waveColor = Color.Lerp(Color.Black * 0.5f, Color.Red, 1f - i / 4f) * Utils.GetLerpValue(45, 10, Time - i * 15f);
                Vector2 waveScale = Vector2.Lerp(new Vector2(2f, 0.1f), new Vector2(0.5f, 2f), MathF.Pow(Utils.GetLerpValue(2, 60, Time - i * 4f), 1.7f)) * (1f - i / 4f);
                Vector2 off = new Vector2(MathF.Pow(Utils.GetLerpValue(10, 60, Time - i * 4f, true), 0.7f) * Size * 0.4f, 20).RotatedBy(Projectile.rotation);
                Main.EntitySpriteDraw(texture, Projectile.Center + off - Main.screenPosition, frame, waveColor, Projectile.rotation, frame.Size() * new Vector2(0.7f, 0.66f), Projectile.scale * new Vector2(3f, 1.7f) * waveScale, SpriteEffects.None, 0);
            }
            //right
            for (int i = 0; i < 4; i++)
            {
                Rectangle frame = texture.Frame(1, 4, 0, 3 - i);
                Color waveColor = Color.Lerp(Color.Black * 0.5f, Color.Red, 1f - i / 4f) * Utils.GetLerpValue(45, 10, Time - i * 15f);
                Vector2 waveScale = Vector2.Lerp(new Vector2(2f, 0.1f), new Vector2(0.5f, 2f), MathF.Pow(Utils.GetLerpValue(2, 60, Time - i * 6f), 1.7f)) * (1f - i / 4f);
                Vector2 off = new Vector2(-MathF.Pow(Utils.GetLerpValue(10, 60, Time - i * 4f, true), 0.7f) * Size * 0.4f, 20).RotatedBy(Projectile.rotation);
                Main.EntitySpriteDraw(texture, Projectile.Center + off - Main.screenPosition, frame, waveColor, Projectile.rotation, frame.Size() * new Vector2(0.3f, 0.66f), Projectile.scale * new Vector2(3f, 1.7f) * waveScale, SpriteEffects.FlipHorizontally, 0);
            }

            return false;
        }
    }
}
