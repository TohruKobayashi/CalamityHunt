#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems;

public sealed class GaslightSystem : ModSystem
{
    private bool allowLogging;
    private Hook? loggerIsEnabledForHook;
    private ILHook? modlistCommandActionHook;

    public override void Load()
    {
        base.Load();

        allowLogging = false;
        Type loggerType = typeof(Logger);
        MethodInfo loggerIsEnabledForMethod = loggerType.GetMethod("IsEnabledFor", BindingFlags.Public | BindingFlags.Instance)!;
        loggerIsEnabledForHook = new Hook(loggerIsEnabledForMethod, DisableLoggingWhenDisallowed);

        DoGaslighting();
        allowLogging = true;
    }

    public override void Unload()
    {
        base.Unload();

        loggerIsEnabledForHook?.Dispose();
        modlistCommandActionHook?.Dispose();
    }

    private void DoGaslighting()
    {
        Assembly tmlAsm = typeof(ModLoader).Assembly;

        Type? modlistCommandType = tmlAsm.GetType("Terraria.ModLoader.Default.ModlistCommand");
        MethodInfo modlistCommandActionMethod = modlistCommandType!.GetMethod("Action", BindingFlags.Public | BindingFlags.Instance)!;
        modlistCommandActionHook = new ILHook(modlistCommandActionMethod, HideModFromModListCommand);
    }

    private bool DisableLoggingWhenDisallowed(Func<Logger, Level, bool> orig, Logger self, Level level) => allowLogging && orig(self, level);

    private void HideModFromModListCommand(ILContext il)
    {
        ILCursor c = new ILCursor(il);

        int modListStloc = -1;

        if (!c.TryGotoNext(x => x.MatchCall("Terraria.ModLoader.ModLoader", "get_Mods"))) {
            return;
        }

        if (!c.TryGotoNext(MoveType.After, x => x.MatchStloc(out modListStloc))) {
            return;
        }

        c.Emit(OpCodes.Ldloc, modListStloc);
        c.EmitDelegate((IEnumerable<Mod> mods) => {
            if (!ModLoader.HasMod(HUtils.CalamityMod)) {
                return mods;
            }

            // TODO: Should we use GetMod here instead of GetInstance?
            CalamityHunt calamityHunt = ModContent.GetInstance<CalamityHunt>();

            // We can also check internal names instead of pointer equality.
            return mods.Where(x => x != calamityHunt);
        });
        c.Emit(OpCodes.Stloc, modListStloc);
    }
}
