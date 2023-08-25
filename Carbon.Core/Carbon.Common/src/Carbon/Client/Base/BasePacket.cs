using Carbon.Client.Contracts;
using Carbon.Client.Packets;
using Network;
using ProtoBuf;

namespace Carbon.Client;

[ProtoContract]
[ProtoInclude(10, typeof(RPCList))]
[ProtoInclude(11, typeof(ClientInfo))]
[ProtoInclude(12, typeof(ItemDefinitionUpdate))]
[ProtoInclude(13, typeof(ClientModifications))]
public class BasePacket : IPacket, IDisposable
{
	public static T Deserialize<T>(NetRead reader)
	{
		reader.TemporaryBytesWithSize(out byte[] buf, out int count);
		return Serializer.Deserialize<T>(new ReadOnlySpan<byte>(buf, 0, count));
	}

	public byte[] Serialize()
	{
		using var stream = new MemoryStream();
		Serializer.Serialize(stream, this);
		return stream.ToArray();
	}

	public virtual void Dispose()
	{
	}
}
