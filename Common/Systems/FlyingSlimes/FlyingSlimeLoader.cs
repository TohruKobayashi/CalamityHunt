using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems.FlyingSlimes;

public class FlyingSlimeLoader : ILoadable
{
    public static List<FlyingSlimeData> flyingSlimeDataInstances;

    private static class Conditions
    {
        public static bool None() => true;

        public static bool IsNight() => !Main.dayTime;

        public static bool IsRaining() => Main.raining;

        public static bool IsWindy() => Main.IsItAHappyWindyDay;

        public static bool IsHalloween() => Main.halloween;

        public static bool IsXmas() => Main.xMas;

        public static bool IsGFBWorld() => Main.zenithWorld;

        public static bool IsHardmode() => Main.hardMode;

        public static bool IsRainingAndHardmode() => !Main.dayTime && Main.hardMode;

        public static bool IsNightAndHardmode() => !Main.dayTime && Main.hardMode;

        public static bool IsPlanteraDead() => NPC.downedPlantBoss;

        public static bool IsGolemDead() => NPC.downedGolemBoss;

        public static bool CalamityEvil() => ModLoader.HasMod(HUtils.CalamityMod) ? (bool)ModLoader.GetMod(HUtils.CalamityMod).Call("GetBossDowned", "HiveMind") || (bool)ModLoader.GetMod(HUtils.CalamityMod).Call("GetBossDowned", "Perforators") : false;

        public static bool Cryonic() => ModLoader.HasMod(HUtils.CalamityMod) ? (bool)ModLoader.GetMod(HUtils.CalamityMod).Call("GetBossDowned", "Cryogen") && ((NPC.downedMechBoss1 && NPC.downedMechBoss2) || (NPC.downedMechBoss2 && NPC.downedMechBoss3) || (NPC.downedMechBoss1 && NPC.downedMechBoss3)) : false;

        public static bool IsAquaticScourgeDead() => ModLoader.HasMod(HUtils.CalamityMod) ? (bool)ModLoader.GetMod(HUtils.CalamityMod).Call("GetBossDowned", "AquaticScourge") : false;
        
        public static bool IsProvidenceDead() => ModLoader.HasMod(HUtils.CalamityMod) ? (bool)ModLoader.GetMod(HUtils.CalamityMod).Call("GetBossDowned", "Providence") : false;

        public static bool IsPolterghastDead() => ModLoader.HasMod(HUtils.CalamityMod) ? (bool)ModLoader.GetMod(HUtils.CalamityMod).Call("GetBossDowned", "Polterghast") : false;

        public static bool IsAstrageldonDead() => ModLoader.HasMod(HUtils.CatalystMod) ? (!NPC.downedMoonlord || (bool)ModLoader.GetMod(HUtils.CatalystMod).Call("worlddefeats.astrageldon")) : false;
    }

    private static class DrawMethods
    {
        public static void DrawSmall(FlyingSlimeData data, SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, float progress, Color color)
        {
            Texture2D texture = AssetDirectory.Textures.FlyingSlime[data.Type].Value;
            Color secondColor = color;
            if (ContentSamples.NpcsByNetId.ContainsKey(data.NPCType)) {
                secondColor = ContentSamples.NpcsByNetId[data.NPCType].GetAlpha(color);
            }
            spriteBatch.Draw(texture, position, texture.Frame(), secondColor, rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale * 0.66f, 0, 0);
            spriteBatch.Draw(texture, position, texture.Frame(), color.MultiplyRGBA(data.DrawColor), rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale * 0.66f, 0, 0);
        }

        public static void DrawBig(FlyingSlimeData data, SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, float progress, Color color)
        {
            Texture2D texture = AssetDirectory.Textures.FlyingSlime[data.Type].Value;
            Color secondColor = color;
            if (ContentSamples.NpcsByNetId.ContainsKey(data.NPCType)) {
                secondColor = ContentSamples.NpcsByNetId[data.NPCType].GetAlpha(color);
            }
            spriteBatch.Draw(texture, position, texture.Frame(), secondColor, rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale * 1.33f, 0, 0);
            spriteBatch.Draw(texture, position, texture.Frame(), color.MultiplyRGBA(data.DrawColor), rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale * 1.33f, 0, 0);
        }

        public static void DrawLikePinky(FlyingSlimeData data, SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, float progress, Color color)
        {
            Texture2D texture = AssetDirectory.Textures.FlyingSlime[data.Type].Value;
            Color drawColor = color;

            int r = Math.Clamp(drawColor.R * 2, 0, 255);
            int g = Math.Clamp(drawColor.G * 2, 0, 255);
            int b = Math.Clamp(drawColor.B * 2, 0, 255);
            drawColor = new Color(r, g, b);

            Color secondColor = drawColor;
            if (ContentSamples.NpcsByNetId.ContainsKey(data.NPCType)) {
                secondColor = ContentSamples.NpcsByNetId[data.NPCType].GetAlpha(drawColor);
            }
            spriteBatch.Draw(texture, position, texture.Frame(), secondColor, rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale * 0.66f, 0, 0);
            spriteBatch.Draw(texture, position, texture.Frame(), drawColor.MultiplyRGBA(data.DrawColor), rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale * 0.66f, 0, 0);
        }

        public static void DrawFullBright(FlyingSlimeData data, SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, float progress, Color color)
        {
            Texture2D texture = AssetDirectory.Textures.FlyingSlime[data.Type].Value;
            Color secondColor = data.DrawColor * Utils.GetLerpValue(0, 0.1f, progress, true);
            if (ContentSamples.NpcsByNetId.ContainsKey(data.NPCType)) {
                secondColor = ContentSamples.NpcsByNetId[data.NPCType].GetAlpha(color);
            }
            spriteBatch.Draw(texture, position, texture.Frame(), secondColor, rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale, 0, 0);
        }

        public static void DrawRainbow(FlyingSlimeData data, SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, float progress, Color color)
        {
            Texture2D texture = AssetDirectory.Textures.FlyingSlime[data.Type].Value;
            spriteBatch.Draw(texture, position, texture.Frame(), color, rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale, 0, 0);
        }

        public static void DrawWithKey(FlyingSlimeData data, SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, float progress, Color color)
        {
            Texture2D texture = AssetDirectory.Textures.FlyingSlime[data.Type].Value;
            Texture2D keyTexture = TextureAssets.Item[ItemID.GoldenKey].Value;

            spriteBatch.Draw(keyTexture, position, keyTexture.Frame(), color, rotation, keyTexture.Size() * 0.5f, scale, 0, 0);
            Color secondColor = color.MultiplyRGBA(data.DrawColor);
            if (ContentSamples.NpcsByNetId.ContainsKey(data.NPCType)) {
                secondColor = ContentSamples.NpcsByNetId[data.NPCType].GetAlpha(color);
            }
            spriteBatch.Draw(texture, position, texture.Frame(), secondColor, rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale, 0, 0);
        }

        public static void DrawWithBalloon(FlyingSlimeData data, SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, float progress, Color color)
        {
            Texture2D texture = AssetDirectory.Textures.FlyingSlime[data.Type].Value;
            Texture2D balloons = AssetDirectory.Textures.Balloons.Value;
            Color secondColor = color;
            if (ContentSamples.NpcsByNetId.ContainsKey(data.NPCType)) {
                secondColor = ContentSamples.NpcsByNetId[data.NPCType].GetAlpha(color);
            }
            spriteBatch.Draw(texture, position, texture.Frame(), secondColor, rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale, 0, 0);
            spriteBatch.Draw(texture, position, texture.Frame(), color.MultiplyRGBA(data.DrawColor), rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale, 0, 0);

            Rectangle balloonFrame = balloons.Frame(7, 1, data.RandomValue % 7, 0);
            spriteBatch.Draw(balloons, position + (rotation - MathHelper.PiOver2).ToRotationVector2() * 15f * scale, balloonFrame, color, rotation - MathHelper.Pi + MathF.Sin(progress * 100f) * 0.05f, balloonFrame.Size() * new Vector2(0.5f, 1f), scale, 0, 0);
        }

        public static void DrawBigWithBalloon(FlyingSlimeData data, SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, float progress, Color color)
        {
            Texture2D texture = AssetDirectory.Textures.FlyingSlime[data.Type].Value;
            Texture2D balloons = AssetDirectory.Textures.Balloons.Value;
            Color secondColor = color;
            if (ContentSamples.NpcsByNetId.ContainsKey(data.NPCType)) {
                secondColor = ContentSamples.NpcsByNetId[data.NPCType].GetAlpha(color);
            }
            spriteBatch.Draw(texture, position, texture.Frame(), secondColor, rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale * 1.33f, 0, 0);
            spriteBatch.Draw(texture, position, texture.Frame(), color.MultiplyRGBA(data.DrawColor), rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale * 1.33f, 0, 0);

            Rectangle balloonFrame = balloons.Frame(7, 1, data.RandomValue % 7, 0);
            spriteBatch.Draw(balloons, position + (rotation - MathHelper.PiOver2).ToRotationVector2() * 15f * scale * 1.33f, balloonFrame, color, rotation - MathHelper.Pi + MathF.Sin(progress * 100f) * 0.05f, balloonFrame.Size() * new Vector2(0.5f, 1f), scale, 0, 0);
        }

        public static void DrawSmallWithBalloon(FlyingSlimeData data, SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, float progress, Color color)
        {
            Texture2D texture = AssetDirectory.Textures.FlyingSlime[data.Type].Value;
            Texture2D balloons = AssetDirectory.Textures.Balloons.Value;
            Color secondColor = color;
            if (ContentSamples.NpcsByNetId.ContainsKey(data.NPCType)) {
                secondColor = ContentSamples.NpcsByNetId[data.NPCType].GetAlpha(color);
            }
            spriteBatch.Draw(texture, position, texture.Frame(), secondColor, rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale * 0.66f, 0, 0);
            spriteBatch.Draw(texture, position, texture.Frame(), color.MultiplyRGBA(data.DrawColor), rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale * 0.66f, 0, 0);

            Rectangle balloonFrame = balloons.Frame(7, 1, data.RandomValue % 7, 0);
            spriteBatch.Draw(balloons, position + (rotation - MathHelper.PiOver2).ToRotationVector2() * 15f * scale * 0.66f, balloonFrame, color, rotation - MathHelper.Pi + MathF.Sin(progress * 100f) * 0.05f, balloonFrame.Size() * new Vector2(0.5f, 1f), scale, 0, 0);
        }

        public static void DrawPresent(FlyingSlimeData data, SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, float progress, Color color)
        {
            Texture2D texture = AssetDirectory.Textures.FlyingSlime[data.Type].Value;

            if (data.ExtraData is int value) {
                Rectangle presentFrame = texture.Frame(4, 1, value % 7, 0);
                spriteBatch.Draw(texture, position, presentFrame, color, rotation, presentFrame.Size() * new Vector2(0.5f, 0.33f), scale, 0, 0);
            }
        }

        public static void DrawShimmered(FlyingSlimeData data, SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, float progress, Color color)
        {
            Texture2D texture = AssetDirectory.Textures.FlyingSlime[data.Type].Value;

            DrawData drawData = new DrawData(texture, position, texture.Frame(), color, rotation, texture.Size() * 0.5f, scale, 0, 0);
            GameShaders.Misc["RainbowTownSlime"].Apply(drawData);
            drawData.Draw(spriteBatch);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }

    private static class DustMethods
    {
        public static void Default(FlyingSlimeData data, Vector2 position)
        {
            int alpha = 0;
            if (ContentSamples.NpcsByNetId.ContainsKey(data.NPCType)) {
                alpha = ContentSamples.NpcsByNetId[data.NPCType].alpha;
            }

            Color secondColor = data.DrawColor;
            if (ContentSamples.NpcsByNetId.ContainsKey(data.NPCType)) {
                secondColor = ContentSamples.NpcsByNetId[data.NPCType].color;
            }

            if (Main.rand.NextBool(4)) {
                Dust dust = Dust.NewDustDirect(position - new Vector2(10), 20, 20, DustID.TintableDust, 0, 0, alpha, secondColor, 1.5f);
                dust.noGravity = true;
            }
        }

        public static void Flame(FlyingSlimeData data, Vector2 position)
        {
            if (Main.rand.NextBool()) {
                Dust dust = Dust.NewDustDirect(position - new Vector2(10), 20, 20, DustID.Torch, 0, 0, 0, Color.White, 1.5f);
                dust.noGravity = true;
            }
        }

        public static void Ice(FlyingSlimeData data, Vector2 position)
        {
            if (Main.rand.NextBool(4)) {
                Dust dust = Dust.NewDustDirect(position - new Vector2(10), 20, 20, DustID.Snow, 0, 0, 0, Color.White, 2f);
                dust.noGravity = true;
            }
        }

        public static void Sand(FlyingSlimeData data, Vector2 position)
        {
            if (Main.rand.NextBool(4)) {
                Dust dust = Dust.NewDustDirect(position - new Vector2(10), 20, 20, DustID.Sand, 0, 0, 0, Color.White, 2f);
                dust.noGravity = true;
            }
        }

        public static void Spore(FlyingSlimeData data, Vector2 position)
        {
            if (Main.rand.NextBool(4)) {
                Dust dust = Dust.NewDustDirect(position - new Vector2(10), 20, 20, DustID.JungleSpore, 0, 0, 0, Color.White, 2f);
                dust.noGravity = true;
            }
        }

        public static void Shimmer(FlyingSlimeData data, Vector2 position)
        {
            if (Main.rand.NextBool()) {
                Dust dust = Dust.NewDustDirect(position - new Vector2(10), 20, 20, DustID.ShimmerSpark, 0, 0, 0, Main.hslToRgb(Main.GlobalTimeWrappedHourly % 1f, 0.5f, 0.66f, 128), 1.5f);
                dust.noGravity = true;
            }
        }
    }

    public static Color GetColor(int npcType)
    {
        Color newColor = ContentSamples.NpcsByNetId[npcType].GetColor(Color.White);
        if (newColor.R + newColor.G + newColor.B + newColor.A < 25) {
            newColor = Color.White;
        }
        return newColor;
    }

    public void Load(Mod mod)
    {
        flyingSlimeDataInstances = new List<FlyingSlimeData>() {
            new FlyingSlimeData("NormalSlime", 50f, Conditions.None, NPCID.BlueSlime, GetColor(NPCID.BlueSlime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("NormalSlime", 60f, Conditions.None, NPCID.GreenSlime, GetColor(NPCID.GreenSlime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("NormalSlime", 80f, Conditions.None, NPCID.RedSlime, GetColor(NPCID.RedSlime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("NormalSlime", 80f, Conditions.None, NPCID.YellowSlime, GetColor(NPCID.YellowSlime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("NormalSlime", 80f, Conditions.None, NPCID.BlackSlime, GetColor(NPCID.BlackSlime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("NormalSlime", 150f, Conditions.None, NPCID.JungleSlime, GetColor(NPCID.JungleSlime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("BigSlime", 80f, Conditions.None, NPCID.PurpleSlime, GetColor(NPCID.PurpleSlime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("BigSlime", 80f, Conditions.None, NPCID.MotherSlime, GetColor(NPCID.MotherSlime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("NormalSlime", 80f, Conditions.None, NPCID.BabySlime, GetColor(NPCID.BabySlime), dustMethod: DustMethods.Default, speed: 1f, specialDraw : DrawMethods.DrawSmall, load: true),
            new FlyingSlimeData("NormalSlime", 80f, Conditions.None, NPCID.Pinky, GetColor(NPCID.Pinky), dustMethod: DustMethods.Default, speed: 1f, specialDraw : DrawMethods.DrawLikePinky, load: true),
            new FlyingSlimeData("BalloonSlime", 1000f, Conditions.IsWindy, NPCID.None, GetColor(NPCID.BlueSlime), dustMethod: DustMethods.Default, speed: 0.5f, specialDraw: DrawMethods.DrawWithBalloon, load: true),
            new FlyingSlimeData("BalloonSlime", 1000f, Conditions.IsWindy, NPCID.None, GetColor(NPCID.GreenSlime), dustMethod: DustMethods.Default, speed: 0.5f, specialDraw: DrawMethods.DrawWithBalloon, load: true),
            new FlyingSlimeData("BalloonSlime", 1000f, Conditions.IsWindy, NPCID.None, GetColor(NPCID.PurpleSlime), dustMethod: DustMethods.Default, speed: 0.5f, specialDraw: DrawMethods.DrawBigWithBalloon, load: true),
            new FlyingSlimeData("BalloonSlime", 1000f, Conditions.IsWindy, NPCID.None, GetColor(NPCID.Pinky), dustMethod: DustMethods.Default, speed: 0.5f, specialDraw: DrawMethods.DrawSmallWithBalloon, extraData: () => Main.rand.Next(7), load: true),
            new FlyingSlimeData("UmbrellaSlime", 2000f, Conditions.IsRaining, NPCID.UmbrellaSlime, GetColor(NPCID.UmbrellaSlime), dustMethod: DustMethods.Default, speed: 0.33f, load: true),
            new FlyingSlimeData("LavaSlime", 700f, Conditions.None, NPCID.LavaSlime, GetColor(NPCID.LavaSlime), dustMethod: DustMethods.Flame, speed: 1.1f, specialDraw: DrawMethods.DrawFullBright, load: true),
            new FlyingSlimeData("ShimmerSlime", 1500f, Conditions.None, NPCID.ShimmerSlime, GetColor(NPCID.ShimmerSlime), dustMethod: DustMethods.Default, speed: 1.2f, specialDraw: DrawMethods.DrawShimmered, load: true),
            new FlyingSlimeData("IceSlime", 700f, Conditions.None, NPCID.IceSlime, GetColor(NPCID.IceSlime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("IceSlimeSpiked", 700f, Conditions.None, NPCID.SpikedIceSlime, GetColor(NPCID.SpikedIceSlime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("SandSlime", 700f, Conditions.None, NPCID.SandSlime, GetColor(NPCID.SandSlime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("JungleSlimeSpiked", 700f, Conditions.None, NPCID.SpikedJungleSlime, GetColor(NPCID.SpikedJungleSlime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("SpikedSlime", 700f, Conditions.None, NPCID.SlimeSpiked, GetColor(NPCID.SlimeSpiked), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("DungeonSlime", 1500f, Conditions.None, NPCID.DungeonSlime, GetColor(NPCID.DungeonSlime), dustMethod: DustMethods.Default, speed: 1f, specialDraw: DrawMethods.DrawWithKey, load: true),
            new FlyingSlimeData("HoppinJack", Conditions.IsHalloween() ? 150f : 2000f, Conditions.None, NPCID.HoppinJack, GetColor(NPCID.HoppinJack), dustMethod: DustMethods.Flame, speed: 1.5f, load: true),
            new FlyingSlimeData("SlimeFish", 1000f, Conditions.None, NPCID.None, Color.White, load: true),
            new FlyingSlimeData("GoldSlime", 5000f, Conditions.None, NPCID.GoldenSlime, GetColor(NPCID.GoldenSlime), dustMethod: DustMethods.Default, speed: 2f, load: true),
            new FlyingSlimeData("YuH", 10000f, Conditions.None, NPCID.None, Color.White, dustMethod: DustMethods.Default, speed: 0.66f, specialDraw: DrawMethods.DrawFullBright, load: true),

            //Night time
            new FlyingSlimeData("ZombieSlime", 800f, Conditions.IsNight, NPCID.SlimedZombie, GetColor(NPCID.SlimedZombie), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("ZombieSlime", 800f, Conditions.IsNight, NPCID.ArmedZombieSlimed, GetColor(NPCID.ArmedZombieSlimed), dustMethod: DustMethods.Default, speed: 1f, load: true),

            //Hardmode
            new FlyingSlimeData("IlluminantSlime", 800f, Conditions.IsHardmode, NPCID.IlluminantSlime, GetColor(NPCID.IlluminantSlime), dustMethod: DustMethods.Default, speed: 1.1f, specialDraw: DrawMethods.DrawFullBright, load: true),
            new FlyingSlimeData("BouncySlime", 800f, Conditions.IsHardmode, NPCID.QueenSlimeMinionPink, GetColor(NPCID.QueenSlimeMinionPink), dustMethod: DustMethods.Default, speed: 1.2f, load: true),
            new FlyingSlimeData("CrystalSlime", 800f, Conditions.IsHardmode, NPCID.QueenSlimeMinionBlue, GetColor(NPCID.QueenSlimeMinionBlue), dustMethod: DustMethods.Default, speed: 1.2f, load: true),
            new FlyingSlimeData("HeavenlySlime", 800f, Conditions.IsHardmode, NPCID.QueenSlimeMinionPurple, GetColor(NPCID.QueenSlimeMinionPurple), dustMethod: DustMethods.Default, speed: 1.2f, load: true),
            new FlyingSlimeData("CorruptSlime", 500f, Conditions.IsHardmode, NPCID.CorruptSlime, GetColor(NPCID.CorruptSlime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("Slimer", 1000f, Conditions.IsHardmode, NPCID.Slimer, GetColor(NPCID.Slimer), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("CorruptSlime", 1000f, Conditions.IsHardmode, NPCID.Slimer2, GetColor(NPCID.Slimer2), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("Crimslime", 500f, Conditions.IsHardmode, NPCID.BigCrimslime, GetColor(NPCID.BigCrimslime), dustMethod: DustMethods.Default, speed: 0.8f, load: true),
            new FlyingSlimeData("Crimslime", 500f, Conditions.IsHardmode, NPCID.LittleCrimslime, GetColor(NPCID.LittleCrimslime), dustMethod: DustMethods.Default, speed: 1.2f, load: true),
            new FlyingSlimeData("Crimslime", 500f, Conditions.IsHardmode, NPCID.Crimslime, GetColor(NPCID.Crimslime), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("ToxicSludge", 1000f, Conditions.IsHardmode, NPCID.ToxicSludge, GetColor(NPCID.ToxicSludge), dustMethod: DustMethods.Default, speed: 1.5f, load: true),

            //Night time and Hardmode
            new FlyingSlimeData("RainbowSlime", 5000f, Conditions.IsRainingAndHardmode, NPCID.RainbowSlime, GetColor(NPCID.RainbowSlime), dustMethod: DustMethods.Default, speed: 1.5f, specialDraw: DrawMethods.DrawRainbow, load: true),
            new FlyingSlimeData("Gastropod", 800f, Conditions.IsNightAndHardmode, NPCID.Gastropod, GetColor(NPCID.Gastropod), dustMethod: DustMethods.Default, speed: 0.8f, load: true),

            //Seasonal
            new FlyingSlimeData("SlimeBunny", 150f, Conditions.IsHalloween, NPCID.SlimeMasked, GetColor(NPCID.SlimeMasked), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("BunnySlime", 150f, Conditions.IsHalloween, NPCID.BunnySlimed, GetColor(NPCID.BunnySlimed), dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("PresentSlime", 150f, Conditions.IsXmas, NPCID.SlimeRibbonYellow, GetColor(NPCID.SlimeRibbonYellow), dustMethod: DustMethods.Default, speed: 1f, specialDraw: DrawMethods.DrawPresent, extraData: 0, load: true),
            new FlyingSlimeData("PresentSlime", 150f, Conditions.IsXmas, NPCID.SlimeRibbonRed, GetColor(NPCID.SlimeRibbonRed), dustMethod: DustMethods.Default, speed: 1f, specialDraw: DrawMethods.DrawPresent, extraData: 1, load: true),
            new FlyingSlimeData("PresentSlime", 150f, Conditions.IsXmas, NPCID.SlimeRibbonGreen, GetColor(NPCID.SlimeRibbonGreen), dustMethod: DustMethods.Default, speed: 1f, specialDraw: DrawMethods.DrawPresent, extraData: 2, load: true),
            new FlyingSlimeData("PresentSlime", 150f, Conditions.IsXmas, NPCID.SlimeRibbonWhite, GetColor(NPCID.SlimeRibbonWhite), dustMethod: DustMethods.Default, speed: 1f, specialDraw: DrawMethods.DrawPresent, extraData: 3, load: true),

            //GFB
            new FlyingSlimeData("YumeSlime", 15000f, Conditions.IsGFBWorld, NPCID.None, Color.White, dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("CoreSlime", 15000f, Conditions.IsGFBWorld, NPCID.None, Color.White, dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("DragonSlime", 15000f, Conditions.IsGFBWorld, NPCID.None, Color.White, dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("FatPixie", 5000f, Conditions.IsGFBWorld, NPCID.None, Color.White, dustMethod: DustMethods.Default, speed: 1f, specialDraw: DrawMethods.DrawFullBright, load: true),
            new FlyingSlimeData("MadnessSlime", 5000f, Conditions.IsGFBWorld, NPCID.None, Color.White, dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("MireSlime", 5000f, Conditions.IsGFBWorld, NPCID.None, Color.White, dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("InfernoSlime", 5000f, Conditions.IsGFBWorld, NPCID.None, Color.White, dustMethod: DustMethods.Default, speed: 1f, load: true),
            new FlyingSlimeData("OilSlime", 5000f, Conditions.IsGFBWorld, NPCID.None, Color.White, dustMethod: DustMethods.Default, speed: 1f, load: true),
        };
        
        flyingSlimeDataInstances.OrderBy(n => n.Type);
    }

    public void Unload()
    {
        flyingSlimeDataInstances.Clear();
    }

    // Load crossmod slimes after everything else has loaded
    public class LoadCalamitySlimes : ModSystem
    {
        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod(HUtils.CalamityMod, out Mod calamity)) {
                flyingSlimeDataInstances.AddRange(new List<FlyingSlimeData>()
            {
                new FlyingSlimeData("AeroSlime", 800f, Conditions.CalamityEvil, calamity.Find<ModNPC>("AeroSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("EbonianBlightSlime", 1500f, Conditions.None, calamity.Find<ModNPC>("EbonianBlightSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("CrimulanBlightSlime", 1500f, Conditions.None, calamity.Find<ModNPC>("CrimulanBlightSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("CorruptSlimeSpawn", 700f, Conditions.None, calamity.Find<ModNPC>("CorruptSlimeSpawn2").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("CorruptSlimeSpawnWinged", 700f, Conditions.None, calamity.Find<ModNPC>("CorruptSlimeSpawn").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("CrimsonSlimeSpawn", 700f, Conditions.None, calamity.Find<ModNPC>("CrimsonSlimeSpawn").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("CrimsonSlimeSpawnSpiked", 700f, Conditions.None, calamity.Find<ModNPC>("CrimsonSlimeSpawn2").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("AstralSlime", 1000f, Conditions.IsHardmode, calamity.Find<ModNPC>("AstralSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("CryoSlime", 1000f, Conditions.Cryonic, calamity.Find<ModNPC>("CryoSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("IrradiatedSlime", 800f, Conditions.IsAquaticScourgeDead, calamity.Find<ModNPC>("IrradiatedSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("CharredSlime", 1000f, Conditions.IsHardmode, calamity.Find<ModNPC>("InfernalCongealment").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("PerennialSlime", 1000f, Conditions.IsPlanteraDead, calamity.Find<ModNPC>("PerennialSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("AureusSpawnSlime", 3000f, Conditions.IsHardmode, calamity.Find<ModNPC>("AureusSpawn").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("PestilentSlime", 800f, Conditions.IsGolemDead, calamity.Find<ModNPC>("PestilentSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("BloomSlime", 1000f, Conditions.IsProvidenceDead, calamity.Find<ModNPC>("BloomSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("GammaSlime", 800f, Conditions.IsPolterghastDead, calamity.Find<ModNPC>("GammaSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod)),
                new FlyingSlimeData("CragmawMire", 5000f, Conditions.IsPolterghastDead, calamity.Find<ModNPC>("CragmawMire").Type, Color.White, load: ModLoader.HasMod(HUtils.CalamityMod))
            });
            }

            if (ModLoader.TryGetMod(HUtils.CatalystMod, out Mod catalyst)) {
                flyingSlimeDataInstances.AddRange(new List<FlyingSlimeData>() {
                new FlyingSlimeData("WulfrumSlime", 800f, Conditions.None, catalyst.Find<ModNPC>("WulfrumSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CatalystMod)),
                new FlyingSlimeData("AscendedAstralSlime", 1500f, Conditions.None, catalyst.Find<ModNPC>("AscendedAstralSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CatalystMod)),
                new FlyingSlimeData("NovaSlime", 700f, Conditions.IsAstrageldonDead, catalyst.Find<ModNPC>("NovaSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CatalystMod)),
                new FlyingSlimeData("NovaSlimer", 700f, Conditions.IsAstrageldonDead, catalyst.Find<ModNPC>("NovaSlimer").Type, Color.White, load: ModLoader.HasMod(HUtils.CatalystMod)),
                new FlyingSlimeData("MetanovaSlime", 1000f, Conditions.IsAstrageldonDead, catalyst.Find<ModNPC>("MetanovaSlime").Type, Color.White, load: ModLoader.HasMod(HUtils.CatalystMod))
            });
            }
        }
    }
}
