using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Systems.FlyingSlimes;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Common.Utilities.Interfaces;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs;

[AutoloadBossHead]
public class PluripotentSpawn : ModNPC, ISubjectOfNPC<Goozma>
{
    public override void SetStaticDefaults()
    {
        NPCID.Sets.MustAlwaysDraw[Type] = true;
        NPCID.Sets.ShouldBeCountedAsBoss[Type] = false;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true });
    }

    public static LocalizedText slimeMonsoonText;

    public override void SetDefaults()
    {
        NPC.width = 100;
        NPC.height = 100;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.immortal = true;
        NPC.damage = 0;
        NPC.dontTakeDamage = true;
        NPC.dontCountMe = true;
        NPC.ShowNameOnHover = false;
        NPC.lifeMax = 2028;
        NPC.BossBar = Main.BigBossProgressBar.NeverValid;
        Music = AssetDirectory.Music.GoozmaSoul;

        slimeMonsoonText = Language.GetOrRegister(Mod.GetLocalizationKey("Chat.SlimeMonsoon"));
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

    public ref float Time => ref NPC.ai[0];

    // having 5+ statues brings back the full animation
    public bool Skip => NPC.ai[1] == 1 || NPC.ai[3] == 1 || (BossDownedSystem.Instance.PluripotentDowned && GoozmaSystem.slimeStatuePoints.Length <= 4);

    public float size;

    public override void AI()
    {
        bool spawnBoss = false;
        ScreenObstruction.screenObstruction = Utils.GetLerpValue(0, 100, NPC.localAI[0], true) * 0.1f;

        if (Skip) {
            size = (int)(Utils.GetLerpValue(20, 105, Time, true) * 6f);
            NPC.scale = 1f + MathF.Round(Utils.GetLerpValue(20, 105, Time, true), 2) + MathF.Pow(Utils.GetLerpValue(230, 250, Time, true), 3f) * 0.2f;
            NPC.velocity.Y = MathHelper.SmoothStep(0, -3f, Utils.GetLerpValue(130, 250, Time, true) * Utils.GetLerpValue(250, 230, Time, true));

            if (Time > 200 && Time % 3 == 0) {
                shake += Main.rand.NextVector2Circular(6, 6) * Utils.GetLerpValue(130, 180, Time, true);
            }

            if (Time == 105) {
                if (Main.dedServ) {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(slimeMonsoonText.Value), new Color(50, 255, 130));
                }
                else if (Main.netMode == NetmodeID.SinglePlayer){
                    Main.NewText(NetworkText.FromKey(slimeMonsoonText.Value), new Color(50, 255, 130));
                }
                Main.StopSlimeRain(true);
            }

            if (Time > 250 && Main.netMode != NetmodeID.MultiplayerClient) {
                spawnBoss = true;
                NPC.netUpdate = true;
            }
        }
        else {
            size = (int)(Utils.GetLerpValue(30, 1000, Time, true) * 6f);
            NPC.scale = 1f + MathF.Round(Utils.GetLerpValue(30, 1000, Time, true), 2);
            NPC.velocity.Y = -0.3f * Utils.GetLerpValue(30, 1000, Time, true);

            if (Time % 3 == 0) {
                shake += Main.rand.NextVector2Circular(6, 6) * size / 6f;
            }

            if (Time > 1000 && Main.netMode != NetmodeID.MultiplayerClient) {
                Time = 104;
                NPC.ai[1] = 1;
                NPC.netUpdate = true;

                slimes.Clear();
            }

            slimes ??= new HashSet<FlyingSlime>();

            foreach (FlyingSlime slime in slimes.ToHashSet()) {
                slime.Update(MathF.Sin(Time * 0.12f) * 0.3f + 4f);

                if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(8)) {
                    CalamityHunt.particles.Add(Particle.Create<SmokeSplatterParticle>(particle => {
                        particle.position = slime.currentPosition + Main.rand.NextVector2Circular(20, 20);
                        particle.velocity = slime.currentPosition.DirectionTo(slime.targetPosition);
                        particle.rotation = Main.rand.NextFloat(-0.1f, 0.1f);
                        particle.scale = 1.2f;
                        particle.color = Color.Black;
                        particle.fadeColor = Color.DimGray * 0.2f;
                        particle.maxTime = Main.rand.Next(30, 45);
                    }));
                }

                if (slime.ShouldRemove) {

                    if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(1 + (int)(Time * 0.005f))) {
                        CalamityHunt.particles.Add(Particle.Create<ChromaticGooBurst>(particle => {
                            particle.position = slime.currentPosition;
                            particle.velocity = (slime.rotation + MathHelper.PiOver2).ToRotationVector2();
                            particle.scale = Main.rand.NextFloat(0.3f, 1f) * slime.scale;
                            particle.color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0] * 0.33f);
                        }));
                    }

                    if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(10 + (int)(Time * 0.001f))) {
                        CalamityHunt.particles.Add(Particle.Create<ChromaticGelChunk>(particle => {
                            particle.position = slime.currentPosition;
                            particle.velocity = (slime.rotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.2f);
                            particle.scale = Main.rand.NextFloat(0.8f, 1.5f);
                            particle.color = Color.White;
                            particle.colorData = new ColorOffsetData(true, NPC.localAI[0] * 0.33f);
                        }));
                    }

                    SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.SlimeAbsorb.WithPitchOffset(0.5f), slime.currentPosition);

                    slimes.Remove(slime);
                }
            }  

            if (Time == 200) {
                SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Intro.WithVolumeScale(2f).WithPitchOffset(-0.95f), NPC.Center);
                Main.StopSlimeRain(true);
            }

            for (int i = 0; i < 60; i++) {
                if (Time < 870 && Main.rand.NextBool((int)Math.Max(1, 150 - Time * 0.16f))) {
                    FlyingSlime slime = FlyingSlime.CreateRandom();
                    Vector2 slimeOffset = Main.rand.NextVector2CircularEdge(1400, 1400) + Main.rand.NextVector2Circular(500, 500);
                    slime.startPosition = NPC.Center + slimeOffset;
                    slime.targetPosition = NPC.Center + slimeOffset.SafeNormalize(Vector2.Zero) * 10f * NPC.scale;
                    slimes?.Add(slime);
                }
            }

            if (Time % 4 == 0) {
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2CircularEdge(1, 1), Time * 0.01f, 6f, 10, 8000, "Goozma"));
            }
        }

        int boss = -1;
        if (spawnBoss) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                boss = NPC.NewNPC(NPC.GetBossSpawnSource(0), (int)NPC.Center.X, (int)NPC.Bottom.Y, ModContent.NPCType<Goozma>(), 0, ai1: -1);
                NPC.active = false;
            }

            if (Main.netMode != NetmodeID.Server) {
                for (int i = 0; i < 50; i++) {
                    CalamityHunt.particles.Add(Particle.Create<ChromaticGelChunk>(particle => {
                        particle.position = NPC.Center + Main.rand.NextVector2Circular(50, 50);
                        particle.velocity = particle.position.DirectionFrom(NPC.Center).RotatedByRandom(0.4f) * Main.rand.Next(2, 10) - Vector2.UnitY * 2f;
                        particle.scale = Main.rand.NextFloat(0.8f, 1.5f);
                        particle.color = Color.White;
                        particle.colorData = new ColorOffsetData(true, NPC.localAI[0] * 0.33f);
                    }));
                }
            }

            if (!BossDownedSystem.Instance.PluripotentDowned) {
                BossDownedSystem.Instance.PluripotentDowned = true;

                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
        }

        if (Main.netMode == NetmodeID.Server && boss > -1) {
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, boss);
        }

        Time++;
        shake *= 0.7f;

        if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(4)) {
            CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                particle.position = NPC.Center + Main.rand.NextVector2Circular(30, 30) * NPC.scale;
                particle.velocity = particle.position.DirectionFrom(NPC.Center) * Main.rand.NextFloat(0.5f, 3f);
                particle.rotation = particle.velocity.ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f);
                particle.scale = Main.rand.NextFloat(0.5f, 1.5f);
                particle.color = (new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0] * 0.33f) * 1.1f) with { A = 128 };
                particle.maxTime = Main.rand.Next(3, 8);
                particle.anchor = () => NPC.velocity * 0.9f;
            }));
        }

        NPC.localAI[0]++;
    }

    public override void Load()
    {
        On_Main.UpdateAudio += FadeMusicOut;
    }

    public override void Unload()
    {
        On_Main.UpdateAudio -= FadeMusicOut;
    }

    private void FadeMusicOut(On_Main.orig_UpdateAudio orig, Main self)
    {
        orig(self);

        if (Main.npc.Any(n => n.active && n.type == ModContent.NPCType<PluripotentSpawn>())) {
            NPC goozma = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<PluripotentSpawn>());
            for (int i = 0; i < Main.musicFade.Length; i++) {
                float volume = Main.musicFade[i] * Main.musicVolume * Utils.GetLerpValue(300, 150, goozma.localAI[0], true);
                float tempFade = Main.musicFade[i];
                Main.audioSystem.UpdateCommonTrackTowardStopping(i, volume, ref tempFade, Main.musicFade[i] > 0.1f && goozma.ai[0] < 600);
                Main.musicFade[i] = tempFade;
            }
        }
    }

    private HashSet<FlyingSlime> slimes;

    private Vector2 shake;

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (NPC.IsABestiaryIconDummy) {
            return false;
        }

        float intensity = (Skip ? Utils.GetLerpValue(220, 110, Time, true) : Utils.GetLerpValue(40, 800, Time, true)) * Utils.GetLerpValue(0, 20, NPC.localAI[0], true);
        if (Filters.Scene["HuntOfTheOldGods:PluripotentSpawn"].Active) {
            Filters.Scene["HuntOfTheOldGods:PluripotentSpawn"].GetShader()
                .UseTargetPosition(NPC.Center)
                .UseProgress(Main.GlobalTimeWrappedHourly * 0.05f % 1f)
                .UseOpacity(intensity * 0.1f)
                .UseIntensity(1f);
            Effect shader = Filters.Scene["HuntOfTheOldGods:PluripotentSpawn"].GetShader().Shader;
            shader.Parameters["distortSize"].SetValue(Vector2.One * 5f);
            shader.Parameters["uNoiseTexture0"].SetValue(AssetDirectory.Textures.Noise[0].Value);
            shader.Parameters["uNoiseTexture1"].SetValue(AssetDirectory.Textures.Noise[11].Value);
        }
        else {
            Filters.Scene.Activate("HuntOfTheOldGods:PluripotentSpawn", NPC.Center);
        }

        Texture2D texture = TextureAssets.Npc[Type].Value;
        Texture2D glow = AssetDirectory.Textures.Glow[1].Value;

        Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0] * 0.33f) with { A = 0 };

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        if (slimes != null) {
            foreach (FlyingSlime slime in slimes.ToHashSet()) {
                slime.Draw(spriteBatch, screenPos);
            }
        }

        Goozma.GetGradientMapValues(out float[] brightnesses, out Vector3[] colors);
        Effect effect = AssetDirectory.Effects.HolographicGel.Value;
        effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly % 1f);
        effect.Parameters["colors"].SetValue(colors);
        effect.Parameters["brightnesses"].SetValue(brightnesses);
        effect.Parameters["baseToScreenPercent"].SetValue(Skip ? Utils.GetLerpValue(150, 100, Time, true) : 1f);
        effect.Parameters["baseToMapPercent"].SetValue(0f);

        Vector2 drawPos = NPC.Center + shake;
        Rectangle frame = texture.Frame(7, 1, (int)size, 0);
        float sizeScale = size / 6f;

        Main.EntitySpriteDraw(glow, drawPos - screenPos, glow.Frame(), glowColor * 0.1f, 0, glow.Size() * 0.5f, NPC.scale * 0.5f, 0, 0);

        Main.pixelShader.CurrentTechnique.Passes["ColorOnly"].Apply();

        for (int i = 0; i < 4; i++) {
            Vector2 off = new Vector2(2, 0).RotatedBy(MathHelper.TwoPi / 4f * i);
            Main.EntitySpriteDraw(texture, drawPos + off - screenPos, frame, glowColor * 0.7f, 0, frame.Size() * 0.5f, NPC.scale - sizeScale, 0, 0);
        }

        effect.CurrentTechnique.Passes[0].Apply();

        Main.EntitySpriteDraw(texture, drawPos - screenPos, frame, Color.White, 0, frame.Size() * 0.5f, NPC.scale - sizeScale, 0, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        if (Main.xMas) {
            Texture2D hat = AssetDirectory.Textures.SantaHat.Value;
            Main.EntitySpriteDraw(hat, drawPos - screenPos + new Vector2(0, 15 - 25 * NPC.scale), hat.Frame(), Color.White, 0.7f, hat.Size() * new Vector2(0.7f, 0.5f), NPC.scale * 0.5f - 0.2f, 0, 0);
        }

        return false;
    }
}
