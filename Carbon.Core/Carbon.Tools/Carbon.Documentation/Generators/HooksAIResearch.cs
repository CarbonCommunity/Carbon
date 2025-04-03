using Newtonsoft.Json;

namespace Carbon.Documentation.Generators;

public static class HooksAIResearch
{
	public static List<Hook>? hooks;

	public static void LoadResearch()
	{
		var research = Path.Combine(Generator.Arguments.Docs, "hooks", "hooks_research.json");
		hooks = JsonConvert.DeserializeObject<List<Hook>>(File.ReadAllText(research));
		Console.WriteLine($"Loaded {hooks.Count:n0} researched hooks");
	}

	public struct Hook
	{
		public string hook;
		public string[] descriptions;
	}
}
