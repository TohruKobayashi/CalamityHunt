using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Mounts
{
    public class PaladinPalanquinMount : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.jumpHeight = 16;
            MountData.jumpSpeed = 12f;

            MountData.acceleration = 0.12f;
            MountData.blockExtraJumps = true;
            MountData.constantJump = true;
            MountData.fallDamage = 0;
            MountData.runSpeed = 16;
            MountData.dashSpeed = 24;

            MountData.fatigueMax = 0;
            MountData.buff = ModContent.BuffType<Buffs.PaladinPalanquinBuff>();

            MountData.yOffset = -26;
            MountData.bodyFrame = 0;

            MountData.totalFrames = 1;

            MountData.heightBoost = 16;
            MountData.playerYOffsets = Enumerable.Repeat(MountData.heightBoost, MountData.totalFrames).ToArray();

            if (!Main.dedServ)
            {
                MountData.textureWidth = MountData.backTexture.Width() + 20;
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }

        public override void SetMount(Player player, ref bool skipDust)
        {
            player.mount._mountSpecificData = new PaladinPalanquinData() { rotation = 0, tilt = 0 };
            SoundEngine.PlaySound(SoundID.Item81.WithVolumeScale(1.5f).WithPitchOffset(0.7f), player.Center);
            skipDust = true;

            for (int i = 0; i < 30; i++)
            {
                Dust d = Dust.NewDustDirect(player.MountedCenter - new Vector2(25, 20), 50, 80, DustID.TintableDust, 0, -Main.rand.NextFloat(5f), 100, Color.Black, Main.rand.NextFloat() + 1);
                d.noGravity = true;
            }
        }

        public override void Dismount(Player player, ref bool skipDust)
        {
            SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Pop.WithVolumeScale(0.5f).WithPitchOffset(1f), player.Center);
            skipDust = true;
            for (int i = 0; i < 40; i++)
            {
                Dust d = Dust.NewDustDirect(player.MountedCenter - new Vector2(25, 20), 50, 80, DustID.TintableDust, 0, -Main.rand.NextFloat(5f), 100, Color.Black, Main.rand.NextFloat() + 1);
                d.noGravity = true;
            }
        }

        public override void UpdateEffects(Player player)
        {
            int heightBoost = 36;

            if (player.mount._mountSpecificData is PaladinPalanquinData data)
            {
                data.tilt = player.velocity.X * 0.02f;

                data.rotation += player.velocity.X * 0.015f;

                player.maxFallSpeed = player.controlDownHold ? 40 : 20;

                if (player.controlJump)
                    player.velocity.Y -= 1f;

                if (player.velocity.Y < -10f)
                    player.velocity.Y = -10f;

                data.frameCounter += (int)(Utils.GetLerpValue(-1, 15, Math.Abs(player.velocity.X), true) * 3);
                if (data.frameCounter > 5)
                {
                    data.frameCounter = 0;
                    data.frameOffset = (data.frameOffset + 1) % 4;
                }
                data.frame = (int)(Utils.GetLerpValue(1, 5, Math.Abs(player.velocity.X), true) * 2);

                if (Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y) > 0.5f)
                {
                    if (data.ballFrameCounter++ > 6)
                    {
                        data.ballFrameCounter = 0;
                        data.ballFrame = Math.Clamp(data.ballFrame + 1, 0, 2);
                    }
                    if (data.ballFrame < 2)
                        data.rotation = 0;
                }
                else
                {
                    data.ballFrameCounter++;
                    data.ballFrame = 2 - (int)(Utils.GetLerpValue(30, 50, data.ballFrameCounter, true) * 2);
                    heightBoost = 36 - (int)(Utils.GetLerpValue(30, 50, data.ballFrameCounter, true) * 2) * 4;
                    if (data.ballFrame < 2)
                        data.rotation = 0;
                }
                if (!player.controlJump)
                {
                    data.wingFrameCounter = 0;
                    data.wingFrame = 1;
                }
                else if (data.wingFrameCounter++ > 2)
                {
                    data.wingFrameCounter = 0;
                    data.wingFrame = (data.wingFrame + 1) % 4;
                    if (data.wingFrame == 3)
                        SoundEngine.PlaySound(SoundID.Item32, player.Center);
                }

                if (Collision.SolidCollision(player.MountedCenter + new Vector2(0, heightBoost + 20), 2, 20) && Math.Abs(player.velocity.X) > 1f)
                {
                    Dust d = Dust.NewDustPerfect(player.MountedCenter + new Vector2(0, heightBoost + 20) + Main.rand.NextVector2Circular(5, 3), DustID.TintableDust, new Vector2(-player.velocity.X * 0.1f, -1 - Math.Abs(player.velocity.X) * 0.2f).RotatedByRandom(1f), 100, Color.Black, 0.7f);
                    d.noGravity = Main.rand.NextBool(3);
                    d.shader = GameShaders.Armor.GetSecondaryShader(player.cMount, player);
                }
            }
            else
                player.mount._mountSpecificData = new PaladinPalanquinData();

            MountData.heightBoost = heightBoost;
            MountData.playerYOffsets = Enumerable.Repeat(MountData.heightBoost, MountData.totalFrames).ToArray();

            //if (Collision.SolidCollision(player.MountedCenter + new Vector2(player.velocity.X * 3f - 10, -1), 2, 20) && Math.Abs(player.velocity.X) > 1f)
            //    player.velocity.X *= -0.5f;

            if (Collision.SolidCollision(player.MountedCenter + new Vector2(0, player.velocity.Y * 2f + 20), 2, 30) && Math.Abs(player.velocity.Y) > 0.5f && !player.controlDown)
            {
                player.velocity.Y *= -0.7f - Utils.GetLerpValue(0, 10, Math.Abs(player.velocity.X), true) * 0.4f;

                SoundStyle bounceSound = SoundID.Item154 with { Pitch = -Utils.GetLerpValue(0, 20, Math.Abs(player.velocity.Y), true) + 0.7f, PitchVariance = 0.3f, Volume = Utils.GetLerpValue(0, 20, Math.Abs(player.velocity.Y), true) };
                SoundEngine.PlaySound(bounceSound, player.Center);
            }

        }

        protected class PaladinPalanquinData
        {
            public float rotation;

            public float tilt;

            public int ballFrame;
            public int ballFrameCounter;            
            public int wingFrame;
            public int wingFrameCounter;

            public int frame;
            public int frameOffset;
            public int frameCounter;
        }

        public static PaladinPalanquinTextureContent ballTextureContent;

        public override void Load()
        {
            Main.ContentThatNeedsRenderTargets.Add(ballTextureContent = new PaladinPalanquinTextureContent());
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            if (drawPlayer.mount._mountSpecificData is PaladinPalanquinData data)
            {
                switch (drawType)
                {
                    case 2:

                        Vector2 minusVelocity = new Vector2(-drawPlayer.velocity.X * 0.1f, Math.Max(-drawPlayer.velocity.Y * 1.5f, 0));

                        float yS = Math.Abs(Math.Min(0, drawPlayer.velocity.Y * 0.02f));
                        float xS = Math.Abs(drawPlayer.velocity.X * 0.01f) * (1f - yS * 0.6f);
                        Vector2 squish = new Vector2(1f - yS + xS, 1f + yS - xS) * drawScale;

                        Rectangle ballFrame = AssetDirectory.Textures.Extras.PaladinPalanquinBall.Value.Frame(3, 1, data.ballFrame, 0);
                        ballTextureContent.frame = ballFrame;
                        ballTextureContent.rotation = data.rotation;
                        ballTextureContent.Request();

                        if (ballTextureContent.IsReady)
                        {
                            Texture2D ballTexture = ballTextureContent.GetTarget();

                            DrawData ballData = new DrawData(ballTexture, drawPlayer.MountedCenter + minusVelocity + new Vector2(0, MountData.heightBoost + xS * 8f).RotatedBy(data.tilt) - Main.screenPosition, ballTexture.Frame(), drawColor, data.tilt, ballTexture.Size() * 0.5f, squish, 0, 0);                         

                            ballData.shader = drawPlayer.cMount;
                            playerDrawData.Add(ballData);
                        }

                        Rectangle frontFrame = MountData.frontTexture.Frame(1, 6, 0, data.frame == 2 ? data.frame + data.frameOffset : data.frame);
                        DrawData palanquinData = new DrawData(MountData.frontTexture.Value, drawPlayer.MountedCenter - new Vector2(0, 9).RotatedBy(data.tilt) - Main.screenPosition, frontFrame, drawColor, data.tilt, frontFrame.Size() * 0.5f, drawScale, spriteEffects, 0);
                        palanquinData.shader = drawPlayer.cMount;
                        playerDrawData.Add(palanquinData);

                        if (data.ballFrame == 2)
                        {
                            Texture2D wingTexture = AssetDirectory.Textures.Extras.PaladinPalanquinWings.Value;
                            Rectangle wingFrame = wingTexture.Frame(1, 4, 0, data.wingFrame);

                            DrawData wingData = new DrawData(wingTexture, drawPlayer.MountedCenter + minusVelocity + new Vector2(-24 * drawPlayer.direction, MountData.heightBoost - 8).RotatedBy(data.tilt) * squish - Main.screenPosition, wingFrame, drawColor, data.tilt, wingFrame.Size() * 0.5f, drawScale, spriteEffects, 0);
                            wingData.shader = drawPlayer.cMount;
                            playerDrawData.Add(wingData);
                        }

                        break;

                    case 3:

                        break;
                }
            }
            else
                drawPlayer.mount._mountSpecificData = new PaladinPalanquinData();

            return false;   
        }
    }
}
