using ProtoBuf;
using ProtoBuf.Meta;

namespace Carbon.Client.Packets;

[ProtoContract]
public class ClientModifications : BasePacket
{
	[ProtoMember(1)]
	public List<Entry> Entries = new();

	public override void Dispose()
	{
		Entries.Clear();
		Entries = null;
	}

	[ProtoContract]
	public class Entry
	{
		public string Identifier;
		public string Path;
		public object Value;
		public Types Type;

		public enum Types
		{
			NONE = -1,
			Entity,
			ItemDefinition,
			Static
		}
	}
}
