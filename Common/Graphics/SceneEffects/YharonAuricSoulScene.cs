using CalamityHunt.Content.Items.Misc.AuricSouls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;
using CalamityHunt.Common.Systems;

namespace CalamityHunt.Common.Graphics.SceneEffects;

public class YharonAuricSoulScene : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;

    public override int Music => AssetDirectory.Music.YharonSoul;

    private static bool interludeConfigWasOn = false;

    public override bool IsSceneEffectActive(Player player)
    {
        bool active = Main.item.Any(n => n.active && n.type == ModContent.ItemType<YharonSoul>());

        if (ModLoader.HasMod(HUtils.CalamityMod)) {
            if (active) {
                // Disable the interlude 3 config
                if ((bool)YharonReflectionSystem.interlude3Config.GetValue(YharonReflectionSystem.calamityConfig)) {
                    YharonReflectionSystem.interlude3Config.SetValue(YharonReflectionSystem.calamityConfig, false);
                    interludeConfigWasOn = true;
                }
                // Set the Yharon monolith to on
                if (YharonReflectionSystem.yharonolithField != null)
                    YharonReflectionSystem.yharonolithField.SetValue(YharonReflectionSystem.calPlayer, 30);
            }
            else {
                // If the config was enabled and the scene is disabled, turn the config back on
                if (interludeConfigWasOn) {
                    YharonReflectionSystem.interlude3Config.SetValue(YharonReflectionSystem.calamityConfig, true);
                }
            }
        }
        return active;
    }
}
