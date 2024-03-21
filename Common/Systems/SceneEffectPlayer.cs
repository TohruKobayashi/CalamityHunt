using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class SceneEffectPlayer : ModPlayer
    {
        public enum EffectorType : ushort
        {
            SlimeMonsoon,
        }

        public Dictionary<ushort, int> effectActive = new Dictionary<ushort, int>()
        {
            { (ushort)EffectorType.SlimeMonsoon, 0 },
        };

        public override void ResetEffects()
        {
            for (int i = 0; i < effectActive.Count; i++) {
                effectActive[(ushort)i] = Math.Max(0, effectActive[(ushort)i] - 1);
            }
        }

        public override void PostUpdateMiscEffects()
        {
            //Player.ManageSpecialBiomeVisuals("HuntOfTheOldGods:SlimeMonsoon", effectorCount[(ushort)EffectorType.SlimeMonsoon] > 0, Player.Center);
        }
    }
}
