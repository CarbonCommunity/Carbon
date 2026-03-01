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

		public bool IsValid { get; internal set; }
		public readonly int PluginCount => IsValid ? Plugins.Count : default;

		public Package AddPlugin(RustPlugin plugin)
		{
			if (!IsValid || Plugins == null || Plugins.Contains(plugin))
			{
				return this;
			}

			Plugins.Add(plugin);
			return this;
		}
		public Package RemovePlugin(RustPlugin plugin)
		{
			if (!IsValid || Plugins == null || !Plugins.Contains(plugin))
			{
				return this;
			}

			Plugins.Remove(plugin);
			return this;
		}
		public RustPlugin FindPlugin(string name)
		{
			return Plugins?.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
		}

		public static Package Get(string name, bool isCoreMod, string file = null)
		{
			Package package = default;
			package.Name = name;
			package.File = file;
			package.IsCoreMod = isCoreMod;
			package.Plugins = new();
			package.IsValid = true;
			return package;
		}
	}
}
