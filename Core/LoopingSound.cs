using System;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria.Audio;

namespace CalamityHunt.Core;

public class LoopingSound
{
    private SoundStyle style;
    private SlotId slot;
    private Func<bool> condition;

    public LoopingSound(SoundStyle soundStyle, Func<bool> activeCondition)
    {
        style = soundStyle;
        condition = activeCondition;
        slot = SlotId.Invalid;
    }

    public void Update(Func<Vector2> position, Func<float> volume, Func<float> pitch)
    {
        bool active = SoundEngine.TryGetActiveSound(slot, out ActiveSound activeSound);

        if ((!active || !slot.IsValid) && condition.Invoke())
        {
            slot = SoundEngine.PlaySound(style, position.Invoke(), sound =>
            {
                if (sound != null)
                {
                    sound.Position = position.Invoke();
                    sound.Volume = volume.Invoke();
                    sound.Pitch = pitch.Invoke();
                }

                return condition.Invoke();
            });
        }

        if (active)
        {
            activeSound.Position = position.Invoke();
            activeSound.Volume = volume.Invoke();
            activeSound.Pitch = pitch.Invoke();
        }
    }
}
