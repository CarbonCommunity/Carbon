using System.Reflection;

#pragma warning disable

public sealed class Generator
{
	public static CommandLineArguments Arguments;

	public static void Generate()
	{
		var oxideHooks = LoadAssembly(Arguments.OxideHooks);

		Console.WriteLine($"{oxideHooks.GetExportedTypes().Length}");
	}

	private static Assembly LoadAssembly(string path)
	{
		if (!File.Exists(path))
		{
			return null;
		}

		var raw = File.ReadAllBytes(path);
		if (IndexOf(raw, _needleBuffer) == 0)
		{
			ResetChechsum();
			Buffer.BlockCopy(raw, 4, _checksumBuffer, 0, 20);
			return Assembly.Load(Package(_checksumBuffer, raw, 24));
		}
		else
		{
			return Assembly.Load(raw);
		}
	}

	private static byte[] _checksumBuffer = new byte[20];

	private static IReadOnlyList<byte> _needleBuffer = [0x01, 0xdc, 0x7f, 0x01];

	private static void ResetChechsum()
	{
		for (int i = 0; i < _checksumBuffer.Length; i++)
		{
			_checksumBuffer[i] = default;
		}
	}

	private static byte[] Package(IReadOnlyList<byte> a, IReadOnlyList<byte> b, int c = 0)
	{
		var buffer = new byte[b.Count - c];

		for (int i = c; i < b.Count; i++)
		{
			buffer[i - c] = (byte)(b[i] ^ a[(i - c) % a.Count]);
		}

		return buffer;
	}

	private static int IndexOf(IReadOnlyList<byte> haystack, IReadOnlyList<byte> needle)
	{
		int len = needle.Count;
		int limit = haystack.Count - len;

		for (int i = 0; i <= limit; i++)
		{
			int k = 0;
			for (; k < len; k++)
			{
				if (needle[k] != haystack[i + k])
				{
					break;
				}
			}

			if (k == len) return i;
		}
		return -1;
	}
}
