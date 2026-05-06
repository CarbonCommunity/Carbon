namespace Carbon.Components;

[Obsolete("TempArray is obsolete and is going to be removed entirely from Carbon on July 3rd, 2025.")]
public class TempArray<T> : IDisposable
{
	public T[] array;

	public bool IsEmpty => array == null || array.Length == 0;

	public int Length => IsEmpty ? 0 : array.Length;

	public T Get(int index, T @default = default)
	{
		return index > array.Length - 1 ? @default : array[index];
	}

	public static TempArray<T> New(T[] array)
	{
		return new TempArray<T>
		{
			array = array
		};
	}

	public void Dispose()
	{
		if (array == null)
		{
			return;
		}

		System.Array.Clear(array, 0, array.Length);
		array = null;
	}
}
