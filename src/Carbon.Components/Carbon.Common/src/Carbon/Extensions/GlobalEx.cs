using Facepunch;

public static class GlobalEx
{
	public static bool IsSteamId(this string id)
	{
		return ulong.TryParse(id, out var num) && num.IsSteamId();
	}

	public static bool IsSteamId(this ulong id)
	{
		return id > 76561197960265728UL;
	}

	[Obsolete("This method is deprecated! Use effect.Clear() instead.")]
	public static void Clear(this Effect effect, bool _)
	{
		effect.Clear();
	}

	public static void CancelAllInvokes(this FacepunchBehaviour behaviour)
	{
		using var invokes = Pool.Get<PooledList<InvokeAction>>();
		InvokeHandler.FindInvokes(behaviour, invokes);
		for (int i = 0; i < invokes.Count; i++)
		{
			var invoke = invokes[i];
			if (invoke.action == null)
			{
				continue;
			}
			behaviour.CancelInvokeFixedTime(invoke.action);
			behaviour.CancelInvoke(invoke.action);
		}
	}
}
