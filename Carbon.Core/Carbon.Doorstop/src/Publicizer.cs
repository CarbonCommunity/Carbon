///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace Carbon.Utility
{
	internal static class Publicizer
	{
		private static MemoryStream memoryStream = null;

		internal static bool Read(string Source)
		{
			try
			{
				if (!File.Exists(Source))
					throw new Exception($"Assembly file '{Source}' was not found");

				memoryStream = new MemoryStream(File.ReadAllBytes(Source));
				Logger.Log($"Loaded '{Path.GetFileName(Source)}' into memory");
				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("Error reading assembly from '{input}", ex);
				return false;
			}
		}

		internal static bool IsPublic(string Type, string Method)
		{
			try
			{
				if (memoryStream == null) throw new Exception();
				memoryStream.Position = 0;

				AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(memoryStream);
				if (assembly == null) throw new Exception();

				TypeDefinition t = assembly.MainModule.Types.Where(x => x.Name == "ServerMgr").FirstOrDefault();
				if (t == null) throw new Exception();

				MethodDefinition m = t.Methods.Where(x => x.Name == "Shutdown").FirstOrDefault();
				if (m == null) throw new Exception();

				assembly.Dispose();
				assembly = null;

				return m.IsPublic;
			}
			catch (Exception ex)
			{
				Logger.Error($"Unable to extract '{Type}.{Method}", ex);
				return false;
			}
		}

		internal static bool Publicize()
		{
			AssemblyDefinition assembly = null;

			try
			{
				if (memoryStream == null) throw new Exception();
				memoryStream.Position = 0;

				assembly = AssemblyDefinition.ReadAssembly(memoryStream);
				if (assembly == null) throw new Exception();
				Logger.Log($"Assembly read from memory");
			}
			catch (Exception ex)
			{
				Logger.Error("Error reading assembly from memory", ex);
				return false;
			}

			try
			{
				/// DISCLAIMER
				/// Some of the following code is based on BepInEx/NStrip
				/// Copyright (c) 2021 BepInEx, released under MIT License
				AssemblyNameReference scope = assembly.MainModule.AssemblyReferences.
					OrderByDescending(a => a.Version).FirstOrDefault(a => a.Name == "mscorlib");

				MethodReference nsAttributeCtor = new MethodReference(".ctor", assembly.MainModule.TypeSystem.Void,
					new TypeReference("System", "NonSerializedAttribute", assembly.MainModule, scope))
				{ HasThis = true };
				/// EOD

				Logger.Log("Executing the publicize process, please wait..");

				foreach (TypeDefinition Type in assembly.MainModule.Types)
				{
					if (Blacklist.IsBlacklisted(Type.Name))
					{
						Logger.Warn($"Excluded '{Type.Name}' due to blacklisting");
						continue;
					}

					if (Type.IsNested)
					{
						Type.IsNestedPublic = true;
					}
					else
					{
						Type.IsPublic = true;
					}

					foreach (MethodDefinition Method in Type.Methods)
					{
						if (Blacklist.IsBlacklisted($"{Type.Name}.{Method.Name}"))
						{
							Logger.Warn($"Excluded '{Type.Name}.{Method.Name}' due to blacklisting");
							continue;
						}

						Method.IsPublic = true;
					}

					foreach (FieldDefinition Field in Type.Fields)
					{
						if (Blacklist.IsBlacklisted($"{Type.Name}.{Field.Name}"))
						{
							Logger.Warn($"Excluded '{Type.Name}.{Field.Name}' due to blacklisting");
							continue;
						}

						// Prevent publicize auto-generated fields
						if (Type.Events.Any(x => x.Name == Field.Name)) continue;

						/// DISCLAIMER
						/// Some of the following code is based on BepInEx/NStrip
						/// Copyright (c) 2021 BepInEx, released under MIT License
						if (nsAttributeCtor != null && !Field.IsPublic &&
							!Field.CustomAttributes.Any(a => a.AttributeType.FullName == "UnityEngine.SerializeField"))
						{
							Field.IsNotSerialized = true;
							Field.CustomAttributes.Add(new CustomAttribute(nsAttributeCtor));
						}
						/// EOD

						Field.IsPublic = true;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Publicize process aborted", ex);
				return false;
			}

			try
			{
				Logger.Log($"Caching new assembly to memory");
				memoryStream = new MemoryStream();
				assembly.Write(memoryStream);

				assembly.Dispose();
				assembly = null;

				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("Error caching assembly to memory", ex);
				return false;
			}
		}

		/// DISCLAIMER
		/// Some of the following code is based on BepInEx/NStrip
		/// Copyright (c) 2021 BepInEx, released under MIT License
		private static IEnumerable<object> GetAllTypeDefinitions(AssemblyDefinition assembly)
		{
			Queue<TypeDefinition> typeQueue = new Queue<TypeDefinition>(assembly.MainModule.Types);

			while (typeQueue.Count > 0)
			{
				TypeDefinition type = typeQueue.Dequeue();

				yield return type;

				foreach (TypeDefinition nestedType in type.NestedTypes)
					typeQueue.Enqueue(nestedType);
			}
		}
		/// EOD

		internal static bool Write(string Target)
		{
			if (memoryStream == null) return false;
			memoryStream.Position = 0;

			try
			{
				FileStream outputStream = File.Open(Target, FileMode.Create);
				Logger.Log($"Writing assembly to disk..");
				memoryStream.CopyTo(outputStream);

				outputStream.Dispose();
				outputStream = null;

				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("Error writing assembly to file", ex);
				return false;
			}
		}
	}
}