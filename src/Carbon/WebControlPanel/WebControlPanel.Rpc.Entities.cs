using Facepunch;
using Newtonsoft.Json.Linq;

namespace Carbon;

public static partial class WebControlPanel
{
	public struct EntitySearchRange
	{
		public Vector3 position;
		public float range;
		public string filter;

		public readonly bool isValid => position != Vector3.zero && range > 0;

		public static EntitySearchRange Parse(string value)
		{
			if (!value.Contains(":"))
			{
				return default;
			}

			var split = value.Split(':');
			var coordinates = split[0].Split(' ');
			EntitySearchRange range = default;
			range.position = new Vector3(float.Parse(coordinates[0]), float.Parse(coordinates[1]),
				float.Parse(coordinates[2]));
			range.range = float.Parse(split[1]);
			range.filter = split.Length >= 3 ? split[2] : null;
			return range;
		}
	}

	[Rpc]
	private static Response SearchEntities(ConsoleSystem.Arg arg)
	{
		var maxCount = arg.GetInt(1, 200);
		var filter = arg.GetString(2);

		if (string.IsNullOrEmpty(filter))
		{
			return GetResponse();
		}

		var range = EntitySearchRange.Parse(filter);

		const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;

		using var entities = Pool.Get<PooledList<BaseEntity>>();

		switch (range.isValid)
		{
			case true:
			{
				Vis.Entities(range.position, range.range, entities, ~0, QueryTriggerInteraction.Ignore);
				for (int i = entities.Count - 1; i >= 0; i--)
				{
					var entity = entities[i];
					if (entities.Count > maxCount || (!string.IsNullOrEmpty(range.filter) &&
					                                  !(entity.GetType().Name.Contains(range.filter, comparison) ||
					                                    entity.PrefabName.Contains(range.filter, comparison) ||
					                                    entity.net.ID.Value.ToString().Contains(range.filter, comparison))))
					{
						entities.RemoveAt(i);
					}
				}

				break;
			}
			case false:
			{
				foreach (var entity in BaseNetworkable.serverEntities.OfType<BaseEntity>())
				{
					if (!entity.IsValid() || entity.IsDestroyed)
					{
						continue;
					}

					if (entity.GetType().Name.Contains(filter, comparison) ||
					    entity.PrefabName.Contains(filter, comparison) ||
					    entity.net.ID.Value.ToString().Contains(filter, comparison))
					{
						entities.Add(entity);
					}

					if (entities.Count >= maxCount)
					{
						break;
					}
				}

				break;
			}
		}

		return GetResponse(entities.Select(x => ParseEntityMetadata(x)).ToArray());
	}

	[Rpc]
	private static Response EntityDetails(ConsoleSystem.Arg arg)
	{
		var entity = BaseNetworkable.serverEntities.Find(new NetworkableId(arg.GetULong(1))) as BaseEntity;
		if (!entity.IsValid() || entity.IsDestroyed)
		{
			return GetResponse();
		}

		return GetResponse(ParseEntityDetails(entity));
	}

	[Rpc]
	private static Response EntitySave(ConsoleSystem.Arg arg)
	{
		var details = JObject.Parse(arg.GetString(1));
		var entity = BaseNetworkable.serverEntities.Find(new NetworkableId(details["NetId"].ToObject<ulong>())) as BaseEntity;
		if (!entity.IsValid() || entity.IsDestroyed)
		{
			return GetResponse();
		}

		entity.OwnerID = details["Owner"].ToObject<ulong>();
		entity.skinID = details["Skin"].ToObject<ulong>();
		entity.ServerPosition = new Vector3(details["PosX"].ToObject<float>(), details["PosY"].ToObject<float>(), details["PosZ"].ToObject<float>());
		entity.ServerRotation = Quaternion.Euler(new Vector3(details["RotX"].ToObject<float>(), details["RotY"].ToObject<float>(), details["RotZ"].ToObject<float>()));

		if (entity is BasePlayer playerEntity)
		{
			var playerData = details["PlayerEntity"];
			playerEntity.metabolism.hydration.max = playerData["MaxThirst"].ToObject<float>();
			playerEntity.metabolism.hydration.value = playerData["Thirst"].ToObject<float>();
			playerEntity.metabolism.calories.max = playerData["MaxHunger"].ToObject<float>();
			playerEntity.metabolism.calories.value = playerData["Hunger"].ToObject<float>();
			playerEntity.metabolism.radiation_level.max = playerData["MaxRads"].ToObject<float>();
			playerEntity.metabolism.radiation_level.value = playerData["Rads"].ToObject<float>();
			playerEntity.metabolism.bleeding.max = playerData["MaxBleed"].ToObject<float>();
			playerEntity.metabolism.bleeding.value = playerData["Bleed"].ToObject<float>();
		}

		if (entity is BaseCombatEntity combatEntity)
		{
			var combatData = details["CombatEntity"];
			combatEntity.SetMaxHealth(combatData["MaxHealth"].ToObject<float>());
			combatEntity.SetHealth(combatData["Health"].ToObject<float>());
		}

		entity.SendNetworkUpdate();
		return GetResponse();
	}

	[Rpc]
	private static Response EntityKill(ConsoleSystem.Arg arg)
	{
		var entity = BaseNetworkable.serverEntities.Find(new NetworkableId(arg.GetULong(1))) as BaseEntity;
		if (!entity.IsValid() || entity.IsDestroyed)
		{
			return GetResponse();
		}

		if (entity is BasePlayer playerEntity)
		{
			playerEntity.Hurt(playerEntity.MaxHealth() + 1);
		}
		else
		{
			entity.AdminKill();
		}

		return GetResponse();
	}

	private static object ParseEntityMetadata(BaseEntity entity)
	{
		return new
		{
			NetId = entity.net.ID.Value,
			Name = entity.name,
			ShortName = entity.ShortPrefabName,
			Id = entity.prefabID,
			Flags = entity.flags,
			PosX = entity.ServerPosition.x,
			PosY = entity.ServerPosition.y,
			PosZ = entity.ServerPosition.z,
			RotX = entity.ServerRotation.eulerAngles.x,
			RotY = entity.ServerRotation.eulerAngles.y,
			RotZ = entity.ServerRotation.eulerAngles.z
		};
	}

	private static object ParseEntityDetails(BaseEntity entity)
	{
		return new
		{
			NetId = entity.net.ID.Value,
			Name = entity.name,
			ShortName = entity.ShortPrefabName,
			Id = entity.prefabID,
			Flags = entity.flags.ToString().Split(','),
			Type = entity.GetType().Name,
			PosX = entity.ServerPosition.x,
			PosY = entity.ServerPosition.y,
			PosZ = entity.ServerPosition.z,
			RotX = entity.ServerRotation.eulerAngles.x,
			RotY = entity.ServerRotation.eulerAngles.y,
			RotZ = entity.ServerRotation.eulerAngles.z,
			Owner = entity.OwnerID,
			Skin = entity.skinID,
			Parent = entity.HasParent() ? ParseEntityMetadata(entity.GetParentEntity()) : 0,
			Children = entity.children.Select(x => ParseEntityMetadata(x)),
			CombatEntity =
				entity is BaseCombatEntity combatEntity
					? new
					{
						Health = Math.Round(combatEntity.Health(), 2),
						MaxHealth = Math.Round(combatEntity.MaxHealth(), 2)
					}
					: null,
			PlayerEntity = entity is BasePlayer playerEntity
				? new
				{
					DisplayName = playerEntity.displayName,
					UserId = playerEntity.userID.Get(),
					Thirst = Math.Round(playerEntity.metabolism.hydration.value, 2),
					MaxThirst = Math.Round(playerEntity.metabolism.hydration.max, 2),
					Hunger = Math.Round(playerEntity.metabolism.calories.value, 2),
					MaxHunger = Math.Round(playerEntity.metabolism.calories.max, 2),
					Rads = Math.Round(playerEntity.metabolism.radiation_level.value, 2),
					MaxRads = Math.Round(playerEntity.metabolism.radiation_level.max, 2),
					Bleed = Math.Round(playerEntity.metabolism.bleeding.value, 2),
					MaxBleed = Math.Round(playerEntity.metabolism.bleeding.max, 2),
				}
				: null
		};
	}
}
