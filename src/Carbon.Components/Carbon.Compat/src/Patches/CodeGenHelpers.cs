using API.Assembly;
using API.Events;
using AsmResolver.DotNet.Collections;
using HarmonyLib;

namespace Carbon.Compat.Patches;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public static class CodeGenHelpers
{
	public static void GenerateEntrypoint(ModuleDefinition asm, ReferenceImporter importer, string name, Guid guid, out MethodDefinition load, out MethodDefinition unload, out TypeDefinition typeDef)
	{
		const MethodAttributes attr = MethodAttributes.CompilerControlled |
			MethodAttributes.Final |
			MethodAttributes.HideBySig |
			MethodAttributes.NewSlot |
			MethodAttributes.Virtual;

		// define type
		TypeDefinition entrypoint = new TypeDefinition($"Carbon.Compat.Generated.{name}", $"Entrypoint",
			TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout | TypeAttributes.NotPublic, asm.CorLibTypeFactory.Object.Type
		);
		entrypoint.Interfaces.Add(new InterfaceImplementation(importer.ImportType(typeof(ICarbonAddon))));
		entrypoint.Interfaces.Add(new InterfaceImplementation(importer.ImportType(typeof(ICarbonExtension))));
		entrypoint.AddDefaultCtor(asm, importer);

		TypeSignature eventArgsRef = importer.ImportTypeSignature(typeof(EventArgs));

		// define not used methods
		MethodDefinition onAwake = new MethodDefinition(nameof(ICarbonAddon.Awake), attr, MethodSignature.CreateInstance(asm.CorLibTypeFactory.Void, eventArgsRef));
		entrypoint.MethodImplementations.Add(new MethodImplementation((IMethodDefOrRef)importer.ImportMethod(AccessTools.Method(typeof(ICarbonAddon), nameof(ICarbonAddon.Awake))), onAwake));
		entrypoint.Methods.Add(onAwake);

		onAwake.CilMethodBody = new CilMethodBody(onAwake);
		onAwake.CilMethodBody.Instructions.Add(CilOpCodes.Ret);

		// define on unload
		unload = new MethodDefinition(nameof(ICarbonAddon.OnUnloaded), attr, MethodSignature.CreateInstance(asm.CorLibTypeFactory.Void, eventArgsRef));
		entrypoint.MethodImplementations.Add(new MethodImplementation((IMethodDefOrRef)importer.ImportMethod(AccessTools.Method(typeof(ICarbonAddon), nameof(ICarbonAddon.OnUnloaded))), unload));
		entrypoint.Methods.Add(unload);

		// define on loaded
		load = new MethodDefinition(nameof(ICarbonAddon.OnLoaded), attr, MethodSignature.CreateInstance(asm.CorLibTypeFactory.Void, eventArgsRef));
		entrypoint.MethodImplementations.Add(new MethodImplementation((IMethodDefOrRef)importer.ImportMethod(AccessTools.Method(typeof(ICarbonAddon), nameof(ICarbonAddon.OnLoaded))), load));
		entrypoint.Methods.Add(load);
		asm.TopLevelTypes.Add(entrypoint);
		typeDef = entrypoint;
	}

	public static void GenerateCarbonEventCall(CilMethodBody body, ReferenceImporter importer, ref int index, CarbonEvent eventId, MethodDefinition method, CilInstruction self = null, string event_method = "Subscribe")
	{
		self ??= new CilInstruction(CilOpCodes.Ldnull);
		List<CilInstruction> IL = new List<CilInstruction>()
		{
			new CilInstruction(CilOpCodes.Call, importer.ImportMethod(AccessTools.PropertyGetter(typeof(Carbon.Community), "Runtime"))),
			new CilInstruction(CilOpCodes.Callvirt, importer.ImportMethod(AccessTools.PropertyGetter(typeof(Carbon.Community), "Events"))),
			new CilInstruction(CilOpCodes.Ldc_I4, (int)eventId),
			self,
			new CilInstruction(CilOpCodes.Ldftn, method),
			new CilInstruction(CilOpCodes.Newobj, importer.ImportMethod(AccessTools.Constructor(typeof(Action<EventArgs>), new Type[]
			{
				typeof(object), typeof(IntPtr)
			}))),
			new CilInstruction(CilOpCodes.Callvirt, importer.ImportMethod(AccessTools.Method(typeof(API.Events.IEventManager), event_method)))
		};
		body.Instructions.InsertRange(index, IL);
		index += IL.Count;
	}

	public static void DoMultiMethodCall(CilMethodBody body, ref int index, List<IMethodDescriptor> staticMethods, IEnumerable<TypeDefinition> instanceTypes, IMethodDescriptor instanceMethod)
	{
		List<CilInstruction> IL = new List<CilInstruction>();

		if (staticMethods != null)
		{
			foreach (MethodDefinition method in staticMethods)
			{
				foreach (Parameter parameter in method.Parameters)
				{
					if (!parameter.ParameterType.IsValueType)
					{
						IL.Add(new CilInstruction(CilOpCodes.Ldnull));
						continue;
					}
				}
				IL.Add(new CilInstruction(CilOpCodes.Call, method));
				if (method.Signature.ReturnsValue)
				{
					IL.Add(new CilInstruction(CilOpCodes.Pop));
				}
			}
		}

		if (instanceTypes != null)
		{
			foreach (TypeDefinition type in instanceTypes)
			{
				IL.Add(new CilInstruction(CilOpCodes.Newobj, type.Methods.First(x => x.Parameters.Count == 0 && x.Name == ".ctor")));

				foreach (TypeSignature parameter in instanceMethod.Signature.ParameterTypes)
				{
					if (!parameter.IsValueType)
					{
						IL.Add(new CilInstruction(CilOpCodes.Ldnull));
					}
				}

				IL.Add(new CilInstruction(CilOpCodes.Callvirt, instanceMethod));

				if (instanceMethod.Signature.ReturnsValue)
				{
					IL.Add(new CilInstruction(CilOpCodes.Pop));
				}
			}
		}

		body.Instructions.InsertRange(index, IL);
		index += IL.Count;
	}
}
