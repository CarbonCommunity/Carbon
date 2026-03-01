namespace Carbon.Extensions;

public static class EnumerableEx
{
	public static int IndexOf<T>(this IEnumerable<T> enumerable, T value)
	{
		if (value == null)
		{
			return default;
		}

		var index = 0;

		foreach (var iteration in enumerable)
		{
			if (iteration.Equals(value))
			{
				return index;
			}

			index++;
		}

		return index;
	}

	public static int FindIndex<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
	{
		var index = 0;

		if (predicate == null)
		{
			index += enumerable.Count();
		}
		else
		{
			index += enumerable.Count(iteration => predicate(iteration));
		}

		return index;
	}

	public static T FindAt<T>(this IEnumerable<T> enumerable, int index)
	{
		var currentIndex = 0;

		foreach (var iteration in enumerable)
		{
			if (currentIndex == index)
			{
				return iteration;
			}

			currentIndex++;
		}

		return default;
	}

	public static ulong SumULong<TSource>(this IEnumerable<TSource> source, Func<TSource, ulong> selector)
	{
		return source.Select(selector).Aggregate(0UL, (current, value) => current + value);
	}

	public static long SumLong<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
	{
		return source.Select(selector).Aggregate(0L, (current, value) => current + value);
	}

	public static uint SumUInt<TSource>(this IEnumerable<TSource> source, Func<TSource, uint> selector)
	{
		return source.Aggregate<TSource, uint>(0, (current, value) => current + selector(value));
	}
}
