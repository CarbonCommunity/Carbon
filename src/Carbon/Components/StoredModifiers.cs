using System.Security.Cryptography;
using System.Text;
using Facepunch;
using ProtoBuf;

namespace Carbon.Components;

public sealed class StoredModifiers
{
	public static Dictionary<ulong, Dictionary<uint, Data>> Entities = [];

	public static string GetSavePath() => $"{World.SaveFolderName}/{Path.GetFileNameWithoutExtension(World.SaveFileName)}.carbon.sav";

	public static bool HasLocalSave() => File.Exists(GetSavePath());

	private static uint ManifestHash(string str)
	{
		return string.IsNullOrEmpty(str) ? 0 : BitConverter.ToUInt32(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(str)), 0);
	}

	public static void TryUpdateData<T>(BaseNetworkable entity, Data data, BaseNetworkable.SaveInfo info) where T : Data
	{
		if (!info.forDisk)
		{
			return;
		}

		var netId = entity.net.ID.Value;
		var id = Vault.Pool.Get(typeof(T).Name);
		Dictionary<uint, Data> dict;

		if (data == null)
		{
			if (Entities.TryGetValue(netId, out dict))
			{
				dict.Remove(id);

				if (dict.Count == 0)
				{
					Entities.Remove(netId);
				}
			}
			return;
		}

		if (!Entities.TryGetValue(netId, out dict))
		{
			Entities[netId] = (dict = new());
		}

		dict[id] = data;
	}

	public static void TryGetData<T>(BaseNetworkable entity, ref T data, BaseNetworkable.LoadInfo info) where T : Data
	{
		if (!info.fromDisk)
		{
			return;
		}

		var netId = entity.net.ID.Value;
		var id = Vault.Pool.Get(typeof(T).Name);

		if (Entities.TryGetValue(netId, out var dict) && dict.TryGetValue(id, out var entityData))
		{
			data = entityData as T;
		}
	}

	public static void Init()
	{
		var baseType = typeof(Data);
		foreach (var assembly in AccessToolsEx.AllAssemblies())
		{
			try
			{
				foreach (var type in assembly.GetExportedTypes())
				{
					try
					{
						if (type != baseType && baseType.IsAssignableFrom(type))
						{
							type.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Invoke(null, null);
						}
					}
					catch { } // we don't care
				}
			}
			catch { } // again, we don't care
		}
	}

	public static void Load()
	{
		if (!HasLocalSave())
		{
			return;
		}

		var savePath = GetSavePath();
		using (var file = File.OpenRead(savePath))
		{
			using (TimeMeasure.New("StoredModifiers.Load", warn: $"Carbon modifier entity data"))
			{
				Entities = Serializer.Deserialize<Dictionary<ulong, Dictionary<uint, Data>>>(file);
				Logger.Log($"Processed {Entities.Count:n0} {Entities.Count.Plural("entity", "entities")} with Carbon modifier data");
			}
		}
	}

	public static void Save()
	{
		if (Entities.Count == 0 && !HasLocalSave())
		{
			return;
		}

		var savePath = GetSavePath();

		using (TimeMeasure.New("StoredModifiers.Save", warn: $"saved {Entities.Count:n0} entities"))
		{
			using var deadIds = Pool.Get<PooledList<ulong>>();
			foreach (var ent in Entities)
			{
				if (!BaseNetworkable.serverEntities.Find(new NetworkableId(ent.Key)))
				{
					deadIds.Add(ent.Key);
				}
			}
			for (int i = 0; i < deadIds.Count; i++)
			{
				Entities.Remove(deadIds[i]);
			}

			using (var file = File.OpenWrite(savePath))
			{
				Serializer.Serialize(file, Entities);
				Logger.Log($"Saved {Entities.Count:n0} {Entities.Count.Plural("ent", "ents")} with Carbon modifier data");
			}
		}
	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class Data;
}
