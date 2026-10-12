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
	/// Makes Oxide compile the plugin again the next time it loads. Oxide otherwise reuses the loaded
	/// assembly when the file is unchanged, and Mono would not compile its methods again.
	/// </summary>
	public static void MarkForRecompile(string name)
	{
		var compilablePlugin = GetCompilablePlugin.Invoke(null, [PluginDirectory.GetValue(Oxide.GetValue(null)), name]);
		LastModifiedAt.SetValue(compilablePlugin, default(DateTime));
	}

	public static bool Recompile(string name)
	{
		MarkForRecompile(name);
		return ReloadPlugin.Invoke(Oxide.GetValue(null), [name]) is true;
	}
}
