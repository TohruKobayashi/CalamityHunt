using System.Linq;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.Particles;
using CalamityHunt.Content.Projectiles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Players
{
    public class SplendorJamPlayer : ModPlayer
    {
        public int tentacleCount;

        public bool active;
        public bool rainbow;
        public bool stressedOut;
        public int delay;
        public int wait;
        public float stress;
        public float checkStress;

        private bool t25, t50, t75;
        private bool activate;
        private bool playFull;
        private float gooTime;
        SlotId loopSlot;

        public override void ResetEffects()
        {
            rainbow = false;
            active = false;
            wait = 0;
            delay = 0;
            if (!stressedOut) {
                checkStress = stress;
                if (SoundEngine.TryGetActiveSound(loopSlot, out ActiveSound activeSound)) {
                    activeSound.Stop();
                }
            }
            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[0] < 15 && stress >= 0.25f && !stressedOut) {
                SoundEngine.PlaySound(AssetDirectory.Sounds.StressActivate, Player.Center);
                activate = true;
                stressedOut = true;
                Player.AddBuff(ModContent.BuffType<SplendorJamBuff>(), (int)(checkStress * 16 + 4) * 60);
            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            Color goo = new GradientColor(SlimeUtils.GoozOilColors, 0.5f, 0.5f).ValueAt(Main.GlobalTimeWrappedHourly * 100 + gooTime);
            if (rainbow) {
                if (drawInfo.shadow != 0) {
                    r = goo.R;
                    g = goo.G;
                    b = goo.B;
                }
            }
            if (drawInfo.shadow == 0 && Main.rand.NextBool(8) && stressedOut) {
                CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                    particle.position = Player.Center + Main.rand.NextVector2Circular(30, 40);
                    particle.velocity = Main.rand.NextVector2Circular(1, 1) - Vector2.UnitY * 3f;
                    particle.scale = 1f;
                    particle.color = goo;
                }));
            }
            if (activate && drawInfo.shadow == 0) {
                for (int i = -70; i < 71; i += 14) {
                    if (i < -14 || i > 42) {
                        CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                            particle.position = Player.Center + new Vector2(i - 14, i - 14);
                            particle.velocity = Vector2.Zero;
                            particle.scale = 1f;
                            particle.color = goo;
                        }));
                    }
                    if (i < -28 || i > 28) {
                        CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                            particle.position = Player.Center + new Vector2(i, -i);
                            particle.velocity = Vector2.Zero;
                            particle.scale = 1f;
                            particle.color = goo;
                        }));
                    }
                    CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                        particle.position = Player.Center + (Vector2.One * 24f).RotatedBy(MathHelper.ToRadians(36 * (i + 70) / 14));
                        particle.velocity = Vector2.Zero;
                        particle.scale = 1f;
                        particle.color = goo;
                    }));
                }
                activate = false;
            }
        }
        public override void FrameEffects()
        {
            if (rainbow) {
                Player.armorEffectDrawShadow = true;
            }
        }
        public override void UpdateEquips()
        {
            if (rainbow || active) {
                gooTime = (gooTime + 1) % 60;
                if (gooTime > 1200) {
                    gooTime = 0;
                }
            }
            if (active) {
                int damage = (int)GetBestClassDamage(Player).ApplyTo(200);
                Color goo = new GradientColor(SlimeUtils.GoozOilColors, 0.5f, 0.5f).ValueAt(Main.GlobalTimeWrappedHourly * 100 + gooTime);
                Lighting.AddLight(Player.Center, new Vector3(goo.R, goo.G, goo.B) * 0.01f * checkStress);
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SplendorTentacle>()] < Player.GetModPlayer<SplendorJamPlayer>().tentacleCount && Player.whoAmI == Main.myPlayer) {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<SplendorTentacle>(), damage, 0.5f, Player.whoAmI);
                }

                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SplendorTentacle>()] > Player.GetModPlayer<SplendorJamPlayer>().tentacleCount) {
                    Main.projectile.First(n => n.active && n.owner == Player.whoAmI && n.type == ModContent.ProjectileType<SplendorTentacle>()).Kill();
                }

                tentacleCount = stressedOut ? tentacleCount : (int)(1 + stress * 4f);
                if (stress < 1f && !stressedOut) {
                    stress += 0.0001f;
                }
                else if (stress > 1f && !stressedOut) {
                    stress = 1f;
                }

                if (stress > 0.25f && !t25) {
                    SoundEngine.PlaySound(AssetDirectory.Sounds.StressPing with { Pitch = -0.4f }, Player.Center);
                    t25 = true;
                }
                else if (stress > 0.5f && !t50) {
                    SoundEngine.PlaySound(AssetDirectory.Sounds.StressPing with { Pitch = -0.2f }, Player.Center);
                    t50 = true;
                }
                if (stress > 0.755f && !t75) {
                    SoundEngine.PlaySound(AssetDirectory.Sounds.StressPing, Player.Center);
                    t75 = true;
                }
                if (stressedOut && stress > 0) {
                    stress -= checkStress / ((int)(checkStress * 16 + 4) * 60);
                    if (!SoundEngine.TryGetActiveSound(loopSlot, out ActiveSound activeSound)) {
                        loopSlot = SoundEngine.PlaySound(AssetDirectory.Sounds.StressLoop, Player.Center);
                    }
                    else {
                        activeSound.Position = Player.Center;
                    }
                }
                if (stressedOut && stress <= 0) {
                    t25 = false;
                    t50 = false;
                    t75 = false;
                    stressedOut = false;
                    stress = 0;
                    checkStress = 0;
                }
                if (wait > 0) {
                    wait--;
                }

                if (delay > 0) {
                    delay--;
                }
            }
            else {
                t25 = false;
                t50 = false;
                t75 = false;
                stressedOut = false;
                stress = 0;
                checkStress = 0;
            }
        }
        public override void PostUpdateMiscEffects()
        {
            if (Player.whoAmI == Main.myPlayer && playFull && checkStress >= 1f) {
                playFull = false;
                SoundEngine.PlaySound(AssetDirectory.Sounds.StressFull, Player.Center);
            }
            else if (!playFull && checkStress < 1f) {
                playFull = true;
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (active) {
                if (proj.type == ModContent.ProjectileType<SplendorTentacle>() || Main.myPlayer != proj.owner) {
                    return;
                }

                if (stress < 1f && delay <= 0 && !stressedOut && target.type != NPCID.TargetDummy) {
                    if (DummyCheck(target)) {
                        return;
                    }

                    float dis = Player.Distance(target.Center);
                    stress += 0.0005f + ((dis > 0 && dis <= 320) ? 0.0005f : 0.0005f - (dis / 320 * 0.0005f));
                    delay = 5;
                }
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<StressExplosion>()] < 1 && wait == 0 && checkStress >= 0.25f && checkStress < 0.75) {
                    Projectile.NewProjectile(proj.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<StressExplosion>(), (int)(damageDone * 0.2f), 0, Player.whoAmI, 0, 0);
                    wait = 60;
                }
                else if (Player.ownedProjectileCounts[ModContent.ProjectileType<StressExplosion>()] < 1 && wait == 0 && stress >= 0.75f) {
                    Projectile.NewProjectile(proj.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<StressExplosion>(), (int)(damageDone * 0.4f), 0, Player.whoAmI, 0, 1);
                    wait = 60;
                }
            }
        }
        public static bool DummyCheck(NPC npc)
        {
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                if (npc.type == ModContent.Find<ModNPC>("CalamityMod/SuperDummyNPC").Type) {
                    return true;
                }
                return false;
            }
            return false;
        }
        public static StatModifier GetBestClassDamage(Player player)
        {
            StatModifier ret = StatModifier.Default;
            StatModifier classless = player.GetTotalDamage<GenericDamageClass>();

            // Atypical damage stats are copied from "classless", like Avenger Emblem. This prevents stacking flat damage effects repeatedly.
            ret.Base = classless.Base;
            ret *= classless.Multiplicative;
            ret.Flat = classless.Flat;

            // Check the five Calamity classes to see what the strongest one is, and use that for the typical damage stat.
            float best = 1f;

            float melee = player.GetTotalDamage<MeleeDamageClass>().Additive;
            if (melee > best) {
                best = melee;
            }

            float ranged = player.GetTotalDamage<RangedDamageClass>().Additive;
            if (ranged > best) {
                best = ranged;
            }

            float magic = player.GetTotalDamage<MagicDamageClass>().Additive;
            if (magic > best) {
                best = magic;
            }

            // Summoner intentionally has a reduction. As the only class with no crit, it tends to have higher raw damage than other classes.
            float summon = player.GetTotalDamage<SummonDamageClass>().Additive * 0.75f;
            if (summon > best) {
                best = summon;
            }
            // We intentionally don't check whip class, because it inherits 100% from Summon

            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                DamageClass roguetype = ModLoader.GetMod(HUtils.CalamityMod).Find<DamageClass>("RogueDamageClass");
                float rogue = player.GetTotalDamage(roguetype).Additive;
                if (rogue > best) {
                    best = rogue;
                }
            }

            // Add the best typical damage stat, then return the full modifier.
            ret += best - 1f;
            return ret;
        }
    }
    internal class JamMelee : GlobalItem
    {
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            SplendorJamPlayer sp = player.GetModPlayer<SplendorJamPlayer>();
            if (sp.active && item.type != ItemID.Zenith) {
                if (sp.stress < 1f && sp.delay <= 0 && !sp.stressedOut && target.type != NPCID.TargetDummy) {
                    if (SplendorJamPlayer.DummyCheck(target)) {
                        return;
                    }

                    float dis = player.Distance(target.Center);
                    sp.stress += 0.0005f + ((dis > 0 && dis <= 320) ? 0.0005f : 0.0005f - (dis / 320 * 0.0005f));
                    sp.delay = 5;
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<StressExplosion>()] < 1 && sp.wait == 0 && sp.checkStress >= 0.25f && sp.checkStress < 0.75) {
                    Projectile.NewProjectile(player.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<StressExplosion>(), (int)(damageDone * 0.2f), 0, player.whoAmI, 0, 0);
                    sp.wait = 60;
                }
                else if (player.ownedProjectileCounts[ModContent.ProjectileType<StressExplosion>()] < 1 && sp.wait == 0 && sp.checkStress >= 0.75f) {
                    Projectile.NewProjectile(player.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<StressExplosion>(), (int)(damageDone * 0.4f), 0, player.whoAmI, 0, 1);
                    sp.wait = 60;
                }
            }
        }
    }
}
