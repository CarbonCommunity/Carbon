using Random = System.Random;

namespace Carbon.Extensions;

public class RandomEx
{
	public static Random rand = new();
	public const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

	#region String

	private static readonly char[] uiId = new char[4];

	public static string GetRandomString(int size)
	{
		char[] randomChars = size == 4 ? uiId : new char[size];

		for (var i = 0; i < size; i++)
		{
			randomChars[i] = Chars[rand.Next(Chars.Length)];
		}

		var str = new string(randomChars);
		Array.Clear(randomChars, 0, randomChars.Length);
		return str;
	}

	public static string GetRandomString(int size, string chars)
	{
		var randomChars = new char[size];

		for (var i = 0; i < size; i++)
		{
			randomChars[i] = chars[rand.Next(chars.Length)];
		}

		var str = new string(randomChars);
		Array.Clear(randomChars, 0, randomChars.Length);
		return str;
	}
	public static string GetRandomString(int size, int seed)
	{
		Random tempRand = new Random(seed);
		var randomChars = new char[size];

		for (var i = 0; i < size; i++)
		{
			randomChars[i] = Chars[tempRand.Next(Chars.Length)];
		}

		var str = new string(randomChars);
		Array.Clear(randomChars, 0, randomChars.Length);
		return str;
	}
	public static string GetRandomString(int size, string chars, int seed)
	{
		Random tempRand = new Random(seed);
		var randomChars = new char[size];

		for (var i = 0; i < size; i++)
		{
			randomChars[i] = chars[tempRand.Next(chars.Length)];
		}

		var str = new string(randomChars);
		Array.Clear(randomChars, 0, randomChars.Length);
		return str;
	}

	#endregion

	#region Integer

	public static int GetRandomInteger(int min, int max) => rand.Next(min, max);
	public static int GetRandomInteger() => rand.Next(int.MinValue, int.MaxValue);
	public static int GetRandomInteger(int seed)
	{
		Random tempRand = new Random(seed);
		return tempRand.Next(int.MinValue, int.MaxValue);
	}
	public static int GetRandomInteger(int min, int max, int seed)
	{
		Random tempRand = new Random(seed);
		return tempRand.Next(min, max);
	}

	#endregion

	#region Float

	public static float GetRandomFloat(float min, float max) => (float)rand.NextDouble() * (max - min) + min;
	public static float GetRandomFloat() => (float)rand.NextDouble();
	public static float GetRandomFloat(int seed)
	{
		Random tempRand = new Random(seed);
		return (float)tempRand.NextDouble() * (float.MaxValue - float.MinValue) + float.MinValue;
	}
	public static float GetRandomFloat(float min, float max, int seed)
	{
		Random tempRand = new Random(seed);
		return (float)tempRand.NextDouble() * (max - min) + min;
	}

	#endregion

	#region Shuffle

	public static string GetShuffledString(string str)
	{
		var chars = str.ToCharArray();
		var length = chars.Length;

		while (length > 1)
		{
			length--;

			var randomLength = rand.Next(length + 1);
			var value = chars[randomLength];

			chars[randomLength] = chars[length];
			chars[length] = value;
		}

		return new string(chars);
	}
	public static string GetShuffledString(string str, int seed)
	{
		Random tempRand = new Random(seed);
		var chars = str.ToCharArray();
		var length = chars.Length;

		while (length > 1)
		{
			length--;

			var RandomLength = tempRand.Next(length + 1);
			var value = chars[RandomLength];

			chars[RandomLength] = chars[length];
			chars[length] = value;
		}

		return new string(chars);
	}

	#endregion
}
