using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using System.Linq;
using Terraria.GameContent.Bestiary;
using System.Reflection;

namespace CalamityHunt.Content.Bosses.Goozma;

public class GoozmaCordTargetContent : ARenderTargetContentByRequest
{
    public Goozma Host;

    protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        Vector2 targetSize = new Vector2(1024);
        Matrix realMatrix = Matrix.CreateOrthographicOffCenter(0, targetSize.X, targetSize.Y, 0, -1, 1);
        PrepareARenderTarget_AndListenToEvents(ref _target, device, (int)targetSize.X, (int)targetSize.Y, RenderTargetUsage.PreserveContents);
        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);
        List<Vector2> positions = new List<Vector2>();
        List<float> rotations = new List<float>();

        float partitions = 50;
        for (int i = 0; i < partitions; i++)
        {
            Vector2 offset = new Vector2(20 + Utils.GetLerpValue(0, partitions / 2.1f, i, true) * Utils.GetLerpValue(partitions * 1.3f, partitions / 3f, i, true) * (140 + (float)Math.Sin((Host.NPC.localAI[0] * 0.125f - i / (partitions * 0.036f)) % MathHelper.TwoPi) * 18 * (i / partitions)), 0).RotatedBy(MathHelper.SmoothStep(0.15f, -3.3f, i / partitions));
            offset.X *= -1;// Host.NPC.direction;
            offset -= Host.drawVelocity * Utils.GetLerpValue(partitions / 3f, partitions, i, true);
            offset *= 0.5f;
            positions.Add(new Vector2(0, 8).RotatedBy(Host.NPC.rotation) + offset.RotatedBy(Host.NPC.rotation));
            rotations.Add(offset.RotatedBy(Host.NPC.rotation).ToRotation() - MathHelper.PiOver2 * (i / partitions) * -1);
        }

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

        Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/GoozmaCordMap", AssetRequestMode.ImmediateLoad).Value;
        effect.Parameters["uTransformMatrix"].SetValue(realMatrix);
        effect.Parameters["uTime"].SetValue(Host.NPC.localAI[0] * 0.005f % 1f);
        effect.Parameters["uTexture"].SetValue(AssetDirectory.Textures.Goozma.LiquidTrail.Value);
        effect.Parameters["uMap"].SetValue(AssetDirectory.Textures.Goozma.ColorMap.Value);
        effect.CurrentTechnique.Passes[0].Apply();

        VertexStrip cord = new VertexStrip();

        Color ColorFunc(float progress) => Color.White;
        float WidthFunc(float progress) => MathF.Pow(Utils.GetLerpValue(1.1f, 0.1f, progress, true), 0.7f) * 18f;

        cord.PrepareStripWithProceduralPadding(positions.ToArray(), rotations.ToArray(), ColorFunc, WidthFunc, targetSize * 0.5f, true);
        cord.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        spriteBatch.End();

        device.SetRenderTarget(null);
        _wasPrepared = true;
    }
}

//    public partial class Goozma : ModNPC
//    {

//        public void DrawCordShapes(On_Main.orig_CheckMonoliths orig)
//        {
//            orig();
        
//            bool switchedTargets = false;

//            //WE DO NOT USE GAMEVIEWMATRIX TRANSFORMATIONMATRIX because that ruins the pixelated thing for reasons. Just use the ortographic projection witout the view matrix
//            Vector2 targetSize = new Vector2(1000);
//            Matrix realMatrix = Matrix.CreateOrthographicOffCenter(0, targetSize.X, targetSize.Y, 0, -1, 1);
//            Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/GoozmaCordMap", AssetRequestMode.ImmediateLoad).Value;

//            foreach (NPC npc in Main.npc.ToArray().Where(n => n.active && n.ModNPC is Goozma))
//            {
//                Goozma targetOwner = npc.ModNPC as Goozma;

//                if (targetOwner.cordTarget == null || targetOwner.cordTarget.IsDisposed)
//                    targetOwner.cordTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)targetSize.X, (int)targetSize.Y);

//                else if (targetOwner.cordTarget.Size() != targetSize)
//                {
//                    Main.QueueMainThreadAction(() =>
//                    {
//                        targetOwner.cordTarget.Dispose();
//                        targetOwner.cordTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)targetSize.X, (int)targetSize.Y);
//                    });
//                    return;
//                }

//                if (targetOwner.cordTarget != null)
//                {
//                    //Be sure to switch to the **TARGET OWNER's** cordTarget, not just cordTarget
//                    //If you just use cordTarget that'll use the one from the autoloaded instance who loaded the detour
//                    Main.graphics.GraphicsDevice.SetRenderTarget(targetOwner.cordTarget);
//                    Main.graphics.GraphicsDevice.Clear(Color.Transparent);
//                    if (!switchedTargets)
//                    {
//                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null);
//                        switchedTargets = true;
//                    }

//                    List<Vector2> positions = new List<Vector2>();
//                    List<float> rotations = new List<float>();

//                    float partitions = 50;
//                    for (int i = 0; i < partitions; i++)
//                    {
//                        Vector2 offset = new Vector2(20 + Utils.GetLerpValue(0, partitions / 2.1f, i, true) * Utils.GetLerpValue(partitions * 1.3f, partitions / 3f, i, true) * (140 + (float)Math.Sin((npc.localAI[0] * 0.125f - i / (partitions * 0.036f)) % MathHelper.TwoPi) * 18 * (i / partitions)), 0).RotatedBy(Math.Clamp((float)Math.Cbrt(npc.scale), 0, 1) * MathHelper.SmoothStep(0.15f, -3.3f, i / partitions));
//                        offset.X *= npc.direction;
//                        offset -= targetOwner.drawVelocity * Utils.GetLerpValue(partitions / 3f, partitions, i, true);
//                        offset *= 0.5f;
//                        positions.Add((new Vector2(0, 8).RotatedBy(npc.rotation) + offset.RotatedBy(npc.rotation)));
//                        rotations.Add(offset.RotatedBy(npc.rotation).ToRotation() - MathHelper.PiOver2 * (i / partitions) * npc.direction);
//                    }

//                    effect.Parameters["uTransformMatrix"].SetValue(realMatrix);
//                    effect.Parameters["uTime"].SetValue(npc.localAI[0] * 0.005f % 1f);
//                    effect.Parameters["uTexture"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/LiquidTrail").Value);
//                    effect.Parameters["uMap"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GoozmaColorMap").Value);
//                    effect.CurrentTechnique.Passes[0].Apply();

//                    VertexStrip cord = new VertexStrip();

//                    Color ColorFunc(float progress) => Color.White;
//                    float WidthFunc(float progress) => MathF.Pow(Utils.GetLerpValue(1.1f, 0.1f, progress, true), 0.7f) * 18f;

//                    cord.PrepareStripWithProceduralPadding(positions.ToArray(), rotations.ToArray(), ColorFunc, WidthFunc, targetSize * 0.5f, true);
//                    cord.DrawTrail();
//                    Main.pixelShader.CurrentTechnique.Passes[0].Apply();
//                }
//            }

//            if (nPCsToDrawCordOn != null)
//            {
//                foreach (NPC npc in nPCsToDrawCordOn.ToArray())
//                {
//                    Goozma targetOwner = npc.ModNPC as Goozma;

//                    if (targetOwner.cordTarget == null || targetOwner.cordTarget.IsDisposed)
//                        targetOwner.cordTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)targetSize.X, (int)targetSize.Y);

//                    else if (targetOwner.cordTarget.Size() != targetSize)
//                    {
//                        Main.QueueMainThreadAction(() =>
//                        {
//                            targetOwner.cordTarget.Dispose();
//                            targetOwner.cordTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)targetSize.X, (int)targetSize.Y);
//                        });
//                        return;
//                    }

//                    if (targetOwner.cordTarget != null)
//                    {
//                        //Be sure to switch to the **TARGET OWNER's** cordTarget, not just cordTarget
//                        //If you just use cordTarget that'll use the one from the autoloaded instance who loaded the detour
//                        Main.graphics.GraphicsDevice.SetRenderTarget(targetOwner.cordTarget);
//                        Main.graphics.GraphicsDevice.Clear(Color.Transparent);
//                        if (!switchedTargets)
//                        {
//                            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null);
//                            switchedTargets = true;
//                        }

//                        List<Vector2> positions = new List<Vector2>();
//                        List<float> rotations = new List<float>();

//                        float partitions = 50;
//                        for (int i = 0; i < partitions; i++)
//                        {
//                            Vector2 offset = new Vector2(20 + Utils.GetLerpValue(0, partitions / 2.1f, i, true) * Utils.GetLerpValue(partitions * 1.3f, partitions / 3f, i, true) * (140 + (float)Math.Sin((npc.localAI[0] * 0.125f - i / (partitions * 0.036f)) % MathHelper.TwoPi) * 18 * (i / partitions)), 0).RotatedBy(MathHelper.SmoothStep(0.15f, -3.3f, i / partitions));
//                            offset.X *= npc.direction;
//                            offset -= targetOwner.drawVelocity * Utils.GetLerpValue(partitions / 3f, partitions, i, true);
//                            offset *= 0.5f;
//                            positions.Add(new Vector2(0, 8).RotatedBy(npc.rotation) + offset.RotatedBy(npc.rotation));
//                            rotations.Add(offset.RotatedBy(npc.rotation).ToRotation() - MathHelper.PiOver2 * (i / partitions) * npc.direction);
//                        }

//                        effect.Parameters["uTransformMatrix"].SetValue(realMatrix);
//                        effect.Parameters["uTime"].SetValue(npc.localAI[0] * 0.005f % 1f);
//                        effect.Parameters["uTexture"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/LiquidTrail").Value);
//                        effect.Parameters["uMap"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GoozmaColorMap").Value);
//                        effect.CurrentTechnique.Passes[0].Apply();

//                        VertexStrip cord = new VertexStrip();

//                        Color ColorFunc(float progress) => Color.White;
//                        float WidthFunc(float progress) => MathF.Pow(Utils.GetLerpValue(1.1f, 0.1f, progress, true), 0.7f) * 18f;

//                        cord.PrepareStripWithProceduralPadding(positions.ToArray(), rotations.ToArray(), ColorFunc, WidthFunc, targetSize * 0.5f, true);
//                        cord.DrawTrail();
//                        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
//                    }
//                }
//            }
        

//            //int goozmaID = ModContent.NPCType<Goozma>();
//            //foreach (BestiaryEntry entry in Main.BestiaryDB.Entries.Where(n => n == Main.BestiaryDB.FindEntryByNPCID(goozmaID)))
//            //{
//            //    NPC bestiaryNPC = (NPC)(entry.Icon.GetType().GetField("_npcCache", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(entry.Icon));
//            //    Goozma bestiaryTargetOwner = bestiaryNPC.ModNPC as Goozma;

//            //    if (bestiaryTargetOwner.cordTarget == null || bestiaryTargetOwner.cordTarget.IsDisposed)
//            //        bestiaryTargetOwner.cordTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);

//            //    else if (bestiaryTargetOwner.cordTarget.Size() != new Vector2(Main.screenWidth / 2, Main.screenHeight / 2))
//            //    {
//            //        Main.QueueMainThreadAction(() =>
//            //        {
//            //            bestiaryTargetOwner.cordTarget.Dispose();
//            //            bestiaryTargetOwner.cordTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);
//            //        });
//            //        return;
//            //    }

//            //    if (bestiaryTargetOwner.cordTarget != null)
//            //    {
//            //        Main.graphics.GraphicsDevice.SetRenderTarget(bestiaryTargetOwner.cordTarget);
//            //        Main.graphics.GraphicsDevice.Clear(Color.Transparent);
//            //        if (!switchedTargets)
//            //        {
//            //            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
//            //            switchedTargets = true;
//            //        }

//            //        List<Vector2> positions = new List<Vector2>();
//            //        List<float> rotations = new List<float>();

//            //        float partitions = 50;
//            //        for (int i = 0; i < partitions; i++)
//            //        {
//            //            Vector2 offset = new Vector2(20 + Utils.GetLerpValue(0, partitions / 2.1f, i, true) * Utils.GetLerpValue(partitions * 1.3f, partitions / 3f, i, true) * (140 + (float)Math.Sin((bestiaryNPC.localAI[0] * 0.125f - i / (partitions * 0.04f)) % MathHelper.TwoPi) * 16 * (i / partitions)), 0).RotatedBy(Math.Clamp((float)Math.Cbrt(bestiaryNPC.scale), 0, 1) * MathHelper.SmoothStep(0.15f, -3.5f, i / partitions));
//            //            offset.X *= bestiaryNPC.direction;
//            //            offset -= bestiaryNPC.velocity * Utils.GetLerpValue(partitions / 3f, partitions, i, true);
//            //            positions.Add((new Vector2(0, 20).RotatedBy(bestiaryNPC.rotation) + offset.RotatedBy(bestiaryNPC.rotation)) * bestiaryNPC.scale);
//            //            rotations.Add(offset.RotatedBy(bestiaryNPC.rotation).ToRotation() - MathHelper.PiOver2 * (i / partitions) * bestiaryNPC.direction);
//            //        }

//            //        effect.Parameters["uTransformMatrix"].SetValue(realMatrix);
//            //        effect.Parameters["uTime"].SetValue(bestiaryNPC.localAI[0] * 0.005f % 1f);
//            //        effect.Parameters["uTexture"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/LiquidTrail").Value);
//            //        effect.Parameters["uMap"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GoozmaColorMap").Value);
//            //        effect.CurrentTechnique.Passes[0].Apply();

//            //        VertexStrip cord = new VertexStrip();

//            //        Color ColorFunc(float progress) => Color.White;
//            //        float WidthFunc(float progress) => Utils.GetLerpValue(1.1f, 0.1f, progress, true) * 40f * bestiaryNPC.scale;

//            //        cord.PrepareStripWithProceduralPadding(positions.ToArray(), rotations.ToArray(), ColorFunc, WidthFunc, bestiaryNPC.Center, true);
//            //        cord.DrawTrail();
//            //        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
//            //    }

//            //Go back to the backbuffer RT if we switched away from it.
//            //Putting it at the end like that avoids having to switch back and forth if somehow theres more than 1 goozma
//            //Also you don't have to clear it transparent when switching back to it
//            if (switchedTargets)
//            {
//                Main.graphics.GraphicsDevice.SetRenderTarget(null);
//                Main.spriteBatch.End();
//            }
//        }
//    }
