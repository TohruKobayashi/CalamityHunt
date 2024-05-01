using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingNormalSlimeParticleBehavior : FlyingSlimeParticleBehavior
{
    public override void PostUpdate(in Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var scale = ref entity.Get<ParticleScale>();

        if (Main.rand.NextBool(3))
        {
            Dust slime = Dust.NewDustPerfect(position.Value + Main.rand.NextVector2Circular(20, 20), DustID.TintableDust, velocity.Value * 0.2f, 200, color.Value, 0.5f + Main.rand.NextFloat());
            slime.noGravity = true;
        }

        if (color.Value != Color.White)
            return;

        WeightedRandom<Color> slimeColor = new WeightedRandom<Color>();
        slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.GreenSlime].color, 1f);
        slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.BlueSlime].color, 0.9f);
        slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.RedSlime].color, 0.3f);
        slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.YellowSlime].color, 0.2f);
        slimeColor.Add(Color.Gray, 0.1f);
        slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.JungleSlime].color, 0.3f);
        color.Value = slimeColor.Get();

        if (Main.rand.NextBool(20))
        {
            color.Value = ContentSamples.NpcsByNetId[NPCID.PurpleSlime].color;
            scale.Value = ContentSamples.NpcsByNetId[NPCID.PurpleSlime].scale;
        }

        if (Main.rand.NextBool(3000))
        {
            color.Value = ContentSamples.NpcsByNetId[NPCID.Pinky].color;
            scale.Value = ContentSamples.NpcsByNetId[NPCID.Pinky].scale;
        }
    }
}