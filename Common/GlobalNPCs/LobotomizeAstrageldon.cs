﻿using System;
using System.Collections.Generic;
using System.Reflection;
using CalamityHunt.Common.DropRules;
using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Consumable;
using CalamityHunt.Content.Items.Dyes;
using CalamityHunt.Content.Items.Misc;
using CalamityHunt.Content.Items.Misc.AuricSouls;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.GlobalNPCs;
public class LobotomizeAstrageldon : GlobalNPC
{
    public static FieldInfo CatalystAstraNebulaField;
    public override void SetStaticDefaults()
    {
        if (ModLoader.TryGetMod(HUtils.CatalystMod, out Mod catalyst)) {
            ModNPC astrageldon = catalyst.Find<ModNPC>("Astrageldon");
            Type astrageldonType = astrageldon.GetType();

            CatalystAstraNebulaField = astrageldonType.GetField("NebulaForm", BindingFlags.Instance | BindingFlags.Public);
        }
    }
    public override bool PreAI(NPC npc)
    {
        if (ModLoader.HasMod(HUtils.CatalystMod)) {
            Mod catalyst = ModLoader.GetMod(HUtils.CatalystMod);
            int astrageldon = NPC.FindFirstNPC(catalyst.Find<ModNPC>("Astrageldon").Type);

            // if armageddon slime and goomba are present, lobotomize 
            // this also involves putting him out of nebula mode 
            if (NPC.AnyNPCs(ModContent.NPCType<Goozma>()) && npc.type == catalyst.Find<ModNPC>("Astrageldon").Type && npc.active) {
                npc.velocity *= 0.98f;
                //FieldInfo neb = astrageldonType.GetField("NebulaForm", BindingFlags.Instance | BindingFlags.Public);
                //neb.SetValue(npc.ModNPC, true);
                return false;
            }
        }
        return true;
    }
}
