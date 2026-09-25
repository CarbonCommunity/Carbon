using ProtoBuf;

namespace Carbon.Plugins;

/// <summary>
/// Persisted permission data of one group.
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class GroupData
{
	public string Title { get; set; } = string.Empty;

	public int Rank { get; set; }

	public HashSet<string> Perms { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	public string ParentGroup { get; set; } = string.Empty;
}
