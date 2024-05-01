using CalamityHunt.Common.Graphics.SlimeMonsoon;
using CalamityHunt.Content.Items.Misc.AuricSouls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class GoozmaAuricSoulScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/GoozmaAuricSoulMusic");

        public override bool IsSceneEffectActive(Player player)
        {
            bool active = Main.item.Any(n => n.active && n.type == ModContent.ItemType<GoozmaAuricSoul>());

            if (active)
            {
                player.GetModPlayer<EffectTilePlayer>().effectorCount["SlimeMonsoon"] = 5;
                SlimeMonsoonBackground.lightningEnabled = false;
                Main.windSpeedTarget = 1;
            }
            return active;
        }
    }

    public class YharonAuricSoulScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/YharonAuricSoulMusic");

        public override bool IsSceneEffectActive(Player player)
        {
            bool active = Main.item.Any(n => n.active && n.type == ModContent.ItemType<FieryAuricSoul>());

            if (active && ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
            }
            return active;
        }
    }
}
