using System.Diagnostics;
using System.Text;

namespace Carbon.Pooling;

public class PoolEx
{
	public static Stopwatch GetStopwatch()
	{
		return Facepunch.Pool.Get<Stopwatch>();
	}

	public static void FreeStopwatch(ref Stopwatch value)
	{
		value.Reset();
		Facepunch.Pool.FreeUnsafe(ref value);
	}

	public static void FreeRaycastHitList(ref List<RaycastHit> hitList)
	{
		Facepunch.Pool.FreeUnmanaged(ref hitList);
	}
}
