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
				Console.WriteLine($"Loaded '{f}'");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error reading assembly from file");
				Console.WriteLine(ex.Message);
				return false;
			}

			try
			{
				/// DISCLAIMER
				/// Some of the following code is based on BepInEx/NStrip
				/// Copyright (c) 2021 BepInEx, release under MIT License
				AssemblyNameReference scope = assembly.MainModule.AssemblyReferences.OrderByDescending(
							a => a.Version).FirstOrDefault(a => a.Name == "mscorlib");

				MethodReference nsAttributeCtor = null;
				nsAttributeCtor = new MethodReference(".ctor", assembly.MainModule.TypeSystem.Void, new TypeReference(
					"System", "NonSerializedAttribute", assembly.MainModule, scope))
				{ HasThis = true };
				/// EOD

				Console.WriteLine("Publicize process will execute..");

				foreach (TypeDefinition Type in assembly.MainModule.Types)
				{
					if (Type.IsNested)
					{
						Type.IsNestedPublic = true;
					}
					else
					{
						Type.IsPublic = true;
					}

					foreach (MethodDefinition Method in Type.Methods)
						Method.IsPublic = true;

					foreach (FieldDefinition Field in Type.Fields)
					{
						/// DISCLAIMER
						/// Some of the following code is based on BepInEx/NStrip
						/// Copyright (c) 2021 BepInEx, release under MIT License
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
				Console.WriteLine("Publicize process aborted");
				Console.WriteLine(ex.Message);
				return false;
			}

			try
			{
				string f = Path.Combine(WorkingPath, $"__{WorkingFile}");
				assembly.Write(f);
				Console.WriteLine($"Writing changes to '{f}'..");

			}
			catch (Exception ex)
			{
				Console.WriteLine("Error saving assembly to file");
				Console.WriteLine(ex.Message);
				return false;
			}

			return true;
		}

		/// DISCLAIMER
		/// Some of the following code is based on BepInEx/NStrip
		/// Copyright (c) 2021 BepInEx, release under MIT License
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