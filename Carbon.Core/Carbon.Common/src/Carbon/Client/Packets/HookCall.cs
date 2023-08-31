using ProtoBuf;
using ProtoBuf.Meta;

namespace Carbon.Client.Packets;

[ProtoContract]
public class HookCall : BasePacket
{
	[ProtoMember(1)]
	public string Hook { get; set; }

	public override void Dispose()
	{

	}
}
