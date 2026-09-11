using Newtonsoft.Json;

namespace Carbon.Core;

public static partial class ModLoader
{
	[JsonObject(MemberSerialization.OptIn)]
	public struct Trace
	{
		[JsonProperty] public string Number;
		[JsonProperty] public string Message;
		[JsonProperty] public int Column;
		[JsonProperty] public int Line;
	}
}
