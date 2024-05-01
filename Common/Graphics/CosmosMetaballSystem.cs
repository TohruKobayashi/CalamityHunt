using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Bosses.Goozma.Projectiles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Arch.Core;
using Arch.Core.Extensions;
using Terraria;
using Terraria.Map;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Common.Graphics
{
    public class CosmosMetaballSystem : ILoadable
    {
        public void Load(Mod mod)
        {
            On_Main.CheckMonoliths += DrawSpaceShapes;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawSpace;

            smokeParticleTexture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Particles/CosmicSmoke", AssetRequestMode.ImmediateLoad).Value;
            blackholeTexture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Bosses/Goozma/Projectiles/BlackHoleBlender", AssetRequestMode.ImmediateLoad).Value;
            blackholeTextureShadow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Bosses/Goozma/Projectiles/BlackHoleBlenderShadow", AssetRequestMode.ImmediateLoad).Value;

            spaceNoise = new Texture2D[]
            {
                AssetDirectory.Textures.Space.Noise0.Value,
                AssetDirectory.Textures.Space.Noise1.Value
            };
        }

        public void Unload()
        {
        }

        public RenderTarget2D SpaceTarget { get; set; }

        private void DrawSpaceShapes(On_Main.orig_CheckMonoliths orig)
        {
            if (SpaceTarget == null || SpaceTarget.IsDisposed)
                SpaceTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);

            else if (SpaceTarget.Size() != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                Main.QueueMainThreadAction(() =>
                {
                    SpaceTarget.Dispose();
                    SpaceTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                });
                return;
            }

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

            Main.graphics.GraphicsDevice.SetRenderTarget(SpaceTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            var query = new QueryDescription().WithAll<Particle, ParticleCosmicSmoke, ParticlePosition, ParticleColor, ParticleScale, ParticleRotation, ParticleActive, ParticleData<string>>();
            var particleSystem = ModContent.GetInstance<ParticleSystem>();
            particleSystem?.ParticleWorld.Query(
                in query,
                (in Entity entity) =>
                {
                    ref readonly var active = ref entity.Get<ParticleActive>();
                    if (!active.Value)
                        return;

                    ref readonly var particle = ref entity.Get<Particle>();
                    ref readonly var smoke = ref entity.Get<ParticleCosmicSmoke>();
                    ref readonly var position = ref entity.Get<ParticlePosition>();
                    ref readonly var color = ref entity.Get<ParticleColor>();
                    ref readonly var scale = ref entity.Get<ParticleScale>();
                    ref readonly var rotation = ref entity.Get<ParticleRotation>();
                    ref readonly var data = ref entity.Get<ParticleData<string>>();

                    switch (data.Value)
                    {
                        case "Cosmos":
                        {
                            Rectangle frame = smokeParticleTexture.Frame(4, 2, smoke.Variant % 4, (int)(smoke.Variant / 4f));
                            float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, smoke.MaxTime * 0.2f, smoke.Time, true));
                            float opacity = Utils.GetLerpValue(smoke.MaxTime * 0.7f, smoke.MaxTime * 0.2f, smoke.Time, true) * Math.Clamp(scale.Value, 0, 1);
                            Main.spriteBatch.Draw(smokeParticleTexture, position.Value - Main.screenPosition, frame, Color.White * 0.5f * opacity * grow, rotation.Value, frame.Size() * 0.5f, scale.Value * grow * 0.5f, 0, 0);
                            break;
                        }

                        case "Interstellar":
                        {
                            Rectangle frame = smokeParticleTexture.Frame(4, 2, smoke.Variant % 4, (int)(smoke.Variant / 4f));
                            float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, smoke.MaxTime * 0.2f, smoke.Time, true));
                            float opacity = Utils.GetLerpValue(smoke.MaxTime * 0.7f, 0, smoke.Time, true) * Math.Clamp(scale.Value, 0, 1);
                            Main.spriteBatch.Draw(smokeParticleTexture, position.Value - Main.screenPosition, frame, Color.White * 0.7f * opacity * color.Value.ToVector4().Length(), rotation.Value, frame.Size() * 0.5f, scale.Value * grow * 0.5f, 0, 0);
                            break;
                        }
                    }
                }
            );

            Effect absorbEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/SpaceAbsorb", AssetRequestMode.ImmediateLoad).Value;
            absorbEffect.Parameters["uRepeats"].SetValue(1f);
            absorbEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Space.Noise0.Value);
            absorbEffect.Parameters["uTexture1"].SetValue(AssetDirectory.Textures.Space.Noise1.Value);

            foreach (Projectile projectile in Main.projectile.Where(n => n.active && n.ModProjectile is BlackHoleBlender))
            {
                BlackHoleBlender blender = projectile.ModProjectile as BlackHoleBlender;
                Texture2D bloom = AssetDirectory.Textures.GlowBig.Value;

                absorbEffect.Parameters["uTime"].SetValue(projectile.localAI[0] * 0.002f % 1f);
                absorbEffect.Parameters["uSize"].SetValue(projectile.scale * new Vector2(8f));

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, absorbEffect);

                Main.spriteBatch.Draw(bloom, projectile.Center - Main.screenPosition, bloom.Frame(), Color.White, 0, bloom.Size() * 0.5f, projectile.scale * 7.5f, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

                Main.spriteBatch.Draw(blackholeTexture, (projectile.Center - Main.screenPosition), blackholeTexture.Frame(), Color.White * projectile.scale * 0.2f, projectile.rotation, blackholeTexture.Size() * 0.5f, projectile.scale * 0.8f, 0, 0);
                Main.spriteBatch.Draw(blackholeTexture, (projectile.Center - Main.screenPosition), blackholeTexture.Frame(), Color.White * projectile.scale * 0.1f, projectile.rotation * 0.8f - 0.5f, blackholeTexture.Size() * 0.5f, projectile.scale * 0.7f, 0, 0);
                Main.spriteBatch.Draw(blackholeTexture, (projectile.Center - Main.screenPosition), blackholeTexture.Frame(), Color.White * projectile.scale * 0.1f, -projectile.rotation * 0.9f - 1f, blackholeTexture.Size() * 0.5f, projectile.scale * 0.5f, 0, 0);
                Main.spriteBatch.Draw(blackholeTexture, (projectile.Center - Main.screenPosition), blackholeTexture.Frame(), Color.White * projectile.scale * 0.1f, projectile.rotation * 0.9f - 0.2f, blackholeTexture.Size() * 0.5f, projectile.scale * 0.9f, 0, 0);
                Main.spriteBatch.Draw(blackholeTextureShadow, (projectile.Center - Main.screenPosition), blackholeTextureShadow.Frame(), Color.White * projectile.scale * 0.1f, projectile.rotation, blackholeTextureShadow.Size() * 0.5f, projectile.scale * 0.5f, 0, 0);
                Main.spriteBatch.Draw(blackholeTextureShadow, (projectile.Center - Main.screenPosition), blackholeTextureShadow.Frame(), Color.White * projectile.scale * 0.1f, projectile.rotation * 0.5f, blackholeTextureShadow.Size() * 0.5f, projectile.scale * 0.7f, 0, 0);
            }

            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            Main.spriteBatch.End();

            orig();
        }

        private static Texture2D smokeParticleTexture;
        private static Texture2D blackholeTexture;
        private static Texture2D blackholeTextureShadow;

        private static Texture2D[] spaceNoise;

        private void DrawSpace(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            //foreach (Particle particle in ParticleSystem.particle.Where(n => n.Active && n is CosmicSmoke && n.data is string))
            //{
            //    CosmicSmoke smoke = particle as CosmicSmoke;
            //    if ((string)smoke.data == "Cosmos")
            //    {
            //        Asset<Texture2D> texture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            //        float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, 7, smoke.time, true));
            //        float opacity = Utils.GetLerpValue(22, 10, smoke.time, true) * Math.Clamp(smoke.scale, 0, 1);
            //        //new Color(20, 13, 11, 0)
            //        Main.spriteBatch.Draw(texture.Value, (smoke.position - Main.screenPosition), null, new Color(13, 7, 20, 0) * 0.1f, smoke.rotation, texture.Size() * 0.5f, smoke.scale * (0.5f + grow * 0.5f) * 4f * opacity, 0, 0);
            //    }
            //}

            Effect absorbEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/SpaceAbsorb", AssetRequestMode.ImmediateLoad).Value;
            absorbEffect.Parameters["uRepeats"].SetValue(1f);
            absorbEffect.Parameters["uTexture0"].SetValue(spaceNoise[0]);
            absorbEffect.Parameters["uTexture1"].SetValue(spaceNoise[1]);

            Texture2D bloom = AssetDirectory.Textures.GlowBig.Value;

            foreach (Projectile projectile in Main.projectile.Where(n => n.active && n.ModProjectile is BlackHoleBlender))
            {
                Main.spriteBatch.Draw(bloom, (projectile.Center - Main.screenPosition), null, new Color(33, 5, 65, 0) * (float)Math.Pow(projectile.scale, 1.5f), projectile.rotation, bloom.Size() * 0.5f, 3.5f, 0, 0);
                Main.spriteBatch.Draw(bloom, projectile.Center - Main.screenPosition, bloom.Frame(), new Color(33, 5, 65, 0), 0, bloom.Size() * 0.5f, projectile.scale * 7.5f, 0, 0);

                absorbEffect.Parameters["uTime"].SetValue(projectile.localAI[0] * 0.002f % 1f);
                absorbEffect.Parameters["uSize"].SetValue(new Vector2(8f));

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, absorbEffect, Main.Transform);

                Main.spriteBatch.Draw(bloom, projectile.Center - Main.screenPosition, bloom.Frame(), new Color(15, 5, 65, 0), 0, bloom.Size() * 0.5f, projectile.scale * 10f, 0, 0);
                Main.spriteBatch.Draw(bloom, projectile.Center - Main.screenPosition, bloom.Frame(), new Color(255, 150, 60, 0), 0, bloom.Size() * 0.5f, projectile.scale * 8f, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
            
            Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/CosmosEffect", AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uTextureClose"].SetValue(AssetDirectory.Textures.Space.Space0.Value);
            effect.Parameters["uTextureFar"].SetValue(AssetDirectory.Textures.Space.Space1.Value);
            effect.Parameters["uPosition"].SetValue((Main.LocalPlayer.oldPosition - Main.LocalPlayer.oldVelocity) * 0.001f);
            effect.Parameters["uParallax"].SetValue(new Vector2(0.5f, 0.2f));
            effect.Parameters["uScrollClose"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * 0.027f % 2f, -Main.GlobalTimeWrappedHourly * 0.017f % 2f));
            effect.Parameters["uScrollFar"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.008f % 2f, Main.GlobalTimeWrappedHourly * 0.0004f % 2f));
            effect.Parameters["uCloseColor"].SetValue(new Color(20, 80, 255).ToVector3());
            effect.Parameters["uFarColor"].SetValue(new Color(110, 50, 200).ToVector3());
            effect.Parameters["uOutlineColor"].SetValue(new Color(10, 5, 45, 0).ToVector4());
            effect.Parameters["uImageRatio"].SetValue(new Vector2(Main.screenWidth / (float)Main.screenHeight, 1f));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);

            Main.spriteBatch.Draw(SpaceTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1f, 0, 0);

            Main.spriteBatch.End();

            //if (Main.UpdateTimeAccumulator < 0.0166666)
            //{
            //    if (Main.mouseLeft)
            //    {
            //        for (int i = 0; i < 5; i++)
            //        {
            //            Particle smoke = Particle.NewParticle(ModContent.GetInstance<CosmicSmoke>(), Main.MouseWorld, Main.rand.NextVector2Circular(8, 8), Color.White, 1f + Main.rand.NextFloat(2f));
            //            smoke.data = "Cosmos";
            //        }
            //    }

            //    if (Main.mouseRight)
            //    {
            //        for (int i = 0; i < 5; i++)
            //        {
            //            Color drawColor = new GradientColor(SlimeUtils.GoozColorArray, 0.1f, 0.1f).Value;
            //            drawColor.A = 0;
            //            Particle smoke = Particle.NewParticle(ModContent.GetInstance<CosmicSmoke>(), Main.MouseWorld, Main.rand.NextVector2Circular(5, 5), drawColor, 1f + Main.rand.NextFloat(2f));
            //        }
            //    }
            //}

            orig(self);
        }
    }
}
