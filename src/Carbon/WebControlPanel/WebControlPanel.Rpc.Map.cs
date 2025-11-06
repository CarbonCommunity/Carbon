using Facepunch;
using Facepunch.Math;

namespace Carbon;

public static partial class WebControlPanel
{
	public static MapInfo MAPINFO_CACHE;

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.MapView)]
	private static void RPC_LoadMapInfo(BridgeRead read)
	{
		if (!MAPINFO_CACHE.IsValid())
		{
			return;
		}
		var write = StartRpcResponse();
		MAPINFO_CACHE.Serialize(write);
		SendRpcResponse(read.Connection, write);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.MapEntities)]
	private static void RPC_RequestMapEntities(BridgeRead read)
	{
		if (!MAPINFO_CACHE.IsValid())
		{
			return;
		}
		var typeCount = read.Int32();
		using var entities = Pool.Get<PooledList<MapEntity>>();
		var allEntities = BaseNetworkable.serverEntities;
		var values = allEntities.entityList.Get().Values;
		var hasPlayerViewAccess = Account.HasPermission(read.Connection, PermissionTypes.PlayersView);
		for (int i = 0; i < typeCount; i++)
		{
			var type = (MapEntity.Types)read.Int32();
			for (int e = 0; e < values.Count; e++)
			{
				switch (type)
				{
					case MapEntity.Types.ActivePlayers:
					{
						if (hasPlayerViewAccess && values[e] is BasePlayer player && player.IsConnected && player.userID.IsSteamId())
						{
							entities.Add(new MapEntity(player, type));
						}
						break;
					}
					case MapEntity.Types.SleepingPlayers:
					{
						if (hasPlayerViewAccess && values[e] is BasePlayer player && !player.IsConnected && player.userID.IsSteamId())
						{
							entities.Add(new MapEntity(player, type));
						}
						break;
					}
				}
			}
		}

		var write = StartRpcResponse();
		write.WriteObject(entities.Count);
		for (int i = 0; i < entities.Count; i++)
		{
			entities[i].Serialize(write);
		}
		write.WriteObject(TOD_Sky.Instance.Cycle.Hour);
		SendRpcResponse(read.Connection, write);
	}

	public struct MapInfo
	{
		private int imageWidth;
		private int imageHeight;
		private byte[] imageData;
		private uint worldSize;

		public bool IsValid() => imageData != null;

		public static MapInfo Get()
		{
			MapInfo info = default;
			info.imageData = MapImageRenderer.Render(out var width, out var height, out _, scale: 1f, lossy: false, transparent: true, oceanMargin: 0);
			info.imageWidth = width;
			info.imageHeight = height;
			info.worldSize = World.Size;
			return info;
		}

		public void Serialize(BridgeWrite write)
		{
			write.WriteObject(imageWidth);
			write.WriteObject(imageHeight);
			write.WriteObject(imageData.Length);
			write.WriteObject(imageData);
			write.WriteObject(worldSize);

			using var monuments = Pool.Get<PooledList<MapMonument>>();
			for (int i = 0; i < TerrainMeta.Path.Monuments.Count; i++)
			{
				var info = new MapMonument(TerrainMeta.Path.Monuments[i]);
				if (!info.HasValidLabel())
				{
					continue;
				}
				monuments.Add(info);
			}
			write.WriteObject(monuments.Count);
			for (int i = 0; i < monuments.Count; i++)
			{
				monuments[i].Serialize(write);
			}
		}
	}

	public struct MapMonument(MonumentInfo monument)
	{
		private string label = monument.displayPhrase.english ?? GetCustomMarkerName(monument.gameObject);
		private Vector3 GetPosition() => TerrainMeta.Normalize(monument.transform.position);

		public bool HasValidLabel()
		{
			return label.Length > 2;
		}

		public void Serialize(BridgeWrite write)
		{
			var position = GetPosition();
			write.WriteObject(label);
			write.WriteObject(position.x);
			write.WriteObject(position.z);
		}

		private static string GetCustomMarkerName(GameObject go)
		{
			foreach (var prefab in World.SpawnedPrefabs)
			{
				if (prefab.Value != null && prefab.Value.Contains(go))
				{
					return prefab.Key;
				}
			}
			return go.name;
		}
	}

	public struct MapEntity(BaseEntity entity, MapEntity.Types type)
	{
		private Vector3 GetPosition() => TerrainMeta.Normalize(entity.transform.position);

		private string GetLabel()
		{
			switch (entity)
			{
				case BasePlayer player:
					return player.displayName;
			}

			return null;
		}

		public enum Types
		{
			ActivePlayers,
			SleepingPlayers
		}

		public void Serialize(BridgeWrite write)
		{
			var position = GetPosition();
			var label = GetLabel();
			write.WriteObject((int)type);
			write.WriteObject(entity.net.ID);
			write.WriteObject(!string.IsNullOrEmpty(label));
			if (!string.IsNullOrEmpty(label))
			{
				write.WriteObject(label);
			}
			write.WriteObject(position.x);
			write.WriteObject(position.z);
		}
	}
}
