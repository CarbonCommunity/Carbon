using ProtoBuf;
using ProtoBuf.Meta;

namespace Carbon.Client.Packets;

[ProtoContract]
public class RPCList : BasePacket
{
	// static RPCList() => RuntimeTypeModel.Default[typeof(BasePacket)].AddSubType(101, typeof(RPCList));

	[ProtoMember(1)]
	public string[] RpcNames;

	[ProtoMember(2)]
	public uint[] RpcIds;

	public static RPCList Get()
	{
		return new RPCList()
		{
			RpcIds = RPC.rpcList.Select(x => x.Id).ToArray(),
			RpcNames = RPC.rpcList.Select(x => x.Name).ToArray()
		};
	}

	public void Sync()
	{
		foreach (var item in RpcNames)
		{
			RPC.Get(item);
		}
		foreach (var item in RpcIds)
		{
			RPC.Get(item);
		}

		foreach(var item in RPC.rpcList)
		{
			Console.WriteLine($"Registered RPC: {item.Name}[{item.Id}]");
		}
	}

	public override void Dispose()
	{
		Array.Clear(RpcNames, 0, RpcNames.Length);
		Array.Clear(RpcIds, 0, RpcIds.Length);
	}
}
