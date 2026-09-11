using AsmResolver;
using Carbon.Base;
using HarmonyLib;
using Assembly = System.Reflection.Assembly;
using AssemblyName = System.Reflection.AssemblyName;

namespace Carbon.Compat;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public static class Helpers
{
	public static bool IsOxideASM(AssemblyReference aref)
	{
		return aref.Name.StartsWith("Oxide.") && !aref.Name.ToLower().StartsWith("oxide.ext.");
	}
	public static bool TryGetLoadedIdentity(Utf8String name, Assembly[] loaded, out AssemblyName identity)
	{
		identity = null;

		foreach (Assembly assembly in loaded)
		{
			AssemblyName current = assembly.GetName();

			if (current.Name != name)
			{
				continue;
			}

			if (identity == null || current.Version > identity.Version)
			{
				identity = current;
			}
		}

		return identity != null;
	}
	private static Assembly _newtonsoft;

	private static Assembly Newtonsoft
	{
		get
		{
			_newtonsoft ??= AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == "Newtonsoft.Json");
			return _newtonsoft;
		}
	}

	public static IResolutionScope GetNewtonsoftScope(TypeReference type, ReferenceImporter importer)
	{
		// Oxide ships a patched Newtonsoft.Json 8.0.0.0 carrying converters the official 13.0.0.0 build
		// Rust uses doesn't have. Carbon mirrors those in Carbon.Common under their original namespace,
		// so anything the real assembly can't provide gets pointed there instead of at a type that
		// would only fail to resolve once the plugin or extension actually runs.
		if (Newtonsoft == null || Newtonsoft.GetType(type.FullName) != null)
		{
			return CompatManager.Newtonsoft.ImportWith(importer);
		}

		if (typeof(BaseHookable).Assembly.GetType(type.FullName) == null)
		{
			Logger.Warn($"Oxide type '{type.FullName}' is missing from Newtonsoft.Json {Newtonsoft.GetName().Version} and Carbon has no equivalent for it");
		}

		return CompatManager.Common.ImportWith(importer);
	}
	public static void AlignIdentityWith(this AssemblyReference reference, AssemblyName identity)
	{
		reference.Version = identity.Version;

		byte[] token = identity.GetPublicKeyToken();

		reference.HasPublicKey = false;
		reference.PublicKeyOrToken = token is { Length: > 0 } ? token : null;
	}
    public static bool StartsWith(this Utf8String str, string value) => str.Value.StartsWith(value);
    public static bool EndsWith(this Utf8String str, string value) => str.Value.EndsWith(value);
    public static Utf8String ToLower(this Utf8String str) => new Utf8String(str.Value.ToLower()); // ITypeDescriptor
    public static void AddDefaultCtor(this TypeDefinition type, ModuleDefinition asm, ReferenceImporter importer)
    {
        MethodDefinition ctor = new MethodDefinition(".ctor",
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
            MethodAttributes.RuntimeSpecialName, MethodSignature.CreateInstance(asm.CorLibTypeFactory.Void));
        ctor.MethodBody = new CilMethodBody(ctor);
        ctor.CilMethodBody.Instructions.AddRange(new[]
        {
            new CilInstruction(CilOpCodes.Ldarg_0),
            new CilInstruction(CilOpCodes.Call, importer.ImportMethod(AccessTools.Constructor(typeof(object)))),
            new CilInstruction(CilOpCodes.Nop),
            new CilInstruction(CilOpCodes.Ret)
        });
        type.Methods.Add(ctor);
    }
    public static bool IsBaseType(this TypeDefinition type, Func<ITypeDefOrRef, bool> call)
    {
	    if (type.BaseType == null)
	    {
		    return false;
	    }

        while (type is { BaseType: not null })
        {
	        if (call(type.BaseType))
	        {
		        return true;
	        }

            type = type.BaseType as TypeDefinition;
        }

        return false;
    }
    public static AssemblyReference DefinitionAssembly(this ITypeDescriptor type)
    {
        AssemblyReference asmRef = null;

        while (type != null)
        {
            type = rec(type, out asmRef);
        }

        return asmRef;

        ITypeDescriptor rec(ITypeDescriptor ftype, out AssemblyReference output)
        {
            IResolutionScope rs = ftype.Scope;
            if (rs is AssemblyReference aref)
            {
                output = aref;
                return null;
            }

            if (rs is ModuleDefinition mdef)
            {
                output = new AssemblyReference(mdef.Assembly);
                return null;
            }

            output = null;
            return ftype.DeclaringType;
        }
    }
}
