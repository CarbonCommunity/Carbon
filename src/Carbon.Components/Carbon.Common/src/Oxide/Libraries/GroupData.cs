using ProtoBuf;

namespace Oxide.Core.Libraries;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class GroupData
{
	public string Title { get; set; } = string.Empty;

	public int Rank { get; set; }

	public HashSet<string> Perms { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	public string ParentGroup { get; set; } = string.Empty;
}
