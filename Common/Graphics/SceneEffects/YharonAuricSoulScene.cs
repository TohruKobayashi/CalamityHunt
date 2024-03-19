using System.Collections.Generic;
using System.Linq;
using CalamityHunt.Content.Items.Misc.AuricSouls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityHunt.Common.Graphics.SceneEffects;

public class YharonAuricSoulScene : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;

    public override int Music => AssetDirectory.Music.YharonSoul;

    public override bool IsSceneEffectActive(Player player)
    {
        bool active = Main.item.Any(n => n.active && n.type == ModContent.ItemType<YharonSoul>());

        if (active && ModLoader.HasMod(HUtils.CalamityMod)) {
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
        return active;
    }
}
