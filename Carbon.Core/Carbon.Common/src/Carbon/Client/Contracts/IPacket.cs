namespace Carbon.Client.Contracts;

public interface IPacket
{
	void Serialize(Network.NetWrite writer);
}
