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
	public static class Publicizer
	{
		public static bool Publicize(string input)
		{
			AssemblyDefinition assembly = null;
			string WorkingPath = Path.GetDirectoryName(input);
			string WorkingFile = Path.GetFileName(input);

			try
			{
				if (!File.Exists(input))
					throw new Exception($"Assembly file '{WorkingFile}' was not found");

				string f = Path.Combine(WorkingPath, WorkingFile);
				assembly = AssemblyDefinition.ReadAssembly(f);
				Logger.Log($"Loaded '{f}'");
			}
			catch (Exception ex)
			{
				Logger.Error("Error reading assembly from file", ex);
				return false;
			}

			try
			{
				/// DISCLAIMER
				/// Some of the following code is based on BepInEx/NStrip
				/// Copyright (c) 2021 BepInEx, released under MIT License
				AssemblyNameReference scope = assembly.MainModule.AssemblyReferences.
					OrderByDescending(a => a.Version).FirstOrDefault(a => a.Name == "mscorlib");

				MethodReference nsAttributeCtor = new MethodReference(".ctor",
					assembly.MainModule.TypeSystem.Void, new TypeReference(
					"System", "NonSerializedAttribute", assembly.MainModule, scope))
				{ HasThis = true };
				/// EOD

				Logger.Log("Publicize process will execute..");

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
				string f = Path.Combine(WorkingPath, $"__{WorkingFile}");
				Logger.Log($"Writing changes to '{f}'..");

				/*
				var tempStream = new MemoryStream();
				assembly.Write(tempStream);

				tempStream.Position = 0;
				var outputStream = File.Open(f, FileMode.Create);
				tempStream.CopyTo(outputStream);
				*/

				assembly.Write(f);
				assembly.Dispose();

			}
			catch (Exception ex)
			{
				Logger.Error("Error saving assembly to file", ex);
				return false;
			}

			assembly.Dispose();
			return true;
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
	}
}