//#nullable enable

//using System;
//using Arch.Core;
//using Arch.Core.Extensions;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Entity = Arch.Core.Entity;

//namespace CalamityHunt.Common.Systems.ParticlesOld;

//[Autoload(Side = ModSide.Client)]
//public sealed class ParticleSystem : ModSystem
//{
//    public World ParticleWorld { get; } = World.Create();

//    public override void OnModLoad()
//    {
//        On_Main.UpdateParticleSystems += UpdateParticles;
//        On_Main.DoDraw_DrawNPCsOverTiles += DrawParticlesUnderEntities;
//        On_Main.DrawDust += DrawParticles;

//    }

//    public override void OnModUnload()
//    {
//        On_Main.UpdateParticleSystems -= UpdateParticles;
//        On_Main.DoDraw_DrawNPCsOverTiles -= DrawParticlesUnderEntities;
//        On_Main.DrawDust -= DrawParticles;
//    }

//    public void UpdateParticles()
//    {
//        if (Main.dedServ || Main.gamePaused || Main.netMode == NetmodeID.Server)
//            return;

//        var query = new QueryDescription().WithAll<Particle, ParticlePosition, ParticleVelocity, ParticleActive>();
//        ParticleWorld.Query(
//            in query, 
//            (in Entity entity) =>
//            {
//                ref var particle = ref entity.Get<Particle>();
//                ref var position = ref entity.Get<ParticlePosition>();
//                ref var velocity = ref entity.Get<ParticleVelocity>();
//                ref var active = ref entity.Get<ParticleActive>();

//                position.Value += velocity.Value;
//                particle.Behavior.Update(in entity);
//                if (!active.Value)
//                    ParticleWorld.Destroy(entity);
//            }
//        );
//    }

//    public void DrawParticlesUnderEntities(SpriteBatch spriteBatch)
//    {
//        if (Main.dedServ || Main.gameMenu || Main.netMode == NetmodeID.Server)
//            return;

//        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
//        Rectangle value = new Rectangle((int)Main.screenPosition.X - Main.screenWidth, (int)Main.screenPosition.Y - Main.screenHeight, Main.screenWidth * 2, Main.screenHeight * 2);

//        var query = new QueryDescription().WithAll<Particle, ParticlePosition, ParticleDrawBehindEntities>().WithNone<ParticleShader>();
//        ParticleWorld.Query(
//            in query,
//            (in Entity entity) =>
//            {
//                ref var particle = ref entity.Get<Particle>();
//                ref var position = ref entity.Get<ParticlePosition>();

//                if (new Rectangle((int)position.Value.X - 3, (int)position.Value.Y - 3, 6, 6).Intersects(value))
//                    particle.Behavior.Draw(in entity, spriteBatch);
//            }
//        );

//        spriteBatch.End();
//        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

//        query = new QueryDescription().WithAll<Particle, ParticlePosition, ParticleDrawBehindEntities, ParticleShader>();
//        ParticleWorld.Query(
//            in query,
//            (in Entity entity) =>
//            {
//                ref var particle = ref entity.Get<Particle>();
//                ref var position = ref entity.Get<ParticlePosition>();
//                ref var shader = ref entity.Get<ParticleShader>();

//                if (!new Rectangle((int)position.Value.X - 3, (int)position.Value.Y - 3, 6, 6).Intersects(value))
//                    return;

//                shader.Value.Apply(null);
//                particle.Behavior.Draw(in entity, spriteBatch);
//                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
//            }
//        );

//        spriteBatch.End();
//    }

//    public void DrawParticles(SpriteBatch spriteBatch)
//    {
//        if (Main.dedServ || Main.gameMenu || Main.netMode == NetmodeID.Server)
//            return;

//        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
//        Rectangle value = new Rectangle((int)Main.screenPosition.X - Main.screenWidth, (int)Main.screenPosition.Y - Main.screenHeight, Main.screenWidth * 2, Main.screenHeight * 2);

//        var query = new QueryDescription().WithAll<Particle, ParticlePosition>().WithNone<ParticleShader, ParticleDrawBehindEntities>();
//        ParticleWorld.Query(
//            in query,
//            (in Entity entity) =>
//            {
//                ref var particle = ref entity.Get<Particle>();
//                ref var position = ref entity.Get<ParticlePosition>();

//                if (new Rectangle((int)position.Value.X - 3, (int)position.Value.Y - 3, 6, 6).Intersects(value))
//                    particle.Behavior.Draw(in entity, spriteBatch);
//            }
//        );

//        spriteBatch.End();
//        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

//        query = new QueryDescription().WithAll<Particle, ParticlePosition, ParticleShader>().WithNone<ParticleDrawBehindEntities>();
//        ParticleWorld.Query(
//            in query,
//            (in Entity entity) =>
//            {
//                ref var particle = ref entity.Get<Particle>();
//                ref var position = ref entity.Get<ParticlePosition>();
//                ref var shader = ref entity.Get<ParticleShader>();

//                if (!new Rectangle((int)position.Value.X - 3, (int)position.Value.Y - 3, 6, 6).Intersects(value))
//                    return;

//                shader.Value?.Apply(null);
//                particle.Behavior.Draw(in entity, spriteBatch);
//                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
//            }
//        );

//        spriteBatch.End();
//    }

//    private void DrawParticlesUnderEntities(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
//    {
//        DrawParticlesUnderEntities(Main.spriteBatch);

//        orig(self);
//    }

//    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
//    {
//        DrawParticles(Main.spriteBatch);

//        orig(self);
//    }

//    private void UpdateParticles(On_Main.orig_UpdateParticleSystems orig, Main self)
//    {
//        orig(self);
//        UpdateParticles();
//    }
//}
