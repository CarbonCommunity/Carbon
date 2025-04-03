using Newtonsoft.Json;

namespace Carbon.Documentation.Generators;

public static class HooksAIMetadata
{
	public static void Generate()
	{
		File.WriteAllText(Path.Combine(Generator.Arguments.Docs, "hooks", "hooks_metadata.json"), JsonConvert.SerializeObject(Hooks.Oxide.Concat(Hooks.Community).Concat(Hooks.Base).ToArray(), Formatting.Indented));
	}
}
