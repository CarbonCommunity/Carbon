namespace Carbon.Components;

/// <summary>
/// Carbon component used for tracking gameobject custom monobehavior components in plugins.
/// </summary>
/// <typeparam name="T">The type of the MonoBehavior inherited class.</typeparam>
public class ComponentCacheBank<T> : Dictionary<GameObject, List<T>>, IComponentBank where T : MonoBehaviour
{
	public static ComponentCacheBank<T> Instance { get; }

	static ComponentCacheBank()
	{
		Instance = new();
		ComponentCacheBankNonGeneric.All.Add(Instance);
	}

	public T Add(GameObject go)
	{
		if (!this.TryGetValue(go, out var monos))
		{
			this[go] = monos = new();
		}

		var mono = go.AddComponent<T>();

		monos.Add(mono);

		return mono;
	}

	public T Get(GameObject go)
	{
		if (!this.TryGetValue(go, out var monos))
		{
			this[go] = monos = new();
		}

		var existent = monos.FirstOrDefault(x => x != null);

		if (existent != null)
		{
			return existent as T;
		}

		if (!go.TryGetComponent(out existent))
		{
			return default;
		}

		monos.Add(existent);

		return existent as T;
	}

	public bool Remove(GameObject go, bool destroy = true)
	{
		if (!this.TryGetValue(go, out var monos))
		{
			return false;
		}

		var removed = monos.RemoveAll(x =>
		{
			if (destroy)
			{
				GameObject.DestroyImmediate(x);
			}

			return true;
		});

		return removed > 0 && base.Remove(go);
	}

	public bool Destroy(GameObject go)
	{
		if (!this.TryGetValue(go, out var monos))
		{
			GameObject.Destroy(go);
			return false;
		}

		monos.Clear();
		Remove(go, false);
		GameObject.Destroy(go);
		return true;
	}
}

public interface IComponentBank
{
	public bool Remove(GameObject go, bool destroy = true);
	public bool Destroy(GameObject go);
}
