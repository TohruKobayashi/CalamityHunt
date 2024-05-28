using CalamityHunt.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;

namespace CalamityHunt
{
    public static class AssetDirectory
    {
        public static class Textures
        {
            public static readonly Asset<Texture2D> Sparkle = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/SharpSpark");
            public static readonly Asset<Texture2D> GlowRing = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/GlowRing");
            public static readonly Asset<Texture2D> GlowRay = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/GlowRay");
            public static readonly Asset<Texture2D> Glow = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/GlowSoft");
            public static readonly Asset<Texture2D> GlowBig = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/GlowSoftBig");
            public static readonly Asset<Texture2D> ShockRing = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ShockRing");
            public static readonly Asset<Texture2D> Empty = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Empty");
            public static class Goozma
            {

                public static readonly Asset<Texture2D> Dress = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Bosses/Goozma/GoozmaDress");
                public static readonly Asset<Texture2D> Tentacle = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Bosses/Goozma/GoozmaTentacle");

                public static readonly Asset<Texture2D> BC = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GoozmaBC");
                public static readonly Asset<Texture2D> GodEye = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/SpecialEye");
                public static readonly Asset<Texture2D> Eyeball = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GoozmaSclera");
                public static readonly Asset<Texture2D> Wormhole = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Wormhole");
                public static readonly Asset<Texture2D> DarkSludge = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/DarkSludgeTexture");
                public static readonly Asset<Texture2D> DarkSludgeGlow = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/DarkSludgeTextureGlow");
                public static readonly Asset<Texture2D> Lightning = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Lightning");
                public static readonly Asset<Texture2D> LightningGlow = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/LightningGlow");
                public static readonly Asset<Texture2D> LiquidTrail = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/LiquidTrail");
                public static readonly Asset<Texture2D> ColorMap = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GoozmaColorMap");

                public static readonly Asset<Texture2D> Crown = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/GoozmaCrown");
                public static readonly Asset<Texture2D> CrownMask = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/GoozmaCrown_Mask");
                public static readonly Asset<Texture2D> Ornament = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/GoozmaOrnament");
                public static readonly Asset<Texture2D> Ninja = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/DeadNinja");
                public static readonly Asset<Texture2D> CorruptEye = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/CorruptEye");
                public static readonly Asset<Texture2D> CrystalMine = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/CrystalMine");

                public static readonly Asset<Texture2D> DivineWings = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/DivineGargooptuarWingsNevermindSorryDominic");
                public static readonly Asset<Texture2D> PixieBeachBall = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/PixieBeachBall");
                public static readonly Asset<Texture2D> PixieHitMeSign = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/PixieHitMeSign");
                public static readonly Asset<Texture2D> PixieHitMeHand = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/PixieHitMeHand");

                public static readonly Asset<Texture2D>[] SwirlingRocks = AssetUtilities.RequestArrayImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/SwirlingRocks_", 2);
                public static readonly Asset<Texture2D> ConstellationArea = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/ConstellationArea");
                public static readonly Asset<Texture2D> SpaceTrail = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/SpaceTrail");
                public static readonly Asset<Texture2D> SpaceTrailGlow = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/SpaceTrailGlow");
            }
            public static class Buffs
            {
                public static readonly Asset<Texture2D>[] SlimeCane = AssetUtilities.RequestArrayImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Buffs/SlimeCane_", 2);
            }
            public static class Space
            {
                public static readonly Asset<Texture2D> Noise0 = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Noise/Noise0");
                public static readonly Asset<Texture2D> Noise1 = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Noise/Noise1");
                public static readonly Asset<Texture2D> Space0 = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Space0");
                public static readonly Asset<Texture2D> Space1 = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Space1");
            }
            public static class Extras
            {
                public static readonly Asset<Texture2D> RainbowMap = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/RainbowMap");
                public static readonly Asset<Texture2D> RainbowGelMap = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/RainbowGelMap");
                public static readonly Asset<Texture2D> PaladinPalanquinBall = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Mounts/PaladinPalanquinMount_Ball");
                public static readonly Asset<Texture2D> PaladinPalanquinWings = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Mounts/PaladinPalanquinMount_Wings");
                public static readonly Asset<Texture2D> InkyHats = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/InkyHats");
                public static readonly Asset<Texture2D> InkyRings = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/InkyRings");
                public static readonly Asset<Texture2D> GoozmoemEye = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/GoozmoemEye");
                public static readonly Asset<Texture2D> GoozmoemCrown = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/GoozmoemCrown");
                public static readonly Asset<Texture2D> CometKunaiFlame = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/CometKunaiFlame");
                public static readonly Asset<Texture2D>[] FusionRay = AssetUtilities.RequestArrayImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/FusionRay_", 4);
                public static readonly Asset<Texture2D> QuestionMark = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/icon_small");
            }
            public static class Bars
            {
                public static readonly Asset<Texture2D> Bar = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/Style0_0");
                public static readonly Asset<Texture2D> BarCharge = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/Style0_1");
                public static readonly Asset<Texture2D> Stress = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/StressBar");
                public static readonly Asset<Texture2D> StressCharge = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/StressBarFill");
                public static readonly Asset<Texture2D> StressTopped = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/StressBarTopped");
            }
            public static class SwordSwing
            {
                public static readonly Asset<Texture2D> Basic = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/SwordSwing");
                public static readonly Asset<Texture2D> Bloody = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/BloodSwing");
            }
            public static class SlimeMonsoon
            {
                public static readonly Asset<Texture2D> Background = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/SlimeMonsoonBG");

                public static readonly Asset<Texture2D> Lightning = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/Lightning");
                public static readonly Asset<Texture2D> LightningGlow = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/LightningGlow");

                public static readonly Asset<Texture2D> SkyTexture = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/SkyNoise");
                public static readonly Asset<Texture2D> SkyDistortion = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/DistortNoise");
                public static readonly Asset<Texture2D> SkyColorMap = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/ColorMap");
            }
        }

        public static class Sounds
        {
            public static class Goozma
            {
                public static readonly SoundStyle SlimeAbsorb = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSlimeAbsorb", 8) { MaxInstances = 0, Volume = 0.1f };
                public static readonly SoundStyle Intro = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaIntro") { MaxInstances = 0, PlayOnlyIfFocused = true };
                public static readonly SoundStyle Awaken = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaAwaken") { MaxInstances = 0 };
                public static readonly SoundStyle Hurt = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaHurt", 1, 3) { MaxInstances = 0 };
                public static readonly SoundStyle SpawnSlime = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSpawnSlime") { MaxInstances = 0 };
                public static readonly SoundStyle EyeAppear = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaEyeAppear") { MaxInstances = 0 };
                public static readonly SoundStyle Reform = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaReform") { MaxInstances = 0 };
                public static readonly SoundStyle Reawaken = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaReawaken") { MaxInstances = 0 };
                public static readonly SoundStyle Dash = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaDash", 1, 2) { MaxInstances = 0, PitchVariance = 0.15f };

                public static readonly SoundStyle SlimeShoot = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSlimeShoot", 1, 3) { MaxInstances = 0, PitchVariance = 0.1f, Volume = 0.66f };
                public static readonly SoundStyle Dart = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaDartShoot", 1, 2) { MaxInstances = 0, PitchVariance = 0.2f };
                public static readonly SoundStyle Shot = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaShot", 1, 2) { MaxInstances = 0 };
                public static readonly SoundStyle RainbowShoot = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaRainbowShoot", 1, 2) { MaxInstances = 0, PitchVariance = 0.15f };
                public static readonly SoundStyle BombCharge = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaBombCharge") { MaxInstances = 0 };
                public static readonly SoundStyle BloatedBlastShoot = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaBloatedBlastShoot") { MaxInstances = 0 };
                public static readonly SoundStyle GoozmiteShoot = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmiteShoot", 1, 2) { MaxInstances = 0 };
                public static readonly SoundStyle BigThunder = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaBigThunder") { MaxInstances = 0, PitchVariance = 0.15f };
                public static readonly SoundStyle SmallThunder = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSmallThunder", 1, 3) { MaxInstances = 0, PitchVariance = 0.15f };

                public static readonly SoundStyle Sizzle = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSizzle") { MaxInstances = 0 };
                public static readonly SoundStyle WarbleLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaTravelLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle ShootLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaShootLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle SimmerLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSimmerLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle FusionRayLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaFusionRayLoop") { MaxInstances = 0, IsLooped = true };

                public static readonly SoundStyle DeathBuildup = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaDeathBuildup") { MaxInstances = 0 };
                public static readonly SoundStyle Pop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaPop") { MaxInstances = 0 };
                public static readonly SoundStyle Explode = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaExplode") { MaxInstances = 0 };
                public static readonly SoundStyle EarRinging = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaEarRinging") { MaxInstances = 0 };
                public static readonly SoundStyle WindLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaWindLoop") { MaxInstances = 0, IsLooped = true };

                public static readonly SoundStyle SlimeJump = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSlimeJump", 1, 2) { MaxInstances = 0, PitchVariance = 0.1f };
            }
            public static class Goozmite
            {
                public static readonly SoundStyle Ambient = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Goozmite/GoozmiteAmbient", 1, 3) { MaxInstances = 0, Volume = 0.5f };
                public static readonly SoundStyle Death = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Goozmite/GoozmiteDeath", 1, 3) { MaxInstances = 0 };
                public static readonly SoundStyle Impact = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Goozmite/GoozmiteImpact", 1, 3) { MaxInstances = 0 };
            }
            public static class Slime
            {
                public static readonly SoundStyle SlimeSlam = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/GoozmaSlimeSlam", 1, 3) { MaxInstances = 0 };

                public static readonly SoundStyle CrimslimeTelegraph = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/CrimslimeTelegraph") { MaxInstances = 0 };

                public static readonly SoundStyle PrismDestroyerTelegraph = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PrismDestroyerTelegraph") { MaxInstances = 0, PitchVariance = 0.1f };
                public static readonly SoundStyle PrismDestroyerExpand = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PrismDestroyerExpand") { MaxInstances = 0, PitchVariance = 0.1f };
                public static readonly SoundStyle PixiePrismDestroyed = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PixiePrismDestroyed") { MaxInstances = 0 };
                public static readonly SoundStyle PixieBallExplode = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PixieBallExplode") { MaxInstances = 0 };
                public static readonly SoundStyle PixieBallBounce = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PixieBallBounce") { MaxInstances = 0, PitchVariance = 0.1f };
                public static readonly SoundStyle PixieBallLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PixieBallLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle Warning = new($"{nameof(CalamityHunt)}/Assets/Sounds/Alarm") { MaxInstances = 0 };

                public static readonly SoundStyle EbonstoneRaise = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/EbonstoneRaise") { MaxInstances = 0 };
                public static readonly SoundStyle EbonstoneToothTelegraph = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/EbonstoneToothTelegraph") { MaxInstances = 0, PitchVariance = 0.2f };
                public static readonly SoundStyle EbonstoneToothEmerge = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/EbonstoneToothEmerge", 1, 2) { MaxInstances = 0, PitchVariance = 0.3f };
                public static readonly SoundStyle EbonstoneCrumble = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/EbonstoneCrumble") { MaxInstances = 0 };

                public static readonly SoundStyle StellarSlimeStarfallTelegraph = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeStarfallTelegraph") { MaxInstances = 0 };
                public static readonly SoundStyle StellarSlimeImpact = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeImpact") { MaxInstances = 0 };
                public static readonly SoundStyle StellarReform = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeReform") { MaxInstances = 0 };
                public static readonly SoundStyle StellarBlackHoleSummon = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarBlackHoleSummon") { MaxInstances = 0 };
                public static readonly SoundStyle StellarBlackHoleLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarBlackHoleLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle StellarBlackHoleGulp = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeBlackHoleGulp") { MaxInstances = 0 };
                public static readonly SoundStyle StellarConstellationWave = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeConstellationWave") { MaxInstances = 0 };
                public static readonly SoundStyle StellarConstellationForm = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeConstellationForm") { MaxInstances = 0 };
            }
            public static class Weapon
            {
                public static readonly SoundStyle CrystalGauntletClap = new($"{nameof(CalamityHunt)}/Assets/Sounds/CrystalGauntletClap") { MaxInstances = 2, PitchVariance = 0.1f };
                public static readonly SoundStyle ScytheOfTheOldGodSwing = new($"{nameof(CalamityHunt)}/Assets/Sounds/OldGodScytheSwing_" + 0) { MaxInstances = 0, PitchVariance = 0.1f };
                public static readonly SoundStyle ScytheOfTheOldGodSwingStrong = new($"{nameof(CalamityHunt)}/Assets/Sounds/OldGodScytheSwing_" + 1) { MaxInstances = 0, PitchVariance = 0.1f };
                public static readonly SoundStyle GoomoireWindLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoomoireWindLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle SludgeShakerFiringLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/SludgeShakerFiringLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle TrailBlazerFireStart = new($"{nameof(CalamityHunt)}/Assets/Sounds/TrailBlazerFireStart") { MaxInstances = 0, PitchVariance = 0.1f };
                public static readonly SoundStyle TrailblazerFireLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/TrailBlazerFireLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle FissionFlyerExplode = new($"{nameof(CalamityHunt)}/Assets/Sounds/FissionFlyerExplode") { MaxInstances = 0 };
            }
            public static readonly SoundStyle MonsoonThunder = new($"{nameof(CalamityHunt)}/Assets/Sounds/SlimeMonsoon/GoozmaMonsoonThunder", 3, SoundType.Ambient) { MaxInstances = 0 };
            public static readonly SoundStyle SlimeRainActivate = new($"{nameof(CalamityHunt)}/Assets/Sounds/SlimeRainActivate") { MaxInstances = 0 };

            public static readonly SoundStyle GunCocking = new($"{nameof(CalamityHunt)}/Assets/Sounds/GunCocking") { MaxInstances = 0, PitchVariance = 0.02f };
            public static readonly SoundStyle SupremeRestorationBigGulp = new($"{nameof(CalamityHunt)}/Assets/Sounds/SupremeRestorationBigGulp") { MaxInstances = 0, PitchVariance = 0.05f };
            public static readonly SoundStyle BloatBabyWarbleLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/BabyBloatTravelLoop") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle GoozmoemRay = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmoemFusionRayShoot") { MaxInstances = 0, Volume = 0.8f };

            public static readonly SoundStyle StressActivate = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaRageActivate") { MaxInstances = 0, Volume = 0.8f };
            public static readonly SoundStyle StressPing = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaRageIndicator") { MaxInstances = 0, Volume = 0.65f };
            public static readonly SoundStyle StressLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaRageLoop") { MaxInstances = 0, Volume = 0.65f, IsLooped = true };
            public static readonly SoundStyle StressFull = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaRageFull") { MaxInstances = 0, Volume = 0.75f };

            public static readonly SoundStyle GoozmaAuricSoulHeartbeat = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaAuricSoulHeartbeat") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle GoozmaAuricSoulDrone = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaAuricSoulDrone") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle GoozmaAuricSoulBreathe = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaAuricSoulBreathe") { MaxInstances = 0, PitchVariance = 0.4f };

            public static readonly SoundStyle YharonAuricSoulHeartbeat = new($"{nameof(CalamityHunt)}/Assets/Sounds/YharonAuricSoulHeartbeat") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle YharonAuricSoulDrone = new($"{nameof(CalamityHunt)}/Assets/Sounds/YharonAuricSoulDrone") { MaxInstances = 0, Volume = 0.5f, IsLooped = true };
            public static readonly SoundStyle YharonAuricSoulBreathe = new($"{nameof(CalamityHunt)}/Assets/Sounds/YharonAuricSoulBreathe") { MaxInstances = 0, PitchVariance = 0.2f };
        }
    }
}
