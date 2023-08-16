using ProtoBuf;
using ProtoBuf.Meta;

namespace Carbon.Client.Packets;

[ProtoContract]
public class ItemDefinitionUpdate : BasePacket
{
	// static ItemDefinitionUpdate() => RuntimeTypeModel.Default[typeof(BasePacket)].AddSubType(102, typeof(ItemDefinitionUpdate));

	[ProtoMember(1)]
	public string Shortname { get; set; }

	[ProtoMember(2)]
	public string DisplayName { get; set; }

	[ProtoMember(3)]
	public string DisplayDescription{ get; set; }

	public void Apply()
	{
		var item = ItemManager.FindItemDefinition(Shortname);

		if (item == null)
		{
			return;
		}

		item.displayName.english = DisplayName;
		item.displayDescription.english = DisplayDescription;
	}

	public override void Dispose()
	{

	}
}
