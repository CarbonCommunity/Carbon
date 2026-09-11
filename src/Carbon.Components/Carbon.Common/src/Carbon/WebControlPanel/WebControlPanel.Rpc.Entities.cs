using Facepunch;
using Newtonsoft.Json.Linq;

namespace Carbon;

public static partial class WebControlPanel
{
	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.EntitiesView)]
	private static void RPC_SearchEntities(BridgeRead read)
	{
		var maxCount = read.Int32().Clamp(1, 200);
		var filter = read.String();
		var range = EntitySearchRange.Parse(filter);

		const CompareOptions comparison = CompareOptions.OrdinalIgnoreCase;
		using var entities = Pool.Get<PooledList<BaseEntity>>();

		switch (range.isValid)
		{
			case true:
			{
				Vis.Entities(range.position, range.range, entities, ~0, QueryTriggerInteraction.Ignore);
				for (int i = entities.Count - 1; i >= 0; i--)
				{
					if (string.IsNullOrEmpty(range.filter))
					{
						break;
					}

					var entity = entities[i];
					if (entities.Count > maxCount || (!(entity.GetType().Name.Contains(range.filter, comparison)) ||
					                                  entity.PrefabName.Contains(range.filter, comparison) ||
					                                  entity.net.ID.Value.ToString().Contains(range.filter, comparison)))
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

		var write = StartRpcResponse();
		write.WriteObject(entities.Count);
		for (int i = 0; i < entities.Count; i++)
		{
			new EntityInfo(entities[i]).Serialize(write);
		}
		SendRpcResponse(read.Connection, write);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.EntitiesView)]
	private static void RPC_EntityDetails(BridgeRead read)
	{
		var entity = BaseNetworkable.serverEntities.Find(new NetworkableId(read.UInt64())) as BaseEntity;
		if (!entity.IsValid())
		{
			return;
		}
		var write = StartRpcResponse();
		new DetailedEntityInfo(entity).Serialize(write);
		SendRpcResponse(read.Connection, write);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.EntitiesEdit)]
	private static void RPC_EntitySave(BridgeRead read)
	{
		var details = JObject.Parse(read.String(read.Int32()));
		var entity = BaseNetworkable.serverEntities.Find(new NetworkableId(details["NetId"].ToObject<ulong>())) as BaseEntity;
		if (!entity.IsValid())
		{
			return;
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
			playerEntity.metabolism.radiation_poison.max = playerData["MaxRads"].ToObject<float>();
			playerEntity.metabolism.radiation_poison.value = playerData["Rads"].ToObject<float>();
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
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.EntitiesEdit)]
	private static void RPC_EntityKill(BridgeRead read)
	{
		var entity = BaseNetworkable.serverEntities.Find(new NetworkableId(read.UInt64())) as BaseEntity;
		if (!entity.IsValid())
		{
			return;
		}
		if (entity is BasePlayer playerEntity)
		{
			playerEntity.Hurt(playerEntity.MaxHealth() + 1);
		}
		else
		{
			entity.AdminKill();
		}
	}

	public struct EntityInfo(BaseEntity entity)
	{
		private ulong netId = entity.net.ID.Value;
		private string name = entity.name;
		private string shortName = entity.ShortPrefabName;
		private uint id = entity.prefabID;
		private int flags = (int)entity.flags;
		private float posX = entity.ServerPosition.x;
		private float posY = entity.ServerPosition.y;
		private float posZ = entity.ServerPosition.z;
		private float rotX = entity.ServerRotation.eulerAngles.x;
		private float rotY = entity.ServerRotation.eulerAngles.y;
		private float rotZ = entity.ServerRotation.eulerAngles.z;

		public void Serialize(BridgeWrite write)
		{
			write.WriteObject(netId);
			write.WriteObject(name);
			write.WriteObject(shortName);
			write.WriteObject(id);
			write.WriteObject(flags);
			write.WriteObject(posX);
			write.WriteObject(posY);
			write.WriteObject(posZ);
			write.WriteObject(rotX);
			write.WriteObject(rotY);
			write.WriteObject(rotZ);
		}
	}

	public struct DetailedEntityInfo(BaseEntity entity)
	{
		private ulong netId = entity.net.ID.Value;
		private string name = entity.name;
		private string shortName = entity.ShortPrefabName;
		private uint id = entity.prefabID;
		private string[] flags = entity.flags.ToString().Split(',');
		private string type = entity.GetType().Name;
		private float posX = entity.ServerPosition.x;
		private float posY = entity.ServerPosition.y;
		private float posZ = entity.ServerPosition.z;
		private float rotX = entity.ServerRotation.eulerAngles.x;
		private float rotY = entity.ServerRotation.eulerAngles.y;
		private float rotZ = entity.ServerRotation.eulerAngles.z;
		private ulong owner = entity.OwnerID;
		private ulong skin = entity.skinID;
		private BaseEntity parent = entity.GetParentEntity();
		private List<BaseEntity> children = entity.children;
		private BaseCombatEntity combatEntity = entity as BaseCombatEntity;
		private BasePlayer playerEntity = entity as BasePlayer;

		public void Serialize(BridgeWrite write, bool ignoreParent = false)
		{
			write.WriteObject(netId);
			write.WriteObject(name);
			write.WriteObject(shortName);
			write.WriteObject(id);
			write.WriteObject(flags.Length);
			for (int i = 0; i < flags.Length; i++)
			{
				write.WriteObject(flags[i]);
			}
			write.WriteObject(type);
			write.WriteObject(posX);
			write.WriteObject(posY);
			write.WriteObject(posZ);
			write.WriteObject(rotX);
			write.WriteObject(rotY);
			write.WriteObject(rotZ);
			write.WriteObject(owner);
			write.WriteObject(skin);
			write.WriteObject(parent.IsValid() && !ignoreParent);
			if (parent.IsValid() && !ignoreParent)
			{
				new DetailedEntityInfo(parent).Serialize(write);
			}
			write.WriteObject(children.Count);
			for (int i = 0; i < children.Count; i++)
			{
				new DetailedEntityInfo(children[i]).Serialize(write, true);
			}
			write.WriteObject(combatEntity != null);
			if (combatEntity != null)
			{
				write.WriteObject((float)Math.Round(combatEntity.Health(), 2));
				write.WriteObject((float)Math.Round(combatEntity.MaxHealth(), 2));
			}
			write.WriteObject(playerEntity != null);
			if (playerEntity != null)
			{
				write.WriteObject(playerEntity.displayName);
				write.WriteObject(playerEntity.userID.Get());
				write.WriteObject((float)Math.Round(playerEntity.metabolism.hydration.value, 2));
				write.WriteObject((float)Math.Round(playerEntity.metabolism.hydration.max, 2));
				write.WriteObject((float)Math.Round(playerEntity.metabolism.calories.value, 2));
				write.WriteObject((float)Math.Round(playerEntity.metabolism.calories.max, 2));
				write.WriteObject((float)Math.Round(playerEntity.metabolism.radiation_poison.value, 2));
				write.WriteObject((float)Math.Round(playerEntity.metabolism.radiation_poison.max, 2));
				write.WriteObject((float)Math.Round(playerEntity.metabolism.bleeding.value, 2));
				write.WriteObject((float)Math.Round(playerEntity.metabolism.bleeding.max, 2));
			}
		}
	}

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
			range.position = new Vector3(float.Parse(coordinates[0]), float.Parse(coordinates[1]), float.Parse(coordinates[2]));
			range.range = float.Parse(split[1]);
			range.filter = split.Length >= 3 ? split[2] : null;
			return range;
		}
	}
}
