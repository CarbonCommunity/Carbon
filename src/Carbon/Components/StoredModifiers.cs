using ProtoBuf;

namespace Carbon.Components;

public class StoredModifiers
{
	public static Dictionary<ulong, object> Entities = new();

	public static string GetSavePath() => $"{World.SaveFolderName}/{Path.GetFileNameWithoutExtension(World.SaveFileName)}.carbon.sav";

	public static void Load()
	{
		var savePath = GetSavePath();

		if (!File.Exists(savePath))
		{
			return;
		}

		using (TimeMeasure.New("StoredModifiers.Load"))
		{
			using (var file = File.OpenRead(savePath))
			{
				Entities = Serializer.Deserialize<Dictionary<ulong, object>>(file);

				foreach (var data in Entities)
				{
					var entity = BaseNetworkable.serverEntities.Find(new NetworkableId(data.Key));
				}
			}
		}
	}

	public static void Save()
	{
		using (TimeMeasure.New("StoredModifiers.Save", warn: $"saved {Entities.Count:n0} entities"))
		{

		}
	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class Data
	{

	}
}
