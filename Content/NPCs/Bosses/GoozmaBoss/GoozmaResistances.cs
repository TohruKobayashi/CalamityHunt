using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;

public static class GoozmaResistances
{
    public static void GoozmaProjectileResistances(Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (ModLoader.HasMod(HUtils.CalamityMod)) {
            List<(int, float)> projectiles = new List<(int, float)> { };
            Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
            //projectiles.Add((calamity.Find<ModProjectile>("EssenceScythe").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("ExcelsusMain").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("ExcelsusPink").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("ExcelsusBlue").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("LaserFountain").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("TaintedBladeSlasher").Type, 0.5f));
            /*projectiles.Add((calamity.Find<ModProjectile>("GaelSkull").Type, 0.6f));*/
            projectiles.Add((calamity.Find<ModProjectile>("LaceratorYoyo").Type, 4f));
            /*projectiles.Add((calamity.Find<ModProjectile>("CosmicDischargeFlail").Type, 0.75f));
            projectiles.Add((calamity.Find<ModProjectile>("CosmicIceBurst").Type, 0.75f));*/
            projectiles.Add((calamity.Find<ModProjectile>("DragonPowFlail").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("Waterfall").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("DraconicSpark").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("SpineOfThanatosProjectile").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("PrismRay").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("ArkoftheCosmosBlast").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("ArkoftheCosmosConstellation").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("ArkoftheCosmosParryHoldout").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("ArkoftheCosmosSwungBlade").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("EonBolt").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("RendingNeedle").Type, 0.9f));
            /*projectiles.Add((calamity.Find<ModProjectile>("MurasamaSlash").Type, 0.6f));
            projectiles.Add((calamity.Find<ModProjectile>("PhaseslayerBeam").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("PhaseslayerProjectile").Type, 0.85f));*/
            projectiles.Add((calamity.Find<ModProjectile>("PrismTooth").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("PhotonRipperProjectile").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("DragonRageStaff").Type, 0.56f));
            projectiles.Add((calamity.Find<ModProjectile>("FuckYou").Type, 0.56f));
            /*projectiles.Add((calamity.Find<ModProjectile>("DracoBeam").Type, 0.4f));
            projectiles.Add((calamity.Find<ModProjectile>("EarthProj").Type, 0.1f));
            projectiles.Add((calamity.Find<ModProjectile>("Earth2").Type, 0.1f));
            projectiles.Add((calamity.Find<ModProjectile>("Earth3").Type, 0.1f));
            projectiles.Add((calamity.Find<ModProjectile>("Earth4").Type, 0.1f));
            projectiles.Add((calamity.Find<ModProjectile>("ElementalExcaliburBeam").Type, 0.25f));
            projectiles.Add((calamity.Find<ModProjectile>("RSSolarFlare").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("TriactisHammerProj").Type, 0.7f));*/

            projectiles.Add((calamity.Find<ModProjectile>("ExoLightningBolt").Type, 0.8f));
            projectiles.Add((calamity.Find<ModProjectile>("CardClub").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("CardClubSplit").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("CardDiamond").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("CardHeart").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("CardSpade").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("CondemnationArrow").Type, 0.7f));
            projectiles.Add((calamity.Find<ModProjectile>("CondemnationArrowHoming").Type, 0.7f));
            //projectiles.Add((calamity.Find<ModProjectile>("DragonsBreathRound").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("PrismMine").Type, 0.65f));
            /*projectiles.Add((calamity.Find<ModProjectile>("PrismaticEnergyBlast").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("PrismEnergyBullet").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("PrismComet").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("PiercingBullet").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("MarksmanShot").Type, 0.01f));
            projectiles.Add((calamity.Find<ModProjectile>("ChickenRocket").Type, 0.8f));
            projectiles.Add((calamity.Find<ModProjectile>("ChickenExplosion").Type, 0.8f));
            projectiles.Add((calamity.Find<ModProjectile>("EssenceFire").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("ExoFire").Type, 0.75f));
            projectiles.Add((calamity.Find<ModProjectile>("ExoLight").Type, 0.8f));
            projectiles.Add((calamity.Find<ModProjectile>("AnomalysNanogunPlasmaBeam").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("AnomalysNanogunMPFBDevastator").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("AnomalysNanogunMPFBBoom").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("NorfleetComet").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("PulseRifleShot").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("ContagionArrow").Type, 0.6f));
            projectiles.Add((calamity.Find<ModProjectile>("ContagionBall").Type, 0.6f));
            projectiles.Add((ProjectileID.BulletHighVelocity, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("Voidragon").Type, 0.35f));
            projectiles.Add((calamity.Find<ModProjectile>("VoidragonTentacle").Type, 0.35f));
            projectiles.Add((calamity.Find<ModProjectile>("PlasmaExplosion").Type, 0.35f));*/
            projectiles.Add((calamity.Find<ModProjectile>("GodSlayerSlugProj").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("HolyFireBulletProj").Type, 0.85f));

            /*projectiles.Add((calamity.Find<ModProjectile>("HolyFlame").Type, 0.75f));
            projectiles.Add((calamity.Find<ModProjectile>("VividBeam").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("VividClarityBeam").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("VividBolt").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("ClimaxProj").Type, 0.95f));
            projectiles.Add((calamity.Find<ModProjectile>("ClimaxBeam").Type, 0.95f));
            projectiles.Add((calamity.Find<ModProjectile>("VoidVortexProj").Type, 0.75f));*/
            projectiles.Add((calamity.Find<ModProjectile>("InfernadoFriendly").Type, 0.3f));
            projectiles.Add((calamity.Find<ModProjectile>("Vehemence").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("VehemenceSkull").Type, 0.85f));
            /*projectiles.Add((calamity.Find<ModProjectile>("AetherBeam").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("TeslaCannonShot").Type, 0.9f));*/
            projectiles.Add((calamity.Find<ModProjectile>("RedirectingLostSoul").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("RedirectingVengefulSoul").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("RedirectingGildedSoul").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("Ancient").Type, 0.75f));
            projectiles.Add((calamity.Find<ModProjectile>("Ancient2").Type, 0.75f));
            /*projectiles.Add((calamity.Find<ModProjectile>("EnormousConsumingVortex").Type, 0.75f));
            projectiles.Add((calamity.Find<ModProjectile>("ExoVortex").Type, 0.8f));
            projectiles.Add((calamity.Find<ModProjectile>("MelterNote1").Type, 0.95f));
            projectiles.Add((calamity.Find<ModProjectile>("MelterNote2").Type, 0.95f));
            projectiles.Add((calamity.Find<ModProjectile>("SpiritCongregation").Type, 0.25f));*/
            projectiles.Add((calamity.Find<ModProjectile>("FabBolt").Type, 0.022f));
            projectiles.Add((calamity.Find<ModProjectile>("FabRay").Type, 0.022f));
            /*projectiles.Add((calamity.Find<ModProjectile>("BlushieStaffProj").Type, 0.1f));
            projectiles.Add((calamity.Find<ModProjectile>("RainbowComet").Type, 0.05f));
            projectiles.Add((calamity.Find<ModProjectile>("RainbowRocket").Type, 0.05f));
            projectiles.Add((calamity.Find<ModProjectile>("ApotheosisWorm").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("ApotheosisEnergy").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("LightBlade").Type, 0.15f));
            projectiles.Add((calamity.Find<ModProjectile>("BlindingLight").Type, 0.15f));
            projectiles.Add((calamity.Find<ModProjectile>("EternityCrystal").Type, 0.15f));
            projectiles.Add((calamity.Find<ModProjectile>("EternityCircle").Type, 0.15f));


            projectiles.Add((calamity.Find<ModProjectile>("CannonLaserbeam").Type, 0.75f));
            projectiles.Add((calamity.Find<ModProjectile>("MinionLaserBurst").Type, 0.75f));
            projectiles.Add((calamity.Find<ModProjectile>("MinionTeslaOrb").Type, 0.65f));
            projectiles.Add((calamity.Find<ModProjectile>("MinionPlasmaBlast").Type, 0.75f));
            projectiles.Add((calamity.Find<ModProjectile>("MinionGaussBoom").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("PowerfulRaven").Type, 0.75f));
            projectiles.Add((calamity.Find<ModProjectile>("CosmicEnergySpiral").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("CosmicBlast").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("CosmicBlastBig").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("SepulcherMinion").Type, 0.8f));
            projectiles.Add((calamity.Find<ModProjectile>("BrimstoneDartMinion").Type, 0.8f));
            projectiles.Add((calamity.Find<ModProjectile>("SeekerSummonProj").Type, 0.8f));
            projectiles.Add((calamity.Find<ModProjectile>("BrimstoneDartSummon").Type, 0.8f));
            projectiles.Add((calamity.Find<ModProjectile>("AquasScepterCloudFlash").Type, 0.75f));
            projectiles.Add((calamity.Find<ModProjectile>("AquasScepterTeslaAura").Type, 0.75f));
            projectiles.Add((calamity.Find<ModProjectile>("AquasScepterRaindrop").Type, 0.75f));*/
            projectiles.Add((calamity.Find<ModProjectile>("AtlasMunitionsLaser").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("AtlasMunitionsLaserOverdrive").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("LostSoulSmall").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("LostSoulGold").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("LostSoulGiant").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("LostSoulLarge").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("LostSoulSmall").Type, 0.85f));
            /*projectiles.Add((calamity.Find<ModProjectile>("EndoBeam").Type, 0.75f));
            projectiles.Add((calamity.Find<ModProjectile>("EndoIceShard").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("EndoCooperBody").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("EndoCooperLimbs").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("EndoFire").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("MagicArrow").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("MagicAxe").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("MagicBird").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("MagicBullet").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("MagicBulletBig").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("MagicBunny").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("MagicHammer").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("MagicRifle").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("MagicTree").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("MagicUmbrella").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("ProfanedCrystalMageFireball").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("ProfanedCrystalMageFireballSplit").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("ProfanedCrystalMeleeSpear").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("ProfanedCrystalRangedHuges").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("ProfanedCrystalRangedSmalls").Type, 0.5f));
            projectiles.Add((calamity.Find<ModProjectile>("ProfanedCrystalRogueShard").Type, 0.5f));

            projectiles.Add((calamity.Find<ModProjectile>("SupernovaBomb").Type, 0.6f));
            projectiles.Add((calamity.Find<ModProjectile>("SupernovaBoom").Type, 0.6f));
            projectiles.Add((calamity.Find<ModProjectile>("CelestusProj").Type, 0.8f));
            projectiles.Add((calamity.Find<ModProjectile>("CelestusMiniScythe").Type, 0.8f));*/
            projectiles.Add((calamity.Find<ModProjectile>("DragonScalesInfernado").Type, 0.7f));
            projectiles.Add((calamity.Find<ModProjectile>("DynamicPursuerProjectile").Type, 0.65f));
            projectiles.Add((calamity.Find<ModProjectile>("DynamicPursuerElectricity").Type, 0.65f));
            projectiles.Add((calamity.Find<ModProjectile>("DynamicPursuerLaser").Type, 0.65f));
            //projectiles.Add((calamity.Find<ModProjectile>("SacrificeProjectile").Type, 0.9f));
            projectiles.Add((calamity.Find<ModProjectile>("SeraphimProjectile").Type, 0.01f));
            projectiles.Add((calamity.Find<ModProjectile>("SeraphimDagger").Type, 0.01f));
            projectiles.Add((calamity.Find<ModProjectile>("SeraphimAngelicLight").Type, 0.01f));
            projectiles.Add((calamity.Find<ModProjectile>("SeraphimAngelicLight2").Type, 0.01f));
            projectiles.Add((calamity.Find<ModProjectile>("SeraphimBeamLarge").Type, 0.01f));
            /*projectiles.Add((calamity.Find<ModProjectile>("TheAtomSplitterProjectile").Type, 0.66f));
            projectiles.Add((calamity.Find<ModProjectile>("TheAtomSplitterDuplicate").Type, 0.66f));*/
            projectiles.Add((calamity.Find<ModProjectile>("WrathwingSpear").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("WrathwingCinder").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("WrathwingFireball").Type, 0.85f));
            projectiles.Add((calamity.Find<ModProjectile>("FinalDawnFireball").Type, 0.2f));
            projectiles.Add((calamity.Find<ModProjectile>("FinalDawnFlame").Type, 0.2f));
            projectiles.Add((calamity.Find<ModProjectile>("FinalDawnHorizontalSlash").Type, 0.2f));
            projectiles.Add((calamity.Find<ModProjectile>("FinalDawnProjectile").Type, 0.2f));
            projectiles.Add((calamity.Find<ModProjectile>("FinalDawnThrow").Type, 0.2f));
            projectiles.Add((calamity.Find<ModProjectile>("FinalDawnThrow2").Type, 0.2f));
            /*projectiles.Add((calamity.Find<ModProjectile>("RefractionRotorProjectile").Type, 0.8f));
            projectiles.Add((calamity.Find<ModProjectile>("PrismShurikenBlade").Type, 0.8f));
            projectiles.Add((calamity.Find<ModProjectile>("NanoblackMain").Type, 0.01f));
            projectiles.Add((calamity.Find<ModProjectile>("NanoblackSplit").Type, 0.01f));
            projectiles.Add((calamity.Find<ModProjectile>("ScarletDevilProjectile").Type, 0.1f));
            projectiles.Add((calamity.Find<ModProjectile>("ScarletDevilBullet").Type, 0.1f));
            projectiles.Add((calamity.Find<ModProjectile>("ScarletBlast").Type, 0.1f));*/

            for (int i = 0; i < projectiles.Count; i++) {
                if (projectile.type == projectiles[i].Item1) {
                    modifiers.SourceDamage *= projectiles[i].Item2;
                }
            }

            if (projectile.minion) {
                modifiers.SourceDamage *= 0.85f;
            }
        }
    }
    public static void GoozmaItemResistances(Item item, ref NPC.HitModifiers modifiers) //TRUE MELEE (holdouts don't count)
    {
        // "give everything a resistance to true melee"
        modifiers.SourceDamage *= 0.7f;
    }

    public static void DisablePointBlank()
    {
        if (ModLoader.HasMod(HUtils.CalamityMod)) {
            Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
            for (int i = 0; i < Main.maxProjectiles; i++) {
                Projectile proj = Main.projectile[i];
                if (proj.active && (int)calamity.Call("GetPointBlankDuration", proj) > 0) {
                    calamity.Call("SetPointBlankDuration", proj, 0);
                }
            }
        }
    }
}
