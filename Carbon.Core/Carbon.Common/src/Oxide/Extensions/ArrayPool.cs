using System.Collections.Generic;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core
{
	public static class ArrayPool
	{
		internal const int MaxArrayLength = 50;
		internal const int InitialPoolAmount = 64;
		internal const int MaxPoolAmount = 256;

		internal static List<Queue<object[]>> pool = new();

		static ArrayPool()
		{
			for (int i = 0; i < 50; i++)
			{
				pool.Add(new Queue<object[]>());
				SetupArrays(i + 1);
			}
		}

		public static object[] Get(int length)
		{
			if (length == 0 || length > 50)
			{
				return new object[length];
			}

			var queue = pool[length - 1];
			var result = (object[])null;

			lock (queue)
			{
				if (queue.Count == 0)
				{
					SetupArrays(length);
				}
				result = queue.Dequeue();
			}

			return result;
		}

		public static void Free(object[] array)
		{
			if (array == null || array.Length == 0 || array.Length > 50)
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = null;
			}

			var queue = pool[array.Length - 1];

			lock (queue)
			{
				if (queue.Count > 256)
				{
					for (int j = 0; j < 64; j++)
					{
						queue.Dequeue();
					}
				}
				else
				{
					queue.Enqueue(array);
				}
			}
		}

		private static void SetupArrays(int length)
		{
			var queue = pool[length - 1];

			for (int i = 0; i < 64; i++)
			{
				queue.Enqueue(new object[length]);
			}
		}
	}
}
