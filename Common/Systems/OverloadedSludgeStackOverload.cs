using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems;

public class OverloadedSludgeStackOverload : GlobalItem
{
    public override void SetDefaults(Item entity)
    {
        // Overloaded Sludge now stacks to 9999
        if (ModCompatibility.Calamity.IsLoaded) {
            if (entity.type == ModCompatibility.Calamity.Mod.Find<ModItem>("OverloadedSludge").Type) {
                entity.maxStack = 9999;
            }
        }
    }
}
