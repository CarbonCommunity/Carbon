using ProtoBuf;
using ProtoBuf.Meta;

namespace Carbon.Client.Packets;

[ProtoContract]
public class ClientModifications : BasePacket
{
	[ProtoMember(1)]
	public List<Entry> Entries { get; set; } = new();

	public override void Dispose()
	{
		Entries.Clear();
		Entries = null;
	}

	[ProtoContract]
	public class Entry
	{
		[ProtoMember(1)]
		public string Identifier { get; set; }

		[ProtoMember(2)]
		public string Path { get; set; }

		[ProtoMember(3)]
		public string Value { get; set; }

		[ProtoMember(4)]
		public string Component { get; set; }

		[ProtoMember(5)]
		public Types Type { get; set; }

		public enum Types
		{
			NONE = -1,
			Entity,
			ItemDefinition,
			Static,
			Prefab
		}
	}
}
