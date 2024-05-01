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
using Arch.Core;
using Arch.Core.Extensions;
using Terraria;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

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

            var query = new QueryDescription().WithAll<Particle, ParticleVelocity, ParticlePosition, ParticleRotation, ParticleScale, ParticleDarkSludgeChunk>();
            var particleSystem = ModContent.GetInstance<ParticleSystem>();
            particleSystem.ParticleWorld.Query(
                in query,
                (in Entity entity) =>
                {
                    ref readonly var particle = ref entity.Get<Particle>();
                    ref readonly var velocity = ref entity.Get<ParticleVelocity>();
                    ref readonly var position = ref entity.Get<ParticlePosition>();
                    ref readonly var rotation = ref entity.Get<ParticleRotation>();
                    ref readonly var scale = ref entity.Get<ParticleScale>();
                    ref readonly var darkSludgeChunk = ref entity.Get<ParticleDarkSludgeChunk>();

                    if (darkSludgeChunk.Time <= 0)
                        return;

                    Asset<Texture2D> texture = ModContent.Request<Texture2D>(particle.Behavior.Texture);
                    Rectangle frame = texture.Frame(4, 1, darkSludgeChunk.Variant, 0);
                    Vector2 squish = new Vector2(1f - velocity.Value.Length() * 0.01f, 1f + velocity.Value.Length() * 0.01f);
                    float grow = (float)Math.Sqrt(Utils.GetLerpValue(-20, 40, darkSludgeChunk.Time, true));
                    if (darkSludgeChunk.Stuck)
                    {
                        grow = 1f;
                        frame = texture.Frame(4, 1, darkSludgeChunk.Variant + 2, 0);
                        squish = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(20, 0, darkSludgeChunk.Time, true)) * 0.33f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(20, 0, darkSludgeChunk.Time, true)) * 0.33f);
                    }

                    Color color = Lighting.GetColor(position.Value.ToTileCoordinates());
                    Main.spriteBatch.Draw(texture.Value, (position.Value - Main.screenPosition) / 2f, frame, color, rotation.Value, frame.Size() * new Vector2(0.5f, 0.84f), scale.Value * grow * squish * 0.5f, 0, 0);
                }
            );
            
            query = new QueryDescription().WithAll<Particle, ParticlePosition, ParticleRotation, ParticleScale, ParticleData<string>, ParticleMegaFlame>();
            particleSystem.ParticleWorld.Query(
                in query,
                (in Entity entity) =>
                {
                    ref readonly var particle = ref entity.Get<Particle>();
                    ref readonly var position = ref entity.Get<ParticlePosition>();
                    ref readonly var rotation = ref entity.Get<ParticleRotation>();
                    ref readonly var scale = ref entity.Get<ParticleScale>();
                    ref readonly var stringData = ref entity.Get<ParticleData<string>>();
                    ref readonly var megaFlame = ref entity.Get<ParticleMegaFlame>();

                    if (stringData.Value != "Sludge")
                        return;

                    Asset<Texture2D> texture = ModContent.Request<Texture2D>(particle.Behavior.Texture + "Sludge");
                    Rectangle frame = texture.Frame(4, 2, megaFlame.Variant % 4, (int)(megaFlame.Variant / 4f));
                    float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, megaFlame.MaxTime * 0.3f, megaFlame.Time, true));
                    float opacity = Utils.GetLerpValue(megaFlame.MaxTime * 0.8f, megaFlame.MaxTime * 0.3f, megaFlame.Time, true) * Math.Clamp(scale.Value, 0, 1);

                    Color color = Lighting.GetColor(position.Value.ToTileCoordinates());
                    Main.spriteBatch.Draw(texture.Value, (position.Value - Main.screenPosition) / 2f, frame, color * opacity, rotation.Value - MathHelper.PiOver2, frame.Size() * 0.5f, scale.Value * grow * 0.5f, 0, 0);
                }
            );

            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            Main.spriteBatch.End();

            orig();
        }

        private void DrawSludge(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        {
            Texture2D baseTex = AssetDirectory.Textures.Goozma.DarkSludge.Value;
            Texture2D glowTex = AssetDirectory.Textures.Goozma.DarkSludgeGlow.Value;
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
