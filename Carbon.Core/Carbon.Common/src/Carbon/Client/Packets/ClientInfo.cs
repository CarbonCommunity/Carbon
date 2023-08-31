using ProtoBuf;
using ProtoBuf.Meta;

namespace Carbon.Client.Packets;

[ProtoContract]
public class ClientInfo : BasePacket
{
	[ProtoMember(1)]
	public int ScreenWidth { get; set; }

	[ProtoMember(2)]
	public int ScreenHeight { get; set; }

	public override void Dispose()
	{

	}
}
