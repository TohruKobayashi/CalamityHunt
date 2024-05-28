using CalamityHunt.Common.Players;
using CalamityHunt.Content.Items.Weapons.Summoner;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Buffs
{
    public class Absorption : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.HeldItem.type == ModContent.ItemType<Gobflogger>()) {
                player.GetModPlayer<GoozmaWeaponsPlayer>().gobfloggerBuff = true;
            }
        }
    }
}
