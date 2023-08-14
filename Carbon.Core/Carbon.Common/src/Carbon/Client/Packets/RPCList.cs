using ProtoBuf;

namespace Carbon.Client.Packets;

[ProtoContract]
public class RPCList : BasePacket
{
	[ProtoMember(1)]
	public string[] RpcNames { get; set; }

	[ProtoMember(2)]
	public uint[] RpcIds { get; set; }

	public static RPCList Get()
	{
		return new RPCList()
		{
			RpcIds = RPC.rpcList.Select(x => x.Id).ToArray(),
			RpcNames = RPC.rpcList.Select(x => x.Name).ToArray()
		};
	}

	public override void Dispose()
	{
		Array.Clear(RpcNames, 0, RpcNames.Length);
		Array.Clear(RpcIds, 0, RpcIds.Length);
	}
}
