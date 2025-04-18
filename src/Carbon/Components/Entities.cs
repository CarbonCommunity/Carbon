using System.Data.Entity.Core.Objects;
using UnityEngine;

namespace Carbon;

/// <summary>
/// Carbon component centralized place of accessing Rust spawned entities.
/// </summary>
[Obsolete("Entities is going to be no longer used and later on permanently removed.")]
public class Entities
{
	public static void Init()
	{
		try
		{
			Dispose();

			foreach (var type in _findAssignablesFrom<BaseEntity>())
			{
				Mapping.Add(new(type), new List<object>(100000));
			}

			if (Community.IsServerInitialized)
			{
				Carbon.Logger.Warn($"Mapping {BaseNetworkable.serverEntities.Count:n0} entities... This will take a while.");
			}

			using (TimeMeasure.New("Entity mapping"))
			{
				foreach (var type in Mapping)
				{
					type.Value.AddRange(BaseNetworkable.serverEntities.Where(x => x.GetType() == type.Key.type).Select(x => x as BaseEntity));
				}
			}

			if (Community.IsServerInitialized)
			{
				Carbon.Logger.Warn($"Done mapping.");
			}
		}
		catch (Exception ex)
		{
			Carbon.Logger.Error($"Failed Entities.Init()", ex);
		}
	}

	public static void Dispose()
	{
		foreach (var map in Mapping)
		{
			map.Value.Clear();
		}

		Mapping.Clear();
	}

	public static Dictionary<EntityType, List<object>> Mapping { get; internal set; } = new();

	internal static IEnumerable<Type> _findAssignablesFrom<TBaseType>()
	{
		var baseType = typeof(TBaseType);
		var assembly = baseType.Assembly;

		return assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t));
	}

	/// <summary>
	/// Gets a map of a specified entity type, or inherited from provided type (if inherited is true).
	/// </summary>
	public static Map<T> Get<T>(bool inherited = false) where T : class, new()
	{
		var map = new Map<T>
		{
			Pool = Facepunch.Pool.Get<List<T>>()
		};

		if (inherited)
		{
			foreach (var entry in Mapping)
			{
				if (typeof(T).IsAssignableFrom(entry.Key.type))
				{
					foreach (T entity in entry.Value)
					{
						map.Pool.Add(entity);
					}
				}
			}
		}
		else
		{
			if (Mapping.TryGetValue(new(typeof(T)), out var mapping))
			{
				foreach (var entity in mapping)
				{
					if (entity is T result) map.Pool.Add(result);
				}
			}
		}

		return map;
	}

	/// <summary>
	/// Gets all entities of BaseEntity. If inherited is true, it returns everything spawnable on the server, otherwise ONLY entities with BaseEntity as base class.
	/// </summary>
	public static Map<BaseEntity> GetAll(bool inherited = true)
	{
		var map = new Map<BaseEntity>
		{
			Pool = Facepunch.Pool.Get<List<BaseEntity>>()
		};

		if (inherited)
		{
			foreach (var entry in Mapping)
			{
				if (typeof(BaseEntity).IsAssignableFrom(entry.Key.type))
				{
					foreach (var entity in entry.Value)
					{
						map.Pool.Add(entity as BaseEntity);
					}
				}
			}
		}
		else
		{
			if (Mapping.TryGetValue(new(typeof(BaseEntity)), out var mapping))
			{
				foreach (var entity in mapping)
				{
					if (entity is BaseEntity result) map.Pool.Add(result);
				}
			}
		}

		return map;
	}

	/// <summary>
	/// Gets all entities of BaseEntity with the option of having them filtered out. If inherited is true, it returns everything spawnable on the server, otherwise ONLY entities with BaseEntity as base class.
	/// </summary>
	public static Map<BaseEntity> GetAllFiltered(Func<BaseEntity, bool> filter, bool inherited = false)
	{
		if (filter == null)
		{
			return default;
		}

		var map = new Map<BaseEntity>
		{
			Pool = Facepunch.Pool.Get<List<BaseEntity>>()
		};

		if (inherited)
		{
			foreach (var entry in Mapping)
			{
				if (typeof(BaseEntity).IsAssignableFrom(entry.Key.type))
				{
					foreach (var entity in entry.Value)
					{
						var ent = entity as BaseEntity;

						if (!filter(ent))
						{
							continue;
						}

						map.Pool.Add(ent);
					}
				}
			}
		}
		else
		{
			if (Mapping.TryGetValue(new(typeof(BaseEntity)), out var mapping))
			{
				foreach (var entity in mapping)
				{
					if (entity is BaseEntity result && filter(result))
					{
						map.Pool.Add(result);
					}
				}
			}
		}

		return map;
	}

	/// <summary>
	/// Get one sample of a specific entity type, or if inherited is true, any entity type with T as inherited type.
	/// </summary>
	public static T GetOne<T>(bool inherited = false) where T : class, new()
	{
		using (var map = Get<T>(inherited))
		{
			return map.Pool.FirstOrDefault();
		}
	}

	/// <summary>
	/// Maps and stores an entity instance.
	/// </summary>
	/// <param name="entity"></param>
	public static void AddMap(BaseEntity entity)
	{
		if (!Mapping.TryGetValue(new(entity.GetType()), out var map))
		{
			return;
		}

		map.Add(entity);
	}

	/// <summary>
	/// Removes an entity instance from the map.
	/// </summary>
	/// <param name="entity"></param>
	public static void RemoveMap(BaseEntity entity)
	{
		if (!Mapping.TryGetValue(new(entity.GetType()), out var map))
		{
			return;
		}

		map.Remove(entity);
		ComponentCacheBankNonGeneric.OnEntityDestruct(entity);
	}

	public struct EntityType : IEqualityComparer<EntityType>
	{
		public Type type;

		public EntityType(Type type)
		{
			this.type = type;
		}

		public override int GetHashCode()
		{
			return (type).GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is EntityType other)
			{
				return other.type == type;
			}

			return base.Equals(obj);
		}

		public bool Equals(EntityType x, EntityType y)
		{
			return x.type == y.type;
		}

		public int GetHashCode(EntityType obj)
		{
			return (obj.type).GetHashCode();
		}
	}

	public struct Map<T> : IDisposable where T : class, new()
	{
		public List<T> Pool;

		public Map<T> Each(Action<T> callback, Func<T, bool> condition = null)
		{
			foreach (var drop in Pool)
			{
				if (condition != null && !condition(drop)) continue;

				callback.Invoke(drop);
			}

			return this;
		}
		public T Pick(int index)
		{
			if (Pool.Count == 0)
			{
				return default;
			}

			if (Pool.Count - 1 > index)
			{
				return default;
			}

			return Pool[index];
		}

		public void Dispose()
		{
			if (Pool == null)
			{
				return;
			}

#if DEBUG
			Logger.Debug($"Cleaned {typeof(T).Name}", 2);
#endif
			Facepunch.Pool.FreeUnmanaged(ref Pool);
		}
	}
}
