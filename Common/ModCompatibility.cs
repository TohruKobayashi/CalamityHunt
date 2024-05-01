#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Terraria.ModLoader;

namespace CalamityHunt.Common;

/// <summary>
///     Compile-time- and runtime-consistent mod compatibility helpers.
/// </summary>
public static class ModCompatibility
{
    /// <summary>
    ///     Provides a mod's internal name. This is used for taking
    ///     advantage of how CLR generics work.
    /// </summary>
    public interface IModNameProvider
    {
        /// <summary>
        ///     The internal name of the mod.
        /// </summary>
        string ModName { get; }
    }

    /// <summary>
    ///     A base class for mod compatibility helpers. This is used for taking
    ///     advantage of how CLR generics work.
    /// </summary>
    /// <typeparam name="TModName">The mod name.</typeparam>
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public abstract class BaseModCompatibility<TModName> where TModName : IModNameProvider, new()
    {
        private static readonly IModNameProvider name_provider = new TModName();

        // TODO: We could just use mod != null, but who cares?
        private static readonly Lazy<bool> is_loaded = new(() => ModLoader.HasMod(ModName));
        private static readonly Lazy<Mod?> mod = new(() => ModLoader.TryGetMod(ModName, out var theMod) ? theMod : null);

        /// <summary>
        ///     The internal name of the mod.
        /// </summary>
        public static string ModName => name_provider.ModName;

        /// <summary>
        ///     Whether the mod is loaded.
        /// </summary>
        public static bool IsLoaded => is_loaded.Value;

        /// <summary>
        ///     The mod instance, if loaded.
        /// </summary>
        public static Mod? Mod => IsLoaded ? mod.Value : null;

        /// <summary>
        ///     The mod's assembly, if loaded.
        /// </summary>
        public static Assembly? Assembly => Mod?.Code;
    }

    public abstract class Calamity : BaseModCompatibility<Calamity.CalamityNameProvider>
    {
        public sealed class CalamityNameProvider : IModNameProvider
        {
            string IModNameProvider.ModName => "CalamityMod";
        }
    }
}
