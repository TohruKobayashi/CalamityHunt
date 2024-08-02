using CalamityHunt.Content.Items.Misc.AuricSouls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using System.Linq;
using System.Reflection;
using System;

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
            string configName = !HUtils.SSO ? "CalamityConfig" : "CalamityClientConfig";
            // Disable the interlude 3 config
            ModConfig calConfig =    ModLoader.GetMod(HUtils.CalamityMod).GetConfig(configName);
            PropertyInfo interlude3 = calConfig.GetType().GetProperty("Interlude3", BindingFlags.Instance | BindingFlags.Public);

            if (active) {
                if ((bool)interlude3.GetValue(calConfig)) {
                    interlude3.SetValue(calConfig, false);
                    interludeConfigWasOn = true;
                }
                bool soulPresent = false;
                Item soul = null;

                for (int i = 0; i < Main.maxItems; i++) {
                    if (Main.item[i].type == ModContent.ItemType<YharonSoul>()) {
                        soul = Main.item[i];
                        soulPresent = true;
                    }
                    if (soulPresent) {
                        Vector2 itemPos = soul.position;
                        Filters.Scene.Activate("CalamityMod:Yharon", itemPos);
                    }
                }
            }
            else {
                if (interludeConfigWasOn) {
                    interlude3.SetValue(calConfig, true);
                }
            }
        }
        return active;
    }
}
