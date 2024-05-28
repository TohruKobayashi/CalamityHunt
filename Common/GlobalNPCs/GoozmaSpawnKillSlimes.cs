using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.GlobalNPCs
{
    public class SlimeSucking : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override void PostAI(NPC npc)
        {
            //for (var i = 0; i < Main.maxProjectiles; i++) {
            //    var proj = Main.projectile[i];
            //    if (proj.active && proj.type == ModContent.ProjectileType<GoozmaSpawn>()) {
            //        var velocity = npc.Center.DirectionTo(proj.Center).SafeNormalize(Vector2.One).RotatedByRandom(3.0);
            //        switch (npc.type) {
            //            case NPCID.BlueSlime:
            //                Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlimeParticleBehavior>());
            //                break;
            //            case NPCID.GreenSlime:
            //                Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlimeParticleBehavior>());
            //                break;
            //            case NPCID.Pinky:
            //                Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlimeParticleBehavior>());
            //                break;
            //            case NPCID.RedSlime:
            //                Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlimeParticleBehavior>());
            //                break;
            //            case NPCID.JungleSlime:
            //                Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlimeParticleBehavior>());
            //                break;
            //            case NPCID.BlackSlime:
            //                Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlimeParticleBehavior>());
            //                break;
            //            case NPCID.BabySlime:
            //                Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlimeParticleBehavior>());
            //                break;
            //            case NPCID.PurpleSlime:
            //                Slime(proj, npc, velocity, ModContent.GetInstance<FlyingBigSlimeParticleBehavior>());
            //                break;
            //            case NPCID.YellowSlime:
            //                Slime(proj, npc, velocity, ModContent.GetInstance<FlyingBigSlimeParticleBehavior>());
            //                break;
            //            case NPCID.MotherSlime:
            //                Slime(proj, npc, velocity, ModContent.GetInstance<FlyingBigSlimeParticleBehavior>());
            //                break;
            //            case NPCID.WindyBalloon:
            //                var npc2 = npc.AI_113_WindyBalloon_GetSlaveNPC();
            //                switch (npc2.type) {
            //                    case NPCID.BlueSlime:
            //                        Balloon(proj, npc2, velocity, ModContent.GetInstance<FlyingBalloonSlimeParticleBehavior>(), npc.frame.Y);
            //                        break;
            //                    case NPCID.GreenSlime:
            //                        Balloon(proj, npc2, velocity, ModContent.GetInstance<FlyingBalloonSlimeParticleBehavior>(), npc.frame.Y);
            //                        break;
            //                    case NPCID.PurpleSlime:
            //                        Balloon(proj, npc2, velocity, ModContent.GetInstance<FlyingBalloonSlimeParticleBehavior>(), npc.frame.Y);
            //                        break;
            //                    case NPCID.Pinky:
            //                        Balloon(proj, npc2, velocity, ModContent.GetInstance<FlyingBalloonSlimeParticleBehavior>(), npc.frame.Y);
            //                        break;
            //                    default:
            //                        break;
            //                }
            //                npc.life = 0;
            //                break;
            //            case NPCID.RainbowSlime:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingRainbowSlimeParticleBehavior>());
            //                break;
            //            case NPCID.Gastropod:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingGastropodParticleBehavior>());
            //                break;
            //            case NPCID.IlluminantSlime:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingIlluminantSlimeParticleBehavior>());
            //                break;
            //            case NPCID.LavaSlime:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingLavaSlimeParticleBehavior>());
            //                break;
            //            case NPCID.SlimedZombie:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingZombieSlimeParticleBehavior>());
            //                break;
            //            case NPCID.ArmedZombieSlimed:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingZombieSlimeParticleBehavior>());
            //                break;
            //            case NPCID.ShimmerSlime:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingShimmerSlimeParticleBehavior>());
            //                break;
            //            case NPCID.IceSlime:
            //                Ice(proj, npc, velocity, ModContent.GetInstance<FlyingIceSlimeParticleBehavior>(), false);
            //                break;
            //            case NPCID.SandSlime:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingSandSlimeParticleBehavior>());
            //                break;
            //            case NPCID.SpikedJungleSlime:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingJungleSlimeSpikedParticleBehavior>());
            //                break;
            //            case NPCID.SpikedIceSlime:
            //                Ice(proj, npc, velocity, ModContent.GetInstance<FlyingIceSlimeParticleBehavior>(), true);
            //                break;
            //            case NPCID.SlimeSpiked:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingSpikedSlimeParticleBehavior>());
            //                break;
            //            case NPCID.QueenSlimeMinionPink:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingBouncySlimeParticleBehavior>());
            //                break;
            //            case NPCID.QueenSlimeMinionBlue:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCrystalSlimeParticleBehavior>());
            //                break;
            //            case NPCID.QueenSlimeMinionPurple:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingHeavenlySlimeParticleBehavior>());
            //                break;
            //            case NPCID.UmbrellaSlime:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingUmbrellaSlimeParticleBehavior>());
            //                break;
            //            case NPCID.CorruptSlime:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCorruptSlimeParticleBehavior>());
            //                break;
            //            case NPCID.Slimeling:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCorruptSlimeParticleBehavior>());
            //                break;
            //            case NPCID.Slimer:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingSlimerParticleBehavior>());
            //                break;
            //            case NPCID.Slimer2:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCorruptSlimeParticleBehavior>());
            //                break;
            //            case NPCID.Crimslime:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCrimslimeParticleBehavior>());
            //                break;
            //            case NPCID.ToxicSludge:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingToxicSludgeParticleBehavior>());
            //                break;
            //            case NPCID.DungeonSlime:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingDungeonSlimeParticleBehavior>());
            //                break;
            //            case NPCID.HoppinJack:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingHoppinJackParticleBehavior>());
            //                break;
            //            case NPCID.GoldenSlime:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingGoldSlimeParticleBehavior>());
            //                break;
            //            case NPCID.BunnySlimed:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingBunnySlimeParticleBehavior>());
            //                break;
            //            case NPCID.SlimeMasked:
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingSlimeBunnyParticleBehavior>());
            //                break;
            //            case NPCID.SlimeRibbonRed:
            //                Ribbon(proj, npc, velocity, ModContent.GetInstance<FlyingPresentSlimeParticleBehavior>(), 0);
            //                break;
            //            case NPCID.SlimeRibbonGreen:
            //                Ribbon(proj, npc, velocity, ModContent.GetInstance<FlyingPresentSlimeParticleBehavior>(), 1);
            //                break;
            //            case NPCID.SlimeRibbonYellow:
            //                Ribbon(proj, npc, velocity, ModContent.GetInstance<FlyingPresentSlimeParticleBehavior>(), 2);
            //                break;
            //            case NPCID.SlimeRibbonWhite:
            //                Ribbon(proj, npc, velocity, ModContent.GetInstance<FlyingPresentSlimeParticleBehavior>(), 3);
            //                break;
            //            default:
            //                break;
            //        }
            //        if (ModLoader.HasMod(HuntOfTheOldGodsUtils.CalamityModName)) {
            //            if (npc.type == ModContent.Find<ModNPC>("CalamityMod/AeroSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingAeroSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/EbonianBlightSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingEbonianBlightSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CrimulanBlightSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCrimulanBlightSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CorruptSlimeSpawn").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCorruptSlimeSpawnParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CorruptSlimeSpawn2").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCorruptSlimeSpawnParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CrimsonSlimeSpawn").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCrimsonSlimeSpawnParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CrimsonSlimeSpawn2").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCrimsonSlimeSpawnParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/AstralSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingAstralSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CryoSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCryoSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/IrradiatedSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingIrradiatedSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/InfernalCongealment").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCharredSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/PerennialSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingPerennialSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/AureusSpawn").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingAureusSpawnSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/PestilentSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingPestilentSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/BloomSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingBloomSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/GammaSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingGammaSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CragmawMire").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCragmawMireParticleBehavior>());
            //        }
            //        if (ModLoader.HasMod(HuntOfTheOldGodsUtils.CatalystModName)) {
            //            if (npc.type == ModContent.Find<ModNPC>("CatalystMod/NovaSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingNovaSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CatalystMod/NovaSlimer").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingNovaSlimerParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CatalystMod/MetanovaSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingMetanovaSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CatalystMod/WulfrumSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingWulfrumSlimeParticleBehavior>());
            //            else if (npc.type == ModContent.Find<ModNPC>("CatalystMod/AscendedAstralSlime").Type)
            //                Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingAscendedAstralSlimeParticleBehavior>());
            //        }
            //    }
            //}
        }
        private static void Slime(Projectile proj, NPC npc, Vector2 velocity, Particle particleBehavior)
        {
            //var entity = Particle.NewParticle(particleBehavior, npc.Center, velocity, npc.color, npc.scale);
            //entity.Add(new ParticleData<Vector2> { Value = proj.Center }, new ParticleDrawBehindEntities());
            //ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
            //flyingSlime.Time = 20;
            //npc.life = 0;
        }
        private static void Slime2(Projectile proj, NPC npc, Vector2 velocity, Particle particleBehavior)
        {
            //var entity = Particle.NewParticle(particleBehavior, npc.Center, velocity, Color.White, npc.scale);
            //entity.Add(new ParticleData<Vector2> { Value = proj.Center }, new ParticleDrawBehindEntities());
            //ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
            //flyingSlime.Time = 20;
            //npc.life = 0;
        }
        private static void Ice(Projectile proj, NPC npc, Vector2 velocity, Particle particleBehavior, bool ice)
        {
            //var entity = Particle.NewParticle(particleBehavior, npc.Center, velocity, Color.White, npc.scale);
            //entity.Add(new ParticleData<Vector2> { Value = proj.Center }, new ParticleDrawBehindEntities());
            //ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
            //ref var iceSlime = ref entity.Get<ParticleFlyingIceSlime>();
            //flyingSlime.Time = 20;
            //iceSlime.Spiked = ice;
            //npc.life = 0;
        }
        private static void Balloon(Projectile proj, NPC npc, Vector2 velocity, Particle particleBehavior, int frame)
        {
            //var entity = Particle.NewParticle(particleBehavior, npc.Center, velocity, npc.color, npc.scale);
            //entity.Add(new ParticleData<Vector2> { Value = proj.Center }, new ParticleDrawBehindEntities());
            //ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
            //ref var balloonSlime = ref entity.Get<ParticleFlyingBalloonSlime>();
            //flyingSlime.Time = 20;
            //balloonSlime.BalloonVariant = frame - 1;
            //npc.life = 0;
        }
        private static void Ribbon(Projectile proj, NPC npc, Vector2 velocity, Particle particleBehavior, int variant)
        {
            //var entity = Particle.NewParticle(particleBehavior, npc.Center, velocity, Color.White, npc.scale);
            //entity.Add(new ParticleData<Vector2> { Value = proj.Center }, new ParticleDrawBehindEntities());
            //ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
            //ref var presentSlime = ref entity.Get<ParticleFlyingPresentSlime>();
            //flyingSlime.Time = 20;
            //presentSlime.Variant = variant;
            //npc.life = 0;
        }
    }
}
