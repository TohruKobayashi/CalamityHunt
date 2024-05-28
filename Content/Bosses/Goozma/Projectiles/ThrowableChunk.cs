using CalamityHunt.Content.Bosses.Goozma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class ThrowableChunk : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            // DisplayName.SetDefault("Toxic Sludge");
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Size => ref Projectile.ai[1];
        public ref float ChunkStyleInt => ref Projectile.ai[2];

        public enum ThrowableChunkStyle
        {
            Default,
            Meteor,
            Rock,
            DirtGrass,
            Comet,
            Count
        }

        public ThrowableChunkStyle chunkStyle;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] = Main.rand.NextFloat(0.9f, 1.15f);
            Projectile.localAI[1] = Main.rand.Next(250);
            Projectile.rotation = Main.rand.NextFloat(-1f, 1f);
            WeightedRandom<ThrowableChunkStyle> styleRandom = new WeightedRandom<ThrowableChunkStyle>();
            styleRandom.Add(ThrowableChunkStyle.DirtGrass, 0.7f);
            styleRandom.Add(ThrowableChunkStyle.Rock, 0.5f);
            styleRandom.Add(ThrowableChunkStyle.Meteor, 0.4f);
            styleRandom.Add(ThrowableChunkStyle.Comet, 0.2f);
            chunkStyle = styleRandom.Get();
            ChunkStyleInt = (int)chunkStyle;
        }

        public override void AI()
        {
            ChunkStyleInt = (int)chunkStyle;

            Vector2 sized = new Vector2(42 + 42 * Size);
            if (Projectile.width != (int)sized.X || Projectile.height != (int)sized.Y)
                Projectile.Resize((int)sized.X, (int)sized.Y);

            //if (Time < 50)
            //    Projectile.velocity *= Utils.GetLerpValue(0, 50, Time, true) * 1.4f;

            Projectile.rotation = Utils.AngleLerp(Projectile.velocity.X * 0.01f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, Projectile.velocity.X * 0.1f) + (float)Math.Sin(Projectile.localAI[0] * 0.08f % MathHelper.TwoPi) * 0.2f;

            if (Time > 2000)
                Projectile.Kill();

            //Projectile.frameCounter++;
            //if (Projectile.frameCounter > 1 + (int)(Size * 1.5f))
            //{
            //    Projectile.frameCounter = 0;
            //    Projectile.frame = (Projectile.frame + 1) % 4;
            //}

            switch (chunkStyle)
            {
                case ThrowableChunkStyle.Meteor:

                    if (Main.rand.NextBool(3))
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.InfernoFork, Main.rand.NextVector2Circular(3, 3) - Projectile.velocity * 0.5f, 0, Color.White, 1f + Main.rand.NextFloat((Size + 1) * 0.5f)).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.Torch, Main.rand.NextVector2Circular(3, 3) - Projectile.velocity, 0, Color.White, 0.3f + Main.rand.NextFloat((Size + 1) * 0.5f)).noGravity = true;

                    break;

                case ThrowableChunkStyle.DirtGrass:

                    if (Main.rand.NextBool() && Size >= 2)
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.Grass, Main.rand.NextVector2Circular(5, 5) - Projectile.velocity, 0, Color.White, 1f + Main.rand.NextFloat()).noGravity = true;
                    
                    if (Main.rand.NextBool(3))
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.Dirt, Main.rand.NextVector2Circular(5, 5) - Projectile.velocity * 0.5f, 0, Color.White, 1f + Main.rand.NextFloat((Size + 1) * 0.5f)).noGravity = true;

                    break;

                case ThrowableChunkStyle.Rock:

                    if (Main.rand.NextBool(3))
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.Stone, Main.rand.NextVector2Circular(3, 3) - Projectile.velocity * 0.5f, 0, Color.White, 1f + Main.rand.NextFloat((Size + 1) * 0.5f)).noGravity = true;

                    break;                
                
                case ThrowableChunkStyle.Comet:

                    Size = 0;

                    Projectile.velocity *= 1.005f;

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.Snow, Main.rand.NextVector2Circular(5, 5) - Projectile.velocity * 0.5f, 0, Color.White, 1f + Main.rand.NextFloat((Size + 1) * 0.5f)).noGravity = true;
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.IceTorch, Main.rand.NextVector2Circular(5, 5) - Projectile.velocity, 0, Color.White, 0.5f + Main.rand.NextFloat((Size + 1))).noGravity = true;

                    break;

                default:

                    break;
            }

            if (Time % 35 == 20)
                Projectile.velocity += Main.rand.NextVector2Circular(2, 2);

            Projectile.localAI[0]++;
            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 20)
            {
                Vector2 radialPoint = Projectile.Center + Projectile.DirectionTo(targetHitbox.Center()).SafeNormalize(Vector2.Zero) * Math.Min(Projectile.Distance(targetHitbox.Center()), Projectile.width);
                if (targetHitbox.Contains(radialPoint.ToPoint()))
                    return true;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //i will allow this one to waste resources because it is weird
            Texture2D texture;

            if (chunkStyle != ThrowableChunkStyle.Default)
                texture = ModContent.Request<Texture2D>(Texture + "_" + chunkStyle.ToString()).Value;
            else
                texture = ModContent.Request<Texture2D>(Texture + "_Rock").Value;

            Texture2D glow = AssetDirectory.Textures.Glow.Value;
            Rectangle frame = texture.Frame(1, 3, 0, (int)Size);
            float power = (float)Math.Sqrt(Utils.GetLerpValue(0, 30, Projectile.localAI[0], true));

            if (chunkStyle != ThrowableChunkStyle.Comet)
            {
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), new Color(13, 13, 41, 0) * 0.7f * power, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * power * (Size + 1) * 3f, 0, 0);
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), new Color(150, 50, 20, 0) * 0.7f * power, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * power * (Size + 1) * 1.33f, 0, 0);
                
                for (int i = 0; i < 8; i++)
                {
                    Vector2 off = new Vector2(2).RotatedBy(MathHelper.TwoPi / 8f * i + Projectile.rotation);
                    Main.EntitySpriteDraw(texture, Projectile.Center + off - Main.screenPosition, frame, new Color(150, 50, 20, 0) * power, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * power, 0, 0);
                }
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.Lerp(lightColor, Color.White, 0.2f), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * power, 0, 0);
            }
            else
            {
                lightColor = Color.White;
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
                {
                    float prog = i / (float)ProjectileID.Sets.TrailCacheLength[Type];
                    Main.EntitySpriteDraw(glow, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, glow.Frame(), Color.Lerp(new Color(53, 90, 200, 0), new Color(13, 13, 41, 0), prog) * 0.3f * (1f - prog), Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * power * (1f - prog) * 2f, 0, 0);
                    Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, frame, Color.Lerp(new Color(53, 90, 200, 0), new Color(13, 13, 41, 0), prog) * 0.2f * (1f - prog), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * power * (1f - prog) * 1.1f, 0, 0);
                }

                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), new Color(13, 90, 200, 0) * 0.8f * power, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * power * (Size + 1) * 1.7f, 0, 0);

                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.Lerp(lightColor, Color.White, 0.5f), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * power, 0, 0);

                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), new Color(13, 13, 41, 0) * 0.7f * power, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * power * (Size + 1) * 3f, 0, 0);
            }

            return false;
        }
    }
}
