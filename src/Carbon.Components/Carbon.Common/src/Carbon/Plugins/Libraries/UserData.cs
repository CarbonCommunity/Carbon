using ProtoBuf;

namespace Carbon.Plugins;

/// <summary>
/// Persisted permission data of one user.
/// </summary>
[ProtoContract]
public class UserData
{
	[ProtoMember(2)]
	public string LastSeenNickname { get; set; } = "Unnamed";

	[ProtoMember(3)]
	public HashSet<string> Perms { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	[ProtoMember(1)]
	public HashSet<string> Groups { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	[ProtoMember(50)]
	public string Language { get; set; } = "en";
}
