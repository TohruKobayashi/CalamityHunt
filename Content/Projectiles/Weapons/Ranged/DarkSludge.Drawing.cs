using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using log4net.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public partial class DarkSludge : ModProjectile
    {
        public override void Load()
        {
            On_Main.CheckMonoliths += DrawShapes;
            On_Main.DoDraw_Tiles_Solid += DrawSludge;
        }

        public static RenderTarget2D SludgeTarget { get; set; }

        private void DrawShapes(On_Main.orig_CheckMonoliths orig)
        {
            if (SludgeTarget == null || SludgeTarget.IsDisposed)
                SludgeTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);

            else if (SludgeTarget.Size() != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                Main.QueueMainThreadAction(() =>
                {
                    SludgeTarget.Dispose();
                    SludgeTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                });
                return;
            }

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

            Main.graphics.GraphicsDevice.SetRenderTarget(SludgeTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            foreach (Projectile projectile in Main.projectile.Where(n => n.active && n.ai[0] > 0 && n.type == ModContent.ProjectileType<DarkSludge>()))
            {
                Texture2D texture = ModContent.Request<Texture2D>(projectile.ModProjectile.Texture).Value;
                projectile.frame = (projectile.ai[1] == 1 || projectile.ai[2] > -1 ? 2 : 0) + (int)projectile.localAI[0];
                Rectangle frame = texture.Frame(4, 1, projectile.frame, 0);

                Vector2 squish = new Vector2(1f - projectile.velocity.Length() * 0.01f, 1f + projectile.velocity.Length() * 0.01f) * 0.8f;
                if (projectile.frame > 1)
                    squish = new Vector2(2f + MathF.Sin(projectile.ai[0] * 0.1f) * 0.1f, 2.3f + MathF.Cos(projectile.ai[0] * 0.1f) * 0.2f) * 0.7f;
                
                Color color = Lighting.GetColor(projectile.Center.ToTileCoordinates());
                Main.spriteBatch.Draw(texture, (projectile.Bottom - Main.screenPosition) / 2f, frame, color, projectile.rotation - MathHelper.PiOver2, frame.Size() * new Vector2(0.5f, 0.8f), projectile.scale * squish * 0.5f, 0, 0);
            }

            if (CalamityHunt.Particles is null) {
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
                Main.spriteBatch.End();

                return;
            }

            foreach (DarkSludgeChunk particle in CalamityHunt.Particles.Particles.Where(n => n is DarkSludgeChunk))
            {
                if (particle.time > 0)
                {
                    Asset<Texture2D> texture = ModContent.Request<Texture2D>(particle.Texture);
                    Rectangle frame = texture.Frame(4, 1, particle.variant, 0);
                    Vector2 squish = new Vector2(1f - particle.Velocity.Length() * 0.01f, 1f + particle.Velocity.Length() * 0.01f);
                    float grow = (float)Math.Sqrt(Utils.GetLerpValue(-20, 40, particle.time, true));
                    if (particle.stuck)
                    {
                        grow = 1f;
                        frame = texture.Frame(4, 1, particle.variant + 2, 0);
                        squish = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(20, 0, particle.time, true)) * 0.33f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(20, 0, particle.time, true)) * 0.33f);
                    }

                    Color color = Lighting.GetColor(particle.Position.ToTileCoordinates());
                    Main.spriteBatch.Draw(texture.Value, (particle.Position - Main.screenPosition) / 2f, frame, color, particle.Rotation, frame.Size() * new Vector2(0.5f, 0.84f), particle.Scale * grow * squish * 0.5f, 0, 0);
                }
            }

            //foreach (FlameParticle particle in ParticleSystem.particle.Where(n => n.Active && n is FlameParticle && n.data is string))
            //{
            //    if ((string)particle.data == "Sludge")
            //    {
            //        Asset<Texture2D> texture = ModContent.Request<Texture2D>(particle.Texture + "Sludge");
            //        Rectangle frame = texture.Frame(4, 2, particle.variant % 4, (int)(particle.variant / 4f));
            //        float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, particle.maxTime * 0.3f, particle.time, true));
            //        float opacity = Utils.GetLerpValue(particle.maxTime * 0.8f, particle.maxTime * 0.3f, particle.time, true) * Math.Clamp(particle.Scale, 0, 1);

            //        Color color = Lighting.GetColor(particle.Position.ToTileCoordinates());
            //        Main.spriteBatch.Draw(texture.Value, (particle.Position - Main.screenPosition) / 2f, frame, color * opacity, particle.Rotation - MathHelper.PiOver2, frame.Size() * 0.5f, particle.Scale * grow * 0.5f, 0, 0);
            //    }
            //}

            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            Main.spriteBatch.End();

            orig();
        }

        private void DrawSludge(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        {
            Texture2D baseTex = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Extra/DarkSludgeTexture").Value;
            Texture2D glowTex = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Extra/DarkSludgeTextureGlow").Value;
            Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/SludgeEffect", AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uPosition"].SetValue(Main.screenPosition * 0.5f);
            effect.Parameters["uBaseTexture"].SetValue(baseTex);
            effect.Parameters["uGlowTexture"].SetValue(glowTex);
            effect.Parameters["uSize"].SetValue(Vector2.One * 5);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.002f % 1f);
            effect.Parameters["uScreenSize"].SetValue(Main.ScreenSize.ToVector2());

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);

            if (SludgeTarget != null)
                Main.spriteBatch.Draw(SludgeTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, 0, 0);

            Main.spriteBatch.End();

            orig(self);
        }

    }
}
