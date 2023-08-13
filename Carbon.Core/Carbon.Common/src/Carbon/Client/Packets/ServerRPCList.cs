namespace Carbon.Client.Packets;

public class ServerRPCList : BasePacket
{
	public string[] RpcNames;
	public uint[] RpcIds;

	public override void Dispose()
	{
		Array.Clear(RpcNames, 0, RpcNames.Length);
		Array.Clear(RpcIds, 0, RpcIds.Length);
	}
}
