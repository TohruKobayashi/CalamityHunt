using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles
{
    public class GelCrystalShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 500;
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(4);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[0] == 0) {
                Projectile.velocity.Y += 0.04f;
            }

            if (Main.rand.NextBool(2)) {
                int dustType = Utils.SelectRandom(Main.rand, DustID.PinkCrystalShard, DustID.BlueCrystalShard, DustID.PurpleCrystalShard);
                //Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(15), 30, 30, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, Color.White, Main.rand.NextFloat());
                //dust.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.DarkBlue, Color.HotPink, (float)Math.Sqrt(Math.Sin(Projectile.timeLeft * 0.1f))).ToVector3() * 0.2f);

            if (Projectile.timeLeft < 20) {
                Projectile.velocity *= 0.9f;
            }
            
            if (Projectile.velocity.Y > 7) {
                Projectile.velocity.Y = 7;
            }
            Projectile.ai[1]++;
        }

        public VertexStrip strip;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = AssetDirectory.Textures.Glow[0].Value;
            Rectangle frame = texture.Frame(4, 1, Projectile.frame, 0);
            Vector2 direction = Projectile.rotation.ToRotationVector2() * 40;

            strip ??= new VertexStrip();

            strip.PrepareStrip(Projectile.oldPos, Projectile.oldRot, StripColor, StripWidth, -Main.screenPosition + Projectile.Size * 0.5f - direction.RotatedBy(-MathHelper.PiOver2), Projectile.oldPos.Length);

            //todo get this to work replace w custom trail
            Effect effect = AssetDirectory.Effects.CometKunaiTrail.Value;
            effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            effect.Parameters["uColor"].SetValue(new Color(0, 80, 255, 0).ToVector4());
            effect.Parameters["uTexture0"].SetValue(TextureAssets.Extra[197].Value);
            effect.Parameters["uTextureNoise0"].SetValue(AssetDirectory.Textures.Noise[16].Value);
            effect.Parameters["uTextureNoise1"].SetValue(AssetDirectory.Textures.Noise[13].Value);
            effect.Parameters["uTime"].SetValue(-(Main.GlobalTimeWrappedHourly * 2f % 1f));
            effect.CurrentTechnique.Passes[0].Apply();

            strip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            Color darkBack = Color.BlueViolet * 0.15f;
            darkBack.A /= 2;
            Color bloom = Color.MediumVioletRed * 0.25f;
            bloom.A = 0;

            Color backColor = Color.SeaGreen * 0.5f;
            backColor.A = 200;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, backColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * 1.4f, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, darkBack, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * 1.66f, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, bloom, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * new Vector2(1.5f, 2f), 0, 0);

            return false;
        }

        public float StripWidth(float x) => 225f * (1 - x) * Utils.GetLerpValue(2, 8, Projectile.ai[1], true) * MathF.Sqrt(Utils.GetLerpValue(0f, 0.1f, x, true));

        public Color StripColor(float x) => Color.Lerp(new Color(10, 140, 255, 0), new Color(10, 170, 255, 128), Utils.GetLerpValue(0.9f, 0.7f, x, true));
    }
}
