namespace Carbon.Extensions;

public static class ArrayEx
{
	public static T[] Randomize<T>(this T[] list)
	{
		var listCopy = list.MakeCopy();
		var count = listCopy.Length;

		while (count > 1)
		{
			count--;

			var randomIndex = RandomEx.GetRandomInteger(0, count);
			var value = listCopy[randomIndex];

			listCopy[randomIndex] = listCopy[count];
			listCopy[count] = value;
		}

		return listCopy;
	}

	public static bool IsSame<T>(this T[] source, T[] target)
	{
		return !source.Except(target).Any();
	}

	public static T[][] Chunkify<T>(this T[] source, int chunkSize)
	{
		var numberOfChunks = (int)Math.Ceiling((double)source.Length / chunkSize);
		var result = new T[numberOfChunks][];

		for (int i = 0; i < numberOfChunks; i++)
		{
			var chunkStart = i * chunkSize;
			var chunkEnd = Math.Min(chunkStart + chunkSize, source.Length);
			var chunkLength = chunkEnd - chunkStart;

			result[i] = new T[chunkLength];
			Array.Copy(source, chunkStart, result[i], 0, chunkLength);
		}

		return result;
	}
}
