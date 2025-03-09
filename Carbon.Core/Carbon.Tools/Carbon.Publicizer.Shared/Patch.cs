using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace Carbon.Publicizer;

#pragma warning disable

public class Patch : IDisposable
{
	public static CarbonAssemblyResolver AssemblyResolver;
	public static List<Patch> Publicized = new();
	public static Dictionary<Assembly, string> PatchMapping = new();

	public static Action<(string path, byte[] buffer)> onBufferUpdate;
	public static Action<(string path, byte[] buffer)> onPatchUpdate;

	public static string CarbonManagedDirectory;
	public static string RustManagedDirectory;

	protected static AssemblyDefinition bootstrap;

	public AssemblyDefinition assembly;
	public ReaderParameters readerParameters;

	public static void Init(string carbonManagedDir, string rustManagedDir)
	{
		CarbonManagedDirectory = carbonManagedDir;
		RustManagedDirectory = rustManagedDir;

		try
		{
			bootstrap = AssemblyDefinition.ReadAssembly(new MemoryStream(File.ReadAllBytes(Path.Combine(carbonManagedDir, "Carbon.Bootstrap.dll"))));
		}
		catch { }

		BuiltInPatches.Current =
		[
			new AssemblyCSharp(),
			new RustHarmony()
		];
	}
	public static void Uninit()
	{
		bootstrap?.Dispose();
		bootstrap = null;
	}

	public string GetFullPath() => Path.Combine(filePath, fileName);

	public byte[] processed;
	public string filePath;
	public string fileName;

	public bool ShouldPublicize => Config.Singleton.Publicizer.PublicizedAssemblies.Any(x => fileName.StartsWith(x, StringComparison.OrdinalIgnoreCase));

	public class CarbonAssemblyResolver : BaseAssemblyResolver {

		readonly IDictionary<string, AssemblyDefinition> cache;

		public CarbonAssemblyResolver ()
		{
			cache = new Dictionary<string, AssemblyDefinition> (StringComparer.Ordinal);
		}

		public override AssemblyDefinition Resolve (AssemblyNameReference name)
		{
			if (cache.TryGetValue (name.FullName, out var assembly))
				return assembly;

			var directories = GetSearchDirectories();
			foreach (var directory in directories)
			{
				var files = Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories);
				foreach (var file in files)
				{
					var fileName = Path.GetFileNameWithoutExtension(file);
					if (fileName.Equals(name.Name, StringComparison.OrdinalIgnoreCase))
					{
						using var stream = new MemoryStream(File.ReadAllBytes(file));
						assembly = AssemblyDefinition.ReadAssembly(stream);
						break;
					}
				}

				if (assembly != null)
				{
					break;
				}
			}

			cache [name.FullName] = assembly;
			return assembly;
		}

		protected void RegisterAssembly (AssemblyDefinition assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException (nameof(assembly));

			var name = assembly.Name.FullName;
			if (cache.ContainsKey (name))
				return;

			cache [name] = assembly;
		}

		protected override void Dispose (bool disposing)
		{
			foreach (var assembly in cache.Values)
				assembly.Dispose ();

			cache.Clear ();

			base.Dispose (disposing);
		}
	}

	public Patch(string path, string name)
	{
		filePath = path;
		fileName = name;

		if (AssemblyResolver == null)
		{
			AssemblyResolver = new();
			AssemblyResolver.AddSearchDirectory(RustManagedDirectory);
		}

		readerParameters = new ReaderParameters { AssemblyResolver = AssemblyResolver };
	}

	public virtual bool Execute()
	{
		assembly = AssemblyDefinition.ReadAssembly(new MemoryStream(File.ReadAllBytes(GetFullPath())), readerParameters);

		if (!ShouldPublicize)
		{
			return false;
		}

		Publicize();
		Publicized.Add(this);
		return true;
	}

	public void UpdateBuffer()
	{
		if (processed != null)
			return;

		using var memoryStream = new MemoryStream();
		assembly.Write(memoryStream);
		processed = memoryStream.ToArray();
		onBufferUpdate?.Invoke((fileName, processed));
	}

	public void Write(string path)
	{
		UpdateBuffer();
		try
		{
			File.WriteAllBytes(path, processed);

			Dispose();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
	}

	public void Load()
	{
		UpdateBuffer();
		var assembly = Assembly.Load(processed);
		PatchMapping[assembly] = Path.Combine(filePath, fileName);
		Console.WriteLine($" Loading patched assembly {fileName}");
	}

	public void Dispose()
	{
		readerParameters = null;
		assembly?.Dispose();
		assembly = null;
	}

	protected void Publicize()
	{
		if (assembly == null)
		{
			throw new Exception($"Loaded assembly is null: {GetFullPath()}");
		}

		foreach (var type in assembly.MainModule.Types)
		{
			Publicize(type);
		}
	}

	private static void Publicize(TypeDefinition type)
	{
		try
		{
			if (Config.Singleton.Publicizer.IsMemberIgnored(type.Name))
			{
				return;
			}

			if (type.IsNested)
				type.IsNestedPublic = true;
			else type.IsPublic = true;

			foreach (var method in type.Methods)
			{
				if (Config.Singleton.Publicizer.IsMemberIgnored($"{type.Name}.{method.Name}"))
				{
					continue;
				}

				method.IsPublic = true;
			}

			foreach (var field in type.Fields)
			{
				if (Config.Singleton.Publicizer.IsMemberIgnored($"{type.Name}.{field.Name}"))
				{
					continue;
				}

				var hasEvent = false;
				foreach (var ev in type.Events)
				{
					if (ev.Name != field.Name) continue;
					hasEvent = true;
					break;
				}

				if (hasEvent) continue;

				var hasSerializeFieldAttribute = false;
				foreach (var attribute in field.CustomAttributes)
				{
					if (attribute.AttributeType.FullName != "UnityEngine.SerializeField") continue;
					hasSerializeFieldAttribute = true;
					break;
				}

				if (!field.IsPublic && !hasSerializeFieldAttribute)
					field.IsNotSerialized = true;

				field.IsPublic = true;
			}

			foreach (var property in type.Properties)
			{
				if (property.GetMethod != null)
					property.GetMethod.IsPublic = true;
				if (property.SetMethod != null)
					property.SetMethod.IsPublic = true;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw ex;
		}

		foreach (var subtype in type.NestedTypes)
			Publicize(subtype);
	}
}
