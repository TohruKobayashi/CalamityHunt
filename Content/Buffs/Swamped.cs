using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace CalamityHunt.Content.Buffs
{
    public class Swamped : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }
    }

    public class SwampedPlayer : ModPlayer
    {
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Player.HasBuff<Swamped>())
            {
                r *= 0.6f;
                g *= 1f;
                b *= 0.7f;

                Dust slimeDust = Dust.NewDustDirect(Player.position - new Vector2(2), Player.width + 4, Player.height + 4, DustID.SlimeBunny, 0f, 0f, 190, new Color(0, 235, 90, 100), 1f + Main.rand.NextFloat());

                if (Main.rand.NextBool(2))
                    slimeDust.alpha += 25;

                if (Main.rand.NextBool(2))
                    slimeDust.alpha += 25;

                slimeDust.noLight = true;
                slimeDust.fadeIn = 1.3f;
                slimeDust.velocity *= 0.2f;
                slimeDust.velocity += Player.velocity * 0.2f;
            }
        }
    }

    public class SwampedNPC : GlobalNPC
    {
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff<Swamped>())
            {
                Dust slimeDust = Dust.NewDustDirect(npc.position - new Vector2(2), npc.width + 4, npc.height + 4, DustID.SlimeBunny, 0f, 0f, 190, new Color(0, 235, 90, 100), 1f + Main.rand.NextFloat());

                if (Main.rand.NextBool(2))
                    slimeDust.alpha += 25;

                if (Main.rand.NextBool(2))
                    slimeDust.alpha += 25;

                slimeDust.noLight = true;
                slimeDust.fadeIn = 1.3f;
                slimeDust.velocity *= 0.2f;
                slimeDust.velocity += npc.velocity * 0.2f;
            }
        }
    }
}
