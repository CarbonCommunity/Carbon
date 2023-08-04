using Facepunch;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Carbon.Components;

public abstract class CustomProtoBuf<T> : ICustomSerializable where T : CustomProtoBuf<T>
{
	public virtual void Setup(BaseEntity entity, bool justCreated) {}

	public virtual bool CanSave => true;

	public virtual byte[] Serialize()
	{
		using (MemoryStream ms = new MemoryStream())
		{
			Serializer.Serialize(ms, (T)this);
			return ms.ToArray();
		}
	}

	public virtual void Deserialize(byte[] data)
	{
		RuntimeTypeModel.Default.Deserialize(new ReadOnlySpan<byte>(data), (T)this);
	}

	public virtual void Dispose() {}
}

public static class CustomDataInternals
{
	public const byte MagicNumber = 195;

	[Obsolete("Internal use only")]
	public static void DeserializeAdditionalEntityData(ProtoBuf.Entity proto, ref Dictionary<string, ICustomSerializable> cache, ref Dictionary<string, byte[]> raw)
	{
		if (proto.associatedFiles?.files == null || proto.associatedFiles.files.Count == 0) return;
		List<AssociatedFiles.AssociatedFile> files = proto.associatedFiles.files;
		for (int index = 0; index < files.Count; index++)
		{
			AssociatedFiles.AssociatedFile entry = files[index];
			if (entry.type == MagicNumber)
			{
				try
				{
					raw = Serializer.Deserialize<Dictionary<string, byte[]>>(new ReadOnlySpan<byte>(entry.data));
					if (raw.Count == 0)
					{
						Logger.Warn("Additional data is empty??");
						raw = null;
					}
					else cache = new Dictionary<string, ICustomSerializable>();
				}
				catch (Exception e)
				{
					Logger.Error("Failed to deserialize additional data", e);
				}

				files.RemoveAt(index);
				entry.Dispose();
				break;
			}
		}

		if (files.Count == 0)
		{
			proto.associatedFiles.Dispose();
			proto.associatedFiles = null;
		}
	}

	[Obsolete("Internal use only")]
	public static void SerializeAdditionalEntityData(BaseNetworkable.SaveInfo info, ref Dictionary<string, ICustomSerializable> cache, ref Dictionary<string, byte[]> raw)
	{
		if (!info.forDisk || (cache == null || cache.Count == 0) && (raw == null || raw.Count == 0)) return;

		raw ??= new Dictionary<string, byte[]>();
		info.msg.associatedFiles ??= Pool.Get<AssociatedFiles>();
		List<AssociatedFiles.AssociatedFile> files = info.msg.associatedFiles.files ??=
			Pool.GetList<AssociatedFiles.AssociatedFile>();

		if (cache != null)
			foreach (KeyValuePair<string, ICustomSerializable> kv in cache)
			{
				if (kv.Value == null || kv.Key == null) continue;
				try
				{
					if (kv.Value.CanSave)
						raw[kv.Key] = kv.Value.Serialize();
					else
						raw.Remove(kv.Key);
				}
				catch (Exception e)
				{
					Logger.Error($"Failed to serialize {kv.Key} - {kv.Value.GetType().FullName}");
				}
			}

		AssociatedFiles.AssociatedFile inst = Pool.Get<AssociatedFiles.AssociatedFile>();
		inst.type = MagicNumber;
		MemoryStream ms = Pool.Get<MemoryStream>();
		{
			Serializer.Serialize(ms, raw);

			inst.data = ms.ToArray();
		}
		Pool.FreeMemoryStream(ref ms);

		files.Add(inst);
	}

    [Obsolete("Use BaseEntity.HasAnyAdditionalData")]
    public static bool HasAnyAdditionalData(ref Dictionary<string, ICustomSerializable> cache, ref Dictionary<string, byte[]> raw)
    {
        return raw?.Count > 0 || cache?.Count > 0;
    }

    [Obsolete("Use BaseEntity.HasAdditionalData")]
    public static bool HasAdditionalData(string name, ref Dictionary<string, ICustomSerializable> cache, ref Dictionary<string, byte[]> raw)
    {
        return (raw != null && raw.ContainsKey(name)) || (cache != null && cache.ContainsKey(name));
    }

    [Obsolete("Use BaseEntity.GetAdditionalData")]
    public static T GetAdditionalData<T>(string name, BaseEntity entity, ref Dictionary<string, ICustomSerializable> cache, ref Dictionary<string, byte[]> raw) where T : class, ICustomSerializable, new()
    {
	    ICustomSerializable instance = null;
        if (cache != null && cache.TryGetValue(name, out instance) && instance is T generic)
        {
            return generic;
        }

        instance?.Dispose();

        if (raw != null && raw.TryGetValue(name, out byte[] arr))
        {
            T generic2 = InitInstance<T>();
            generic2.Deserialize(arr);
            generic2.Setup(entity, false);
            cache ??= new();
            cache[name] = generic2;
            return generic2;
        }

        return null;
    }

    [Obsolete("Use BaseEntity.TryGetAdditionalData")]
    public static bool TryGetAdditionalData<T>(string name, BaseEntity entity, out T ret, ref Dictionary<string, ICustomSerializable> cache, ref Dictionary<string, byte[]> raw) where T : class, ICustomSerializable, new()
    {
	    ICustomSerializable instance = null;
        if (cache != null && cache.TryGetValue(name, out instance) && instance is T generic)
        {
            ret = generic;
            return true;
        }

        instance?.Dispose();

        if (raw != null && raw.TryGetValue(name, out byte[] arr))
        {
            T generic2 = InitInstance<T>();
            generic2.Deserialize(arr);
            generic2.Setup(entity, false);
            cache ??= new();
            cache[name] = generic2;
            ret = generic2;
            return true;
        }

        ret = null;
        return false;
    }

    [Obsolete("Use BaseEntity.GetOrCreateAdditionalData")]
    public static T GetOrCreateAdditionalData<T>(string name, BaseEntity entity, ref Dictionary<string, ICustomSerializable> cache, ref Dictionary<string, byte[]> raw) where T : class, ICustomSerializable, new()
    {
        if (TryGetAdditionalData(name, entity, out T ret, ref cache, ref raw))
        {
            return ret;
        }

        ret = InitInstance<T>();
        ret.Setup(entity, true);
        cache ??= new();
        cache[name] = ret;
        return ret;
    }

    public static T InitInstance<T>()  where T : class, ICustomSerializable, new()
    {
        return typeof(Pool.IPooled).IsAssignableFrom(typeof(T)) ? Pool.Get<T>() : new T();
    }

    [Obsolete("Use BaseEntity.AddAdditionalData")]
    public static void AddAdditionalData(string name, BaseEntity entity, ICustomSerializable instance, ref Dictionary<string, ICustomSerializable> cache)
    {
        cache ??= new();
        cache[name] = instance;
        instance.Setup(entity, true);
    }

    [Obsolete("Use BaseEntity.DeleteAdditionalData")]
    public static bool DeleteAdditionalData(string name, ref Dictionary<string, ICustomSerializable> cache, ref Dictionary<string, byte[]> raw)
    {
        bool ret = false;
        if (cache != null && cache.TryGetValue(name, out ICustomSerializable inst))
        {
            inst.Dispose();
            cache.Remove(name);
            ret = true;
        }
        if (raw != null && raw.Remove(name)) ret = true;
        return ret;
    }

    [Obsolete("Use BaseEntity.ClearAdditionalData")]
    public static void ClearAdditionalData(ref Dictionary<string, ICustomSerializable> cache, ref Dictionary<string, byte[]> raw)
    {
        if (cache != null)
        {
            foreach (ICustomSerializable inst in cache.Values)
            {
                inst.Dispose();
            }
            cache.Clear();
        }

        raw?.Clear();
    }
}
