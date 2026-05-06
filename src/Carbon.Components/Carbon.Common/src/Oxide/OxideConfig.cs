using Newtonsoft.Json;

namespace Oxide.Core.Configuration;

public class OxideConfig : ConfigFile
{
	public class OxideOptions
	{
		public bool Modded { get; set; } = true;
		public bool PluginWatchers { get; set; } = true;
		public DefaultGroups DefaultGroups { get; set; } = new DefaultGroups();
		public string WebRequestIP { get; set; } = "0.0.0.0";
	}

	public class CommandOptions
	{
		[JsonProperty(PropertyName = "Chat command prefixes")]
		public List<string> ChatPrefix { get; set; } = new List<string>() { "/" };
	}

	public class CompilerOptions
	{
		[JsonProperty(PropertyName = "Shutdown on idle")]
		public bool IdleShutdown { get; set; } = true;

		[JsonProperty(PropertyName = "Seconds before idle")]
		public int IdleTimeout { get; set; } = 60;

		[JsonProperty(PropertyName = "Preprocessor directives")]
		public List<string> PreprocessorDirectives { get; set; } = new List<string>();
	}

	[JsonObject]
	public class DefaultGroups : IEnumerable<string>
	{
		public string Players { get; set; } = "default";
		public string Administrators { get; set; } = "admin";

		public IEnumerator<string> GetEnumerator()
		{
			yield return Players;
			yield return Administrators;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public class OxideConsole
	{
		public bool Enabled { get; set; } = true;
		public bool MinimalistMode { get; set; } = true;
		public bool ShowStatusBar { get; set; } = true;
	}

	public class OxideRcon
	{
		public bool Enabled { get; set; } = false;
		public int Port { get; set; } = 25580;
		public string Password { get; set; } = string.Empty;
		public string ChatPrefix { get; set; } = "[Server Console]";
	}

	public OxideOptions Options { get; set; }

	[JsonProperty(PropertyName = "Commands")]
	public CommandOptions Commands { get; set; } = new();

	[JsonProperty(PropertyName = "Plugin Compiler")]
	public CompilerOptions Compiler { get; set; } = new();

	[JsonProperty(PropertyName = "OxideConsole")]
	public OxideConsole Console { get; set; } = new();

	[JsonProperty(PropertyName = "OxideRcon")]
	public OxideRcon Rcon { get; set; } = new();

	public OxideConfig(string filename) : base(filename)
	{
	}
}
