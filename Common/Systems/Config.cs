using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CalamityHunt.Common.Systems
{
    public class Config : ModConfig
    {
        public static Config Instance;
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.CalamityHunt.Config.VisualHeader")]
        [DefaultValue(true)]
        public bool epilepsy { get; set; }

        [Range(0f, 1f)]
        [DefaultValue(1)]
        public float monsoonDistortion { get; set; }

        [DefaultValue(true)]
        public bool monsoonLightning { get; set; }

        [Header("$Mods.CalamityHunt.Config.StressHeader")]
        [Range(0f, 100f)]
        [DefaultValue(47.5)]
        public float stressX { get; set; }

        [Range(0f, 100f)]
        [DefaultValue(3.97614312f)]
        public float stressY { get; set; }
        [Range(0f, 4f)]
        [DefaultValue(2f)]
        public float stressShake { get; set; }

        [DefaultValue(true)]
        public bool shadowspecCurse;

    }
}
