using System;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.GlobalNPCs;

public class DoomedNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public int doomCount;

    public override void PostAI(NPC npc)
    {
        if (doomCount > 0) {
            doomCount--;

            if (Main.rand.NextBool(4)) {
                Color color = Color.Lerp(Color.Turquoise, AntiMassAccumulatorProj.MainColor, Main.rand.NextBool(20).ToInt()) with { A = 20 };
                Vector2 sparkPosition = Main.rand.NextVector2FromRectangle(npc.Hitbox);
                Vector2 sparkVelocity = sparkPosition.DirectionFrom(npc.Bottom) * Main.rand.NextFloat(2f);
                Dust sparks = Dust.NewDustPerfect(sparkPosition, 278, sparkVelocity.RotatedByRandom(0.2f), 0, color * 0.5f, Main.rand.NextFloat(0.9f));
                sparks.noGravity = true;
            }

            CalamityHunt.Particles.SpawnParticle<LightningParticle>(particle => {
                particle.Position = Main.rand.NextVector2FromRectangle(npc.Hitbox);
                particle.Velocity = particle.Position.DirectionFrom(npc.Bottom) * Main.rand.NextFloat(2f);
                particle.Rotation = particle.Velocity.ToRotation() + Main.rand.NextFloat(-0.5f, 0.5f);
                particle.Scale = new Vector2(Main.rand.NextFloat(0.3f, 0.51f + MathF.Sqrt(npc.width / 150f + npc.height / 150f)));
                particle.Color = Color.Turquoise with { A = 40 };
                particle.anchor = () => npc.velocity;
            });
        }
    }
}
