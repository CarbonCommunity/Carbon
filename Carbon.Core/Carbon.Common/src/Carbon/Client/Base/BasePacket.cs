using Carbon.Client.Contracts;
using Carbon.Client.Packets;
using Network;
using ProtoBuf;

namespace Carbon.Client;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[ProtoInclude(100, typeof(ServerRPCList))]
public class BasePacket : IPacket, IDisposable
{
	public static T Deserialize<T>(NetRead reader)
	{
		reader.TemporaryBytesWithSize(out byte[] buf, out int count);
		return Serializer.Deserialize<T>(new ReadOnlySpan<byte>(buf, 0, count));
	}

	public void Serialize(NetWrite writer)
	{
		using var stream = new MemoryStream();
		Serializer.Serialize(stream, this);
		writer.BytesWithSize(stream);
	}

	public virtual void Dispose()
	{
	}
}
