using System;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Players;

public class SpecialSoundPlayer : ModPlayer
{
    public LoopingSound trailBlazerSound;
    public float trailBlazerVolume;
    private bool trailBlazerInUse;

    public override void PostUpdateMiscEffects()
    {
        if (Player.HeldItem.type == ModContent.ItemType<Trailblazer>()) {
            if (Player.ItemAnimationJustStarted && !trailBlazerInUse) {
                SoundEngine.PlaySound(Player.HeldItem.UseSound.Value.WithVolumeScale(0.6f), Player.MountedCenter);
                trailBlazerInUse = true;
            }
            if (!Player.ItemAnimationActive) {
                trailBlazerInUse = false;
            }

            trailBlazerVolume += Player.ItemAnimationActive ? 0.2f : -0.05f;
        }
        else {
            trailBlazerVolume -= 0.05f;
            trailBlazerInUse = false;
        }

        trailBlazerVolume = MathHelper.Clamp(trailBlazerVolume, 0f, 0.6f);

        trailBlazerSound ??= new LoopingSound(AssetDirectory.Sounds.Weapons.TrailblazerFireLoop, () => trailBlazerVolume > 0.1f);

        if (trailBlazerVolume > 0.01f) {
            trailBlazerSound.PlaySound(() => Player.MountedCenter, () => trailBlazerVolume, () => MathF.Sin(Main.GlobalTimeWrappedHourly * 8) * 0.07f);
        }
        else {
            trailBlazerSound?.StopSound();
        }
    }
}
