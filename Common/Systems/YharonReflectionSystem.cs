﻿using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace CalamityHunt.Common.Systems;

public class YharonReflectionSystem : ModSystem
{
    public static bool SSO = false;
    public static PropertyInfo interlude3Config;
    public static FieldInfo yharonolithField = null;
    public static ModConfig calamityConfig;
    public static ModPlayer calPlayer = null;

    public override void PostSetupContent()
    {
        if (ModLoader.HasMod(HUtils.CalamityMod)) {
            // If the Nimble Bounder, an item added in the Sunken Sea update exists, then the Sunken Sea update did indeed happen
            if (ModLoader.GetMod(HUtils.CalamityMod).TryFind("NimbleBounder", out ModItem nim)) {
                SSO = true;
            }
            // Grab Calamity's config
            string configName = !SSO ? "CalamityConfig" : "CalamityClientConfig";
            calamityConfig = ModLoader.GetMod(HUtils.CalamityMod).GetConfig(configName);
            interlude3Config = calamityConfig.GetType().GetProperty("Interlude3", BindingFlags.Instance | BindingFlags.Public);
        }
    }

    public override void OnWorldLoad()
    {
        if (ModLoader.HasMod(HUtils.CalamityMod)) {
            // Grab the Yharon monolith field and store the player as well if this fella is empty
            if (calPlayer == null) {
                calPlayer = HUtils.GetCalamityModPlayer();
                yharonolithField = calPlayer.GetType().GetField("monolithYharonShader", BindingFlags.Public | BindingFlags.Instance);
            }
        }
    }

    public override void OnWorldUnload()
    {
        if (ModLoader.HasMod(HUtils.CalamityMod)) {
            // Unload the player and their yharonolith field
            calPlayer = null;
            yharonolithField = null;
        }
    }
}
