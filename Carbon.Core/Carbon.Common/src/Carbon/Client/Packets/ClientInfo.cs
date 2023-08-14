using ProtoBuf;

namespace Carbon.Client.Packets;

[ProtoContract]
public class ClientInfo : BasePacket
{
	[ProtoMember(1)]
	public int ScreenWidth{ get; set; }

	[ProtoMember(1)]
	public int ScreenHeight { get; set; }

	public override void Dispose()
	{

	}
}
