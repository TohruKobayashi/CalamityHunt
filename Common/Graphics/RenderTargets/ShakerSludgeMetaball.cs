using System.Linq;
using CalamityHunt.Common.Systems.Metaballs;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Graphics.RenderTargets;

public class ShakerSludgeMetaball : MetaballDrawer
{
    public static ParticleSystem particles;

    public override void Initialize()
    {
        particles = new ParticleSystem();

        content.width = Main.screenWidth;
        content.height = Main.screenHeight;
        content.draw = DrawShapes;

        On_Main.UpdateParticleSystems += UpdateCosmosParticleSystem;
        On_Main.DoDraw_DrawNPCsOverTiles += DrawTarget;
    }

    private void UpdateCosmosParticleSystem(On_Main.orig_UpdateParticleSystems orig, Main self)
    {
        orig(self);
        particles.Update();
    }

    private void DrawShapes(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        particles.Draw(spriteBatch, false);

        foreach (Projectile projectile in Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<DarkSludge>())) {

        }

        spriteBatch.End();
    }

    private void DrawTarget(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
    {
        Effect effect = AssetDirectory.Effects.ShakerSludge.Value;

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.EffectMatrix);

        content.width = Main.screenWidth;
        content.height = Main.screenHeight;
        content.Request();

        if (content.IsReady) {
            Texture2D texture = content.GetTarget();
            Main.spriteBatch.Draw(texture, Vector2.Zero, texture.Frame(), Color.White, 0, Vector2.Zero, 1f, 0, 0);
        }

        Main.spriteBatch.End();

        orig(self);
    }
}
