using System;
using System.Collections.Generic;
using System.Linq;
using Carbon.Core;

public class Entities : IDisposable
{
	public static void Init()
	{
		CarbonCore.Instance.Entities?.Dispose();

		foreach (var type in _findSubClassesOf<BaseEntity>())
		{
			Mapping.Add(type, new List<BaseEntity>(CarbonCore.Instance.Config.EntityMapBufferSize));
		}

		foreach (var entity in BaseNetworkable.serverEntities)
		{
			AddMap(entity as BaseEntity);
		}
	}

	public void Dispose()
	{
		foreach (var map in Mapping)
		{
			map.Value.Clear();
		}

		Mapping.Clear();
	}

	public static Dictionary<Type, List<BaseEntity>> Mapping { get; internal set; } = new Dictionary<Type, List<BaseEntity>>();

	internal static IEnumerable<Type> _findSubClassesOf<TBaseType>()
	{
		var baseType = typeof(TBaseType);
		var assembly = baseType.Assembly;

		return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
	}

	public static Map<T> Get<T>() where T : BaseEntity
	{
		var map = new Map<T>
		{
			Pool = Facepunch.Pool.GetList<T>()
		};

		if (Mapping.TryGetValue(typeof(T), out var mapping))
		{
			foreach (var entity in mapping)
			{
				if (entity is T result) map.Pool.Add(result);
			}
		}

		return map;
	}
	public static void AddMap(BaseEntity entity)
	{
		if (!Mapping.TryGetValue(entity.GetType(), out var map))
		{
			return;
			// EntityMapping.Add(entity.GetType(), map = new List<BaseEntity> { entity });
		}

		if (!map.Contains(entity)) map.Add(entity);
	}
	public static void RemoveMap(BaseEntity entity)
	{
		if (!Mapping.TryGetValue(entity.GetType(), out var map))
		{
			return;
		}

		if (map.Contains(entity)) map.Remove(entity);
	}

	public struct Map<T> : IDisposable where T : BaseEntity
	{
		public List<T> Pool;

		public void Dispose()
		{
			Carbon.Logger.Instance.Warn($"Cleaned {typeof(T).Name}");
			Facepunch.Pool.FreeList(ref Pool);
		}
	}
}
