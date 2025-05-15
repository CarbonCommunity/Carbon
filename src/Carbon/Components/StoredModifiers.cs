using Facepunch;
using ProtoBuf;

namespace Carbon.Components;

public class StoredModifiers
{
	public static Dictionary<ulong, Data> Entities = [];

	public static string GetSavePath() => $"{World.SaveFolderName}/{Path.GetFileNameWithoutExtension(World.SaveFileName)}.carbon.sav";

	public static void TryUpdateData(BaseNetworkable entity, Data data, BaseNetworkable.SaveInfo info)
	{
		if (!info.forDisk)
		{
			return;
		}

		var id = entity.net.ID.Value;

		if (data == null)
		{
			Entities.Remove(id);
			return;
		}

		Entities[id] = data;
	}

	public static void TryGetData<T>(BaseNetworkable entity, ref T data, BaseNetworkable.LoadInfo info) where T : Data
	{
		if (!info.fromDisk)
		{
			return;
		}
		var id = entity.net.ID.Value;
		Entities.TryGetValue(id, out var entityData);
		Logger.Warn($"StoredModifiers.TryGetData {entity}[{id}] {info.fromDisk} | {entityData == null}");
		data = entityData as T;
	}

	public static void Init()
	{
		using var types = Pool.Get<PooledList<Type>>();
		types.AddRange(AccessToolsEx.AllTypes());
		foreach (var type in types)
		{
			if (type.GetNestedType("CarbonData") is Type carbonData)
			{
				carbonData.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Invoke(null, null);
				Logger.Warn($"Initialized {carbonData.FullName}");
			}
		}
	}

	public static void Load()
	{
		var savePath = GetSavePath();

		if (!File.Exists(savePath))
		{
			return;
		}

		using (var file = File.OpenRead(savePath))
		{
			using (TimeMeasure.New("StoredModifiers.Load", warn: $"loaded {Entities.Count:n0} entities"))
			{
				Entities = Serializer.Deserialize<Dictionary<ulong, Data>>(file);
				Logger.Log($"Processed {Entities.Count:n0} {Entities.Count.Plural("entity", "entities")} with Carbon modifier data");
			}
		}
	}

	public static void Save()
	{
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
				Logger.Log($"Saved {Entities.Count:n0} {Entities.Count.Plural("ent", "ents")} (skipped {deadIds.Count:n0}) with Carbon modifier data");
			}
		}
	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class Data;
}
