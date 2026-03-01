using System.Reflection;
using System.Runtime.CompilerServices;
using Carbon.Core;
using JetBrains.Annotations;
using Oxide.Core.Extensions;
using Oxide.Core.Plugins;
using Oxide.Plugins;
using Timer = Oxide.Plugins.Timer;

namespace Carbon.Compat.Lib;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static partial class OxideCompat
{
	internal const string LEGACY_MSG = "Used for oxide backwards compatibility";

    internal static Dictionary<Assembly, ModLoader.Package> modPackages = new();

    public static void RegisterPluginLoader(ExtensionManager self, PluginLoader loader, Extension oxideExt)
    {
        self.RegisterPluginLoader(loader);

        string asmName = Assembly.GetCallingAssembly().GetName().Name;

        Assembly asm = oxideExt != null ? oxideExt.GetType().Assembly : loader.GetType().Assembly;
        string name = oxideExt != null ? oxideExt.Name : asm.GetName().Name;
        string author = oxideExt != null ? oxideExt.Author : "Carbon.Compat";

        if (!modPackages.TryGetValue(asm, out ModLoader.Package package))
        {
	        ModLoader.RegisterPackage(package = modPackages[asm] =
		        ModLoader.Package.Get($"{name} - {author} (Oxide Extension)", false));
        }
        foreach (Type type in loader.CorePlugins)
        {
	        if (type.IsAbstract)
	        {
		        continue;
	        }

            try
            {
                ModLoader.InitializePlugin(type, out RustPlugin plugin, package, precompiled: true, preInit: oxideExt == null ? null :
                    rustPlugin =>
                    {
                        rustPlugin.Version = oxideExt.Version;
                        if (rustPlugin.Author == "Carbon.Compat" && !string.IsNullOrWhiteSpace(oxideExt.Author))
                            rustPlugin.Author = oxideExt.Author;
                    });
				plugin.IsCorePlugin = true;
                plugin.IsExtension = true;
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to load plugin {type.Name} in oxide extension {asmName}: {e}");
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetMessage1(global::Oxide.Core.Libraries.Lang lang, string key, Plugin plugin, string player)
    {
	    return lang.GetMessage(key, plugin, player);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddConsoleCommand1(global::Oxide.Game.Rust.Libraries.Command cmd, string name, Plugin plugin, Func<ConsoleSystem.Arg, bool> callback)
    {
        cmd.AddConsoleCommand(name, plugin, callback);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddChatCommand1(global::Oxide.Game.Rust.Libraries.Command cmd, string name, Plugin plugin, Action<BasePlayer, string, string[]> callback)
    {
        cmd.AddChatCommand(name, plugin, callback);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T OxideCallHookGeneric<T>(string hook, params object[] args)
    {
        return (T)global::Oxide.Core.Interface.Call<T>(hook, args);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetExtensionDirectory(global::Oxide.Core.OxideMod _)
    {
        return Defines.GetExtensionsFolder();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Timer TimerOnce(Timers timers, float delay, Action callback, Plugin owner = null)
    {
        return timers.Once(delay, callback);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Timer TimerRepeat(Timers timers, float delay, int reps, Action callback, Plugin owner = null)
    {
        return timers.Repeat(delay, reps, callback);
    }
}
