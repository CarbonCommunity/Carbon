using Newtonsoft.Json;

namespace Carbon.Core;

public static partial class ModLoader
{
	[JsonObject(MemberSerialization.OptIn)]
	public struct Package
	{
		public Assembly Assembly;
		public Type[] AllTypes;

		[JsonProperty] public string Name;
		[JsonProperty] public string File;
		[JsonProperty] public bool IsCoreMod;
		[JsonProperty] public List<RustPlugin> Plugins;

		public Dictionary<string, RustPlugin> Index;

		public bool IsValid { get; internal set; }
		public readonly int PluginCount => IsValid ? Plugins.Count : default;

		public Package AddPlugin(RustPlugin plugin)
		{
			if (!IsValid || Plugins == null || Plugins.Contains(plugin))
			{
				return this;
			}

			Plugins.Add(plugin);
			if (Index != null && plugin.Name != null) Index[plugin.Name] = plugin;
			return this;
		}
		public Package RemovePlugin(RustPlugin plugin)
		{
			if (!IsValid || Plugins == null || !Plugins.Contains(plugin))
			{
				return this;
			}

			Plugins.Remove(plugin);
			if (Index != null && plugin.Name != null && Index.TryGetValue(plugin.Name, out var indexed) && ReferenceEquals(indexed, plugin))
			{
				Index.Remove(plugin.Name);
			}
			return this;
		}
		public RustPlugin FindPlugin(string name)
		{
			if (string.IsNullOrEmpty(name) || Index == null) return null;
			return Index.TryGetValue(name, out var plugin) ? plugin : null;
		}

		public static Package Get(string name, bool isCoreMod, string file = null)
		{
			Package package = default;
			package.Name = name;
			package.File = file;
			package.IsCoreMod = isCoreMod;
			package.Plugins = new();
			package.Index = new(StringComparer.OrdinalIgnoreCase);
			package.IsValid = true;
			return package;
		}
	}
}
