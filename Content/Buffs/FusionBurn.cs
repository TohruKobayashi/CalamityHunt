using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Buffs
{
    public class FusionBurn : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FusionBurnPlayer>().active = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FusionBurnNPC>().active = true;
        }
    }

    public class FusionBurnPlayer : ModPlayer
    {
        public bool active;

        public override void UpdateBadLifeRegen()
        {
            if (active) {
                Player.lifeRegen = -48;
                Player.lifeRegenTime = 0;

                if (Main.rand.NextBool(5)) {
                    Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Sand, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-3f, -1f), 0, Color.Black, 0.1f + Main.rand.NextFloat());
                    dust.noGravity = true;
                }

                Rectangle box = new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height);
                Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).Value; //goozma's main color

                if (Main.rand.NextBool(5)) {
                    CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                        particle.position = Main.rand.NextVector2FromRectangle(box);
                        particle.velocity = -Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(3f);
                        particle.scale = Main.rand.NextFloat(0.5f, 1.5f);
                        particle.color = glowColor;
                    }));
                }

                CalamityHunt.particles.Add(Particle.Create<FusionFlameParticle>(particle => {
                    particle.position = Main.rand.NextVector2FromRectangle(box);
                    particle.velocity = -Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(2f);
                    particle.scale = Main.rand.NextFloat(1f, 3f);
                    particle.maxTime = Main.rand.Next(15, 20);
                    particle.color = glowColor;
                    particle.fadeColor = glowColor * 0.6f;
                }));
            }
        }

        public override void ResetEffects()
        {
            active = false;
        }
    }

    public class FusionBurnNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool active;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (active) {
                npc.lifeRegenCount -= 120;
                npc.lifeRegen = -80;
                damage = 40;

                if (Main.rand.NextBool(5)) {
                    Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Sand, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-3f, -1f), 0, Color.Black, 0.3f + Main.rand.NextFloat());
                    dust.noGravity = true;
                }

                Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).Value with { A = 0 }; //oil, bc not from goozma

                if (Main.rand.NextBool(3)) {
                    CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                        particle.position = Main.rand.NextVector2FromRectangle(npc.Hitbox);
                        particle.velocity = -Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(3f);
                        particle.scale = Main.rand.NextFloat(0.5f, 1.5f);
                        particle.color = glowColor;
                    }));
                }

                CalamityHunt.particles.Add(Particle.Create<FusionFlameParticle>(particle => {
                    particle.position = Main.rand.NextVector2FromRectangle(npc.Hitbox);
                    particle.velocity = -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(1f, 5f);
                    particle.rotation = particle.velocity.ToRotation();
                    particle.scale = Main.rand.NextFloat(1f, 3f);
                    particle.maxTime = Main.rand.Next(15, 20);
                    particle.color = glowColor * 2f;
                    particle.fadeColor = glowColor * 0.4f;
                }));
            }
        }

        public override void ResetEffects(NPC npc)
        {
            active = false;
        }
    }
}
