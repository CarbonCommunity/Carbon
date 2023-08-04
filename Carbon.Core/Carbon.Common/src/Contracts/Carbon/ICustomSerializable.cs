namespace Carbon.Contracts;

public interface ICustomSerializable : IDisposable
{
	public void Setup(BaseEntity entity, bool justCreated);
	public bool CanSave { get; }
	public byte[] Serialize();
	public void Deserialize(byte[] data);
}
