using System.Reflection;
using Carbon.Documentation.Generators;

#pragma warning disable

public sealed class Generator
{
	public static CommandLineArguments Arguments;

	public static void Generate()
	{
		var files = Directory.GetFiles(Arguments.Carbon, "*.dll", SearchOption.AllDirectories).Concat(
										 Directory.GetFiles(Arguments.Rust, "*.dll", SearchOption.AllDirectories));
		AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
		{
			var name = args.Name[..(args.Name.IndexOf(' ') - 1)];
			if (!_assemblyCache.TryGetValue(name, out var assembly))
			{
				var assemblyFile = files.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).Equals(name, StringComparison.OrdinalIgnoreCase));
				if (string.IsNullOrEmpty(assemblyFile))
				{
					return null;
				}

				assemblyFile = Path.GetFullPath(assemblyFile);
				Console.WriteLine($" Resolving assembly '{name}' @ {assemblyFile}");
				_assemblyCache[name] = assembly = Assembly.LoadFrom(assemblyFile);
			}
			return assembly;
		};

		LoadAssembly(Path.Combine(Arguments.Carbon, "managed", "Carbon.Common.dll"));
		HooksAIResearch.LoadResearch();
		Hooks.CollectHooks(LoadAssembly(Path.Combine(Arguments.Carbon, "managed", "hooks", "Carbon.Hooks.Oxide.dll")), Hooks.Oxide, true);
		Hooks.CollectHooks(LoadAssembly(Path.Combine(Arguments.Carbon, "managed", "hooks", "Carbon.Hooks.Community.dll")), Hooks.Community, false);
		Hooks.CollectHooks(LoadAssembly(Path.Combine(Arguments.Carbon, "managed", "hooks", "Carbon.Hooks.Base.dll")), Hooks.Base, false);
		Hooks.GenerateHooks();
		HooksAIMetadata.Generate();
	}

	#region Helpers

	private static Dictionary<string, Assembly> _assemblyCache = new();

	private static Assembly LoadAssembly(string path)
	{
		path = Path.GetFullPath(path);

		if (!File.Exists(path))
		{
			Console.WriteLine($"Couldn't find assembly: {path}");
			return null;
		}

		Console.WriteLine($"Loading assembly: {path}");

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

	#endregion
}
