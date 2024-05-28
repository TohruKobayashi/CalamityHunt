//using CalamityHunt.Content.Bosses.Goozma;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using CalamityHunt.Common.Systems.Particles;
//using ReLogic.Content;
//using System;
//using Terraria;
//using Terraria.GameContent;
//using Terraria.ModLoader;

//namespace CalamityHunt.Content.Particles
//{
//    public class CrackSpot : Particle
//    {
//        private Vector2 rand0;
//        private Vector2 rand1;
//        private float time;

//        public override void OnSpawn()
//        {
//            behindEntities = true;
//            rotation = Main.rand.NextFloat(-2f, 2f);
//            rand0 = new Vector2(Main.rand.NextFloat(-16f, 16f), Main.rand.NextFloat(-16f, 16f));
//            rand1 = new Vector2(Main.rand.NextFloat(-16f, 16f), Main.rand.NextFloat(-16f, 16f));
//            if (data is string)
//                if ((string)data == "GoozmaColor")
//                    color = new GradientColor(SlimeUtils.GoozColorArray, 0.1f, 0.1f).ValueAt(time);
//        }
//        public override void Update()
//        {
//            if (data is string)
//                if ((string)data == "GoozmaColor")
//                    color = new GradientColor(SlimeUtils.GoozColorArray, 0.1f, 0.1f).ValueAt(time);
//            velocity = Vector2.Zero;
//            time++;

//            if (time > 45 + scale * 0.5f)
//                Active = false;
//        }

//        public override void Draw(SpriteBatch spriteBatch)
//        {
//            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");

//            float newScale = (float)Math.Sqrt(Utils.GetLerpValue(-7, 10, time, true));
//            float power = (float)Math.Pow(Utils.GetLerpValue(45 + scale * 0.5f, 0, time, true), 0.3f);

//            Color drawColor = color;
//            drawColor.A = 0;
//            Effect crackShader = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/SpaceCrack", AssetRequestMode.ImmediateLoad).Value;
//            crackShader.Parameters["uTexture0"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Noise/Noise0").Value);
//            crackShader.Parameters["uTexture1"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Noise/Noise3").Value);
//            crackShader.Parameters["random0"].SetValue(rand0 * 16f);
//            crackShader.Parameters["random1"].SetValue(rand1 * 16f);
//            crackShader.Parameters["uStrength"].SetValue(power * newScale);
//            crackShader.Parameters["uSize"].SetValue(scale * 1.33f * (0.2f + newScale * 0.8f));
//            crackShader.Parameters["uColor"].SetValue(drawColor.ToVector4());
//            if (data is string)
//            {
//                if ((string)data == "GoozmaBlack")
//                {
//                    newScale = (0.3f + (float)Math.Cbrt(Utils.GetLerpValue(0, 35, time, true)) * 0.7f) * (float)Math.Pow(Utils.GetLerpValue(41 + scale * 0.5f, 10, time, true), 0.5f);
//                    crackShader.Parameters["uTexture0"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Noise/Noise0").Value);
//                    crackShader.Parameters["uTexture1"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Noise/Noise2").Value);
//                    crackShader.Parameters["uSize"].SetValue(scale * newScale * (0.5f + time * 0.05f));
//                    crackShader.Parameters["uColor"].SetValue(Color.Black.ToVector4());
//                }                
//                if ((string)data == "GoozmaColor")
//                {
//                    newScale = (float)Math.Sqrt(Utils.GetLerpValue(0, 35, time, true));
//                    crackShader.Parameters["uTexture0"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Noise/Noise0").Value);
//                    crackShader.Parameters["uTexture1"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Noise/Noise2").Value);
//                    crackShader.Parameters["uStrength"].SetValue((float)Math.Pow(Utils.GetLerpValue(40 + scale * 0.5f, 0, time, true), 0.5f));
//                    crackShader.Parameters["uSize"].SetValue(scale * 2f * (1f - newScale * 0.015f) - time);
//                    crackShader.Parameters["uColor"].SetValue(Color.Black.ToVector4());
//                    spriteBatch.Draw(bloom.Value, position - Main.screenPosition, null, drawColor * 0.4f * Utils.GetLerpValue(40 + scale * 0.3f, 25, time, true) * newScale, rotation, bloom.Size() * 0.5f, scale * newScale * 0.2f, 0, 0);
//                }                
//            }
//            else
//                spriteBatch.Draw(bloom.Value, position - Main.screenPosition, null, drawColor * 0.5f * Utils.GetLerpValue(30 + scale * 0.3f, 5, time, true) * newScale, rotation, bloom.Size() * 0.5f, scale * newScale * 0.2f, 0, 0);

//            spriteBatch.End();
//            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, crackShader, Main.Transform);

//            spriteBatch.Draw(TextureAssets.BlackTile.Value, position - Main.screenPosition, null, drawColor, rotation, TextureAssets.BlackTile.Size() * 0.5f, scale * newScale, 0, 0);

//            spriteBatch.End();
//            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

//        }
//    }
//}
