using CalamityHunt.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Graphics.SlimeMonsoon
{
    public class SlimeMonsoonSceneEffect : ModSceneEffect
    {
        public override string MapBackground => $"{nameof(CalamityHunt)}/Assets/Textures/SlimeMonsoonBG";
        public override bool IsSceneEffectActive(Player player) => GoozmaSystem.GoozmaActive;
    }
}
