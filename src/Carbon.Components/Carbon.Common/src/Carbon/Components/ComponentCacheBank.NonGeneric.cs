namespace Carbon.Components;

/// <summary>
/// General implementation of the ComponentCache Carbon component.
/// </summary>
public static class ComponentCacheBankNonGeneric
{
	public static List<IComponentBank> All = [];

	public static T AddComponentCache<T>(this GameObject go) where T : MonoBehaviour
	{
		return go == null ? default : ComponentCacheBank<T>.Instance.Add(go);
	}
	public static T GetComponentCache<T>(this GameObject go) where T : MonoBehaviour
	{
		return go == null ? default : ComponentCacheBank<T>.Instance.Get(go);
	}
	public static bool RemoveComponentCache<T>(this GameObject go) where T : MonoBehaviour
	{
		return go == null ? default : ComponentCacheBank<T>.Instance.Remove(go);
	}
	public static bool TryGetOrAddComponentCache<T>(this GameObject go, out T component) where T : MonoBehaviour
	{
		return (component = go.GetComponentCache<T>() ?? go.AddComponentCache<T>()) != null;
	}
	public static bool DestroyCache(this GameObject go)
	{
		if (go == null)
		{
			return false;
		}

		return All.Count(cache => cache.Destroy(go)) > 0;
	}
	public static void OnEntityDestruct(BaseEntity entity)
	{
		if (entity == null || entity.gameObject == null)
		{
			return;
		}

		foreach(var element in All)
		{
			element.Remove(entity.gameObject);
		}
	}
}
