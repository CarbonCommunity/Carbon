namespace Oxide.Compatibility;

/// <summary>
/// Oxide types that Carbon provides natively under a different name. Used to translate
/// plugin sources (<see cref="OxideSourceTranslator"/>) and precompiled assemblies (compat type remapping).
/// Anything under an Oxide namespace that isn't listed here is a real type in this package.
/// </summary>
public static class OxideTypeMap
{
	private static readonly string[] CuiTypes =
	[
		"ComponentConverter", "CuiButtonComponent", "CuiElementContainer", "CuiImageComponent", "CuiInputFieldComponent",
		"CuiNeedsCursorComponent", "CuiNeedsKeyboardComponent", "CuiOutlineComponent", "CuiRawImageComponent",
		"CuiRectTransformComponent", "CuiRectTransform", "CuiCountdownComponent", "CuiCanvasGroupComponent",
		"CuiMaskComponent", "CuiTooltipComponent", "TimerFormat", "CuiTextComponent", "CuiScrollViewComponent",
		"CuiScrollbar", "CuiLayoutGroupComponent", "CuiHorizontalLayoutGroupComponent", "CuiVerticalLayoutGroupComponent",
		"CuiGridLayoutGroupComponent", "CuiContentSizeFitterComponent", "CuiLayoutElementComponent", "CuiDraggableComponent",
		"CuiSlotComponent", "CuiButton", "CuiElement", "ICuiGraphic", "ICuiEnableable", "CuiLabel", "CuiPanel",
		"CuiHelper", "ICuiComponent", "ICuiColor"
	];

	/// <summary>Oxide full type name -> native full type name.</summary>
	public static IReadOnlyDictionary<string, string> Types { get; } = Build();

	/// <summary>Native types living in the SDK rather than Carbon.Common.</summary>
	public static IReadOnlyCollection<string> SdkTypes { get; } = new HashSet<string> { "Carbon.VersionNumber" };

	private static Dictionary<string, string> Build()
	{
		var map = new Dictionary<string, string>
		{
			["Oxide.Core.VersionNumber"] = "Carbon.VersionNumber",
			["Oxide.Core.Plugins.Plugin"] = "Carbon.Plugins.Plugin",
			["Oxide.Core.Plugins.AutoPatchAttribute"] = "Carbon.Plugins.AutoPatchAttribute",
			["Oxide.Core.Libraries.Library"] = "Carbon.Plugins.Library",
			["Oxide.Core.Libraries.Permission"] = "Carbon.Plugins.Permission",
			["Oxide.Core.Libraries.UserData"] = "Carbon.Plugins.UserData",
			["Oxide.Core.Libraries.GroupData"] = "Carbon.Plugins.GroupData",
			["Oxide.Core.Libraries.Lang"] = "Carbon.Plugins.Lang",
			["Oxide.Core.Libraries.Timer"] = "Carbon.Plugins.TimerLibrary",
			["Oxide.Core.Libraries.WebRequests"] = "Carbon.Plugins.WebRequests",
			["Oxide.Core.Libraries.RequestMethod"] = "Carbon.Plugins.RequestMethod",
			["Oxide.Plugins.Timer"] = "Carbon.Plugins.Timer",
			["Oxide.Plugins.PluginTimers"] = "Carbon.Plugins.PluginTimers",
			["Oxide.Game.Rust.Libraries.Command"] = "Carbon.Plugins.CommandLibrary",
			["Oxide.Core.DataFileSystem"] = "Carbon.Plugins.DataFileSystem",
			["Oxide.Core.ProtoStorage"] = "Carbon.Plugins.ProtoStorage",
			["Oxide.Core.Configuration.ConfigFile"] = "Carbon.Plugins.ConfigFile",
			["Oxide.Core.Configuration.DynamicConfigFile"] = "Carbon.Plugins.DynamicConfigFile",
			["Oxide.Core.Configuration.KeyValuesConverter"] = "Carbon.Plugins.KeyValuesConverter",
		};

		foreach (var type in CuiTypes)
		{
			map[$"Oxide.Game.Rust.Cui.{type}"] = $"Carbon.Components.{type}";
		}

		return map;
	}

	/// <summary>
	/// Maps a full type name (nested types included, e.g. "Oxide.Core.Libraries.Timer.TimerInstance").
	/// Returns null when the type isn't renamed.
	/// </summary>
	public static string Map(string fullName)
	{
		if (string.IsNullOrEmpty(fullName))
		{
			return null;
		}

		if (Types.TryGetValue(fullName, out var mapped))
		{
			return mapped;
		}

		// Nested types follow their declaring type
		for (var index = fullName.LastIndexOf('.'); index > 0; index = fullName.LastIndexOf('.', index - 1))
		{
			if (Types.TryGetValue(fullName.Substring(0, index), out var parent))
			{
				return parent + fullName.Substring(index);
			}
		}

		return null;
	}

	/// <summary>Renamed types grouped by the Oxide namespace they came from, as short name -> native full name.</summary>
	public static IReadOnlyDictionary<string, Dictionary<string, string>> ByNamespace { get; } = Types
		.GroupBy(x => x.Key.Substring(0, x.Key.LastIndexOf('.')))
		.ToDictionary(x => x.Key, x => x.ToDictionary(y => y.Key.Substring(y.Key.LastIndexOf('.') + 1), y => y.Value));
}
