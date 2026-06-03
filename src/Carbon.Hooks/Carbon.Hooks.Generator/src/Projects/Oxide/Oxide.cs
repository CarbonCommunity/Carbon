#nullable disable

using Carbon.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Carbon.Projects.Oxide;

/// <summary>
///     An Oxide patcher project
/// </summary>
public class Oxide
{
	public static Oxide Load(string fileName)
	{
		try
		{
			using var sr = new StreamReader(fileName);
			var json = sr.ReadToEnd();
			var retobj = JsonConvert.DeserializeObject<Oxide>(json);
			retobj.PostProcess();
			Logger.Information($"Project file '{Path.GetFileName(fileName)}' is loaded");
			return retobj;
		}
		catch (Exception ex)
		{
			Logger.Error("Error while parsing the json project file.");
			Logger.None($"{ex}");
			throw;
		}
	}

	/// <summary>
	///     Gets or sets the project name
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	///     Gets or sets the directory of the dlls
	/// </summary>
	public string TargetDirectory { get; set; }

	/// <summary>
	///     Gets or sets all the manifests contained in this project
	/// </summary>
	public List<Manifest> Manifests { get; set; }

	/// <summary>
	///     Initializes a new instance of the Project class with sensible defaults
	/// </summary>
	public Oxide()
	{
		// Fill in defaults
		Manifests = [];
	}

	private static readonly Dictionary<string, string> OperandReplacers = new()
	{
		["Facepunch.System|Facepunch.Pool|FreeUnmanaged[UnityEngine.PhysicsModule|UnityEngine.RaycastHit]"] =
			"Carbon.Common|Carbon.Pooling.PoolEx|FreeRaycastHitList",
	};

	private void PostProcess()
	{
		for (var manifestIndex = 0; manifestIndex < Manifests.Count; manifestIndex++)
		{
			var manifest = Manifests[manifestIndex];
			for (var hookIndex = 0; hookIndex < manifest.Hooks.Count; hookIndex++)
			{
				var hook = manifest.Hooks[hookIndex];
				if (hook.Hook.Instructions == null)
				{
					continue;
				}

				for (var instructionIndex = 0; instructionIndex < hook.Hook.Instructions.Count; instructionIndex++)
				{
					var instruction = hook.Hook.Instructions[instructionIndex];
					if (instruction.Operand == null)
					{
						continue;
					}

					var operand = instruction.Operand.ToString();

					if (OperandReplacers.TryGetValue(operand, out var value))
					{
						instruction.Operand = value;
					}
				}
			}
		}
	}

	/// <summary>
	///     Gets or sets the name of the assembly in the target directory
	/// </summary>
	public class Manifest
	{
		/// <summary>
		///     Gets or sets the name of the assembly in the target directory
		/// </summary>
		public string AssemblyName { get; set; }

		/// <summary>
		///     Gets or sets the hooks contained in this project
		/// </summary>
		public List<HookDef> Hooks { get; set; }

		/// <summary>
		///     Gets or sets the changed modifiers in this project
		/// </summary>
		public List<ModifierDef> Modifiers { get; set; }

		/// <summary>
		///     Gets or sets the additional fields in this project
		/// </summary>
		public List<FieldDef> Fields { get; set; }

		/// <summary>
		///     Initializes a new instance of the Manifest class
		/// </summary>
		public Manifest()
		{
			// Fill in defaults
			Hooks = [];
			Modifiers = [];
			Fields = [];
		}
	}

	/// <summary>
	///     Represents a hook that is applied to single method and calls a single Oxide hook
	/// </summary>
	public class HookDef
	{
		public string Type { get; set; }

		public Data Hook { get; set; }

		public class Data
		{
			/// <summary>
			///     Gets a human friendly type name for this hook
			/// </summary>
			public string HookTypeName { get; set; }
			// enum: "Initialize Oxide", Modify and "Simple"

			/// <summary>
			///     Gets or sets a name for this hook
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			///     Gets or sets the name of the Oxide hook to call
			/// </summary>
			public string HookName { get; set; }

			/// <summary>
			///     Gets or sets the name of the assembly in which the target type resides
			/// </summary>
			public string AssemblyName { get; set; }

			/// <summary>
			///     Gets or sets the fully qualified name for the type in which the target method resides
			/// </summary>
			public string TypeName { get; set; }

			/// <summary>
			///     Gets or sets if this hook has been flagged
			/// </summary>
			public bool Flagged { get; set; }

			/// <summary>
			///     Gets or sets the target method signature
			/// </summary>
			public MethodSignature Signature { get; set; }

			/// <summary>
			///     Gets or sets the MSIL hash of the target method
			/// </summary>
			public string MsilHash { get; set; }

			/// <summary>
			///     Gets or sets the base hook name
			/// </summary>
			public string BaseHookName { get; set; }

			/// <summary>
			///     Gets or sets the base hook
			/// </summary>
			public string BaseHook { get; set; }

			/// <summary>
			///     Gets or sets the hook category
			/// </summary>
			public string HookCategory { get; set; }

			/// <summary>
			///     A simple hook that injects at a certain point in the method, with a few options for handling arguments and return values
			/// </summary>
			/// <summary>
			///     Gets or sets the instruction index to inject the hook call at
			/// </summary>
			public int InjectionIndex { get; set; }

			public int RemoveCount { get; set; }

			/// <summary>
			///     Gets or sets the return behavior
			/// </summary>
			public ReturnBehavior ReturnBehavior { get; set; }

			/// <summary>
			///     Gets or sets the argument behavior
			/// </summary>
			public ArgumentBehavior ArgumentBehavior { get; set; }

			/// <summary>
			///     Gets or sets the argument string
			/// </summary>
			public string ArgumentString { get; set; }

			public bool IsInternal => HookName.StartsWith("IOn") || HookName.StartsWith("ICan");

			public DeprecatedStatus Deprecation { get; set; }

			public class DeprecatedStatus
			{
				public string ReplacementHook { get; set; }
				public DateTime RemovalDate { get; set; }
			}


			/// <summary>
			///     A simple hook that injects at a certain point in the method, with a few options for handling arguments and return values
			/// </summary>
			public enum OpType
			{
				None,
				Byte,
				SByte,
				Int32,
				Int64,
				Single,
				Double,
				String,
				VerbatimString,
				Instruction,
				Variable,
				Parameter,
				Field,
				Method,
				Generic,
				Type,
				VariableIndex,
			}

			public class InstructionData
			{
				public string OpCode { get; set; }

				[JsonConverter(typeof(StringEnumConverter))]
				public OpType OpType { get; set; }

				public object Operand { get; set; }

				public bool ReferencesNewInstruction { get; set; }

				public override string ToString()
				{
					return $"{OpCode} {Operand}";
				}
			}

			public List<InstructionData> Instructions { get; set; }

			public Data()
			{
				Instructions = [];
			}
		}
	}

	/// <summary>
	///     Represents a hook that is applied to single method and calls a single Oxide hook
	/// </summary>
	public class ModifierDef
	{
		/// <summary>
		///     Gets or sets a name for this modifier
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Gets or sets the name of the assembly in which the target resides
		/// </summary>
		public string AssemblyName { get; set; }

		/// <summary>
		///     Gets or sets the fully qualified name for the type in which the target resides
		/// </summary>
		public string TypeName { get; set; }

		/// <summary>
		///     Gets or sets the type of the target
		/// </summary>
		public ModifierType Type { get; set; }

		/// <summary>
		///     Gets the target exposure
		/// </summary>
		public Exposure[] TargetExposure { get; set; }

		/// <summary>
		///     Gets or sets if this modifier has been flagged
		/// </summary>
		public bool Flagged { get; set; }

		/// <summary>
		///     Gets or sets the target signature
		/// </summary>
		public ModifierSignature Signature { get; set; }

		/// <summary>
		///     Gets or sets the MSIL hash of the target
		/// </summary>
		public string MsilHash { get; set; }

		/// <summary>
		///     Represents the signature of a method, field or property
		/// </summary>
		public sealed class ModifierSignature
		{
			/// <summary>
			///     Gets the exposure
			/// </summary>
			public Exposure[] Exposure { get; set; }

			/// <summary>
			///     Gets the name
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			///     Gets the method return type or the field or property type as a fully qualified type name
			/// </summary>
			public string FullTypeName { get; set; }

			/// <summary>
			///     Gets the parameter list for methods as fully qualified type names
			/// </summary>
			public string[] Parameters { get; set; }
		}
	}

	/// <summary>
	///     Represents a hook that is applied to single method and calls a single Oxide hook
	/// </summary>
	public class FieldDef
	{
		/// <summary>
		///     Gets or sets a name for this field
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Gets or sets the name of the assembly in which the target resides
		/// </summary>
		public string AssemblyName { get; set; }

		/// <summary>
		///     Gets or sets the fully qualified name for the type in which the target resides
		/// </summary>
		public string TypeName { get; set; }

		/// <summary>
		///     Gets or sets the field to be added
		/// </summary>
		public string FieldType { get; set; }

		/// <summary>
		///     Gets or sets if this modifier has been flagged
		/// </summary>
		public bool Flagged { get; set; }
	}

	/// <summary>
	///     Represents the signature of a method
	/// </summary>
	public class MethodSignature
	{
		/// <summary>
		///     Gets the method exposure
		/// </summary>
		public MethodExposure Exposure { get; set; }

		/// <summary>
		///     Gets the method name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Gets the method return type as a fully qualified type name
		/// </summary>
		public string ReturnType { get; set; }

		/// <summary>
		///     Gets the parameter list as fully qualified type names
		/// </summary>
		public string[] Parameters { get; set; }

		public enum MethodExposure
		{
			Private,
			Protected,
			Public,
			Internal,
		}
	}

	public enum ModifierType
	{
		Field,
		Method,
		Property,
		Type,
	}

	public enum Exposure
	{
		Private,
		Protected,
		Public,
		Internal,
		Static,
		Null,
	}

	public enum ReturnBehavior
	{
		Continue,
		ExitWhenValidType,
		ModifyRefArg,
		UseArgumentString,
		ExitWhenNonNull,
	}

	public enum ArgumentBehavior
	{
		None,
		JustThis,
		JustParams,
		All,
		UseArgumentString,
	}
}
