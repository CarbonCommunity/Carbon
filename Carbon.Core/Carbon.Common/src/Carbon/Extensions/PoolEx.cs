/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Pooling;

public class PoolEx
{
	public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>()
	{
		return Facepunch.Pool.Get<Dictionary<TKey, TValue>>();
	}

	public static void FreeDictionary<TKey, TValue>(ref Dictionary<TKey, TValue> value)
	{
		value.Clear();
		Facepunch.Pool.Free(ref value);
	}
}
