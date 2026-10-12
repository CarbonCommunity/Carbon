using System;
using System.IO;
using System.Reflection;
using HarmonyLib;

namespace Carbon;

/// <summary>
/// Reaches Oxide through Harmony's AccessTools, so the profiler has no compile-time dependency on it.
/// </summary>
internal static class OxidePlugins
{
	private static readonly PropertyInfo Oxide = AccessTools.Property("Oxide.Core.Interface, Oxide.Core:Oxide");
	private static readonly PropertyInfo RootPluginManager = AccessTools.Property("Oxide.Core.OxideMod, Oxide.Core:RootPluginManager");
	private static readonly PropertyInfo PluginDirectory = AccessTools.Property("Oxide.Core.OxideMod, Oxide.Core:PluginDirectory");
	private static readonly MethodInfo ReloadPlugin = AccessTools.Method("Oxide.Core.OxideMod, Oxide.Core:ReloadPlugin", [typeof(string)]);
	private static readonly MethodInfo GetPlugin = AccessTools.Method("Oxide.Core.Plugins.PluginManager, Oxide.Core:GetPlugin", [typeof(string)]);
	private static readonly PropertyInfo Filename = AccessTools.Property("Oxide.Core.Plugins.Plugin, Oxide.Core:Filename");
	private static readonly MethodInfo GetCompilablePlugin = AccessTools.Method("Oxide.Plugins.CSharpPluginLoader, Oxide.CSharp:GetCompilablePlugin");
	private static readonly FieldInfo LastModifiedAt = AccessTools.Field("Oxide.Plugins.CompilableFile, Oxide.CSharp:LastModifiedAt");

	public static string FindFileName(string name)
	{
		var manager = RootPluginManager.GetValue(Oxide.GetValue(null));
		var plugin = GetPlugin.Invoke(manager, [name]);
		var file = plugin == null ? null : Filename.GetValue(plugin) as string;

		return string.IsNullOrEmpty(file) ? null : Path.GetFileNameWithoutExtension(file);
	}

	/// <summary>
	/// Reloads the plugin and forces Oxide to compile it again. Oxide otherwise reuses the loaded
	/// assembly when the file is unchanged, and Mono would not compile its methods again.
	/// </summary>
	public static bool Recompile(string name)
	{
		var oxide = Oxide.GetValue(null);
		var compilablePlugin = GetCompilablePlugin.Invoke(null, [PluginDirectory.GetValue(oxide), name]);

		LastModifiedAt.SetValue(compilablePlugin, default(DateTime));
		return ReloadPlugin.Invoke(oxide, [name]) is true;
	}
}
