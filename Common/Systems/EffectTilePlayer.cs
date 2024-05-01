using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class EffectTilePlayer : ModPlayer
    {
        public Dictionary<string, int> effectorCount = new Dictionary<string, int>()
        {
            { "SlimeMonsoon", 0 } 
        };

        public override void ResetEffects()
        {
            if (effectorCount["SlimeMonsoon"] > 0)
                effectorCount["SlimeMonsoon"]--;
        }

        public override void PostUpdateMiscEffects()
        {
            if (GoozmaSystem.GoozmaActive)
                effectorCount["SlimeMonsoon"] = 60;

            Player.ManageSpecialBiomeVisuals("HuntOfTheOldGods:SlimeMonsoon", effectorCount["SlimeMonsoon"] > 0, Player.Center);
        }
    }
}
