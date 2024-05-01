using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Weapons.Summoner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Buffs
{
    public class Absorption : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.HeldItem.type == ModContent.ItemType<Gobflogger>())
                player.GetModPlayer<GoozmaWeaponsPlayer>().gobfloggerBuff = true;
        }
    }
}
