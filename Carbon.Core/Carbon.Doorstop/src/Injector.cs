using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Mono.Cecil;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;
using PropertyAttributes = Mono.Cecil.PropertyAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace Carbon.Utility;

internal static class Injector
{
	public static void Inject(ModuleDefinition module)
	{
		var covalence = AssemblyDefinition.ReadAssembly(new MemoryStream(File.ReadAllBytes(Path.Combine(Context.Modules, "Carbon.Oxide.Covalence.dll"))));
		module.AssemblyReferences.Add(covalence.Name);

		var iplayer = covalence.MainModule.GetType("Oxide.Core.Libraries.Covalence", "IPlayer");
		module.ModuleReferences.Add(covalence.MainModule);

		module.GetType("BasePlayer").Fields.Add(new FieldDefinition("IPlayer", FieldAttributes.Public, module.ImportReference(iplayer)));
	}

	public class PatcherAssemblyResolver : DefaultAssemblyResolver
	{
		public PatcherAssemblyResolver(string path)
		{
			if (path == null) throw new ArgumentNullException("path");
			if (!Directory.Exists(path)) throw new DirectoryNotFoundException("Directory not found: " + path);
			AddSearchDirectory(path);
		}

		public override AssemblyDefinition Resolve(AssemblyNameReference assemblyName)
		{
			return base.Resolve(assemblyName);
		}
	}

	internal static TypeDefinition CreateCommandType(ModuleDefinition md)
	{
		var type = CreateType(md, "CommandType", TypeAttributes.Sealed, out var properties, out var methods, out var fields, "Oxide.Core.Libraries.Covalence", md.ImportReference(typeof(System.Enum)));
		fields.Add(new FieldDefinition("value__", FieldAttributes.SpecialName | FieldAttributes.RTSpecialName, md.ImportReference(typeof(int))));
		fields.Add(new FieldDefinition("Chat", FieldAttributes.Static | FieldAttributes.Literal, type) { Constant = 0 });
		fields.Add(new FieldDefinition("Console", FieldAttributes.Static | FieldAttributes.Literal, type) { Constant = 1 });

		md.Types.Add(type);
		return type;
	}
	internal static TypeDefinition CreateIPlayer(ModuleDefinition md, TypeDefinition commandType)
	{
		var parameters = (Mono.Collections.Generic.Collection<ParameterDefinition>)null;

		var type = CreateType(md, "IPlayer", TypeAttributes.Interface, out var properties, out var methods, out var fields, null, null);
		properties.Add(CreateProperty("Object", typeof(object), ref type, ref md));
		properties.Add(CreateProperty("LastCommand", commandType, ref type, ref md));
		properties.Add(CreateProperty("Name", typeof(string), ref type, ref md));
		properties.Add(CreateProperty("Id", typeof(string), ref type, ref md));
		properties.Add(CreateProperty("Address", typeof(string), ref type, ref md));
		properties.Add(CreateProperty("Ping", typeof(int), ref type, ref md));
		properties.Add(CreateProperty("Language", typeof(CultureInfo), ref type, ref md));
		properties.Add(CreateProperty("IsConnected", typeof(bool), ref type, ref md));
		properties.Add(CreateProperty("IsSleeping", typeof(bool), ref type, ref md));
		properties.Add(CreateProperty("IsServer", typeof(bool), ref type, ref md));
		properties.Add(CreateProperty("IsAdmin", typeof(bool), ref type, ref md));
		properties.Add(CreateProperty("IsBanned", typeof(bool), ref type, ref md));
		properties.Add(CreateProperty("BanTimeRemaining", typeof(TimeSpan), ref type, ref md));
		properties.Add(CreateProperty("Health", typeof(float), ref type, ref md));
		properties.Add(CreateProperty("MaxHealth", typeof(float), ref type, ref md));

		methods.Add(CreateMethod("Ban", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("reason", ParameterAttributes.None, md.ImportReference(typeof(string))));
		parameters.Add(new ParameterDefinition("duration", ParameterAttributes.HasDefault, md.ImportReference(typeof(TimeSpan))));

		methods.Add(CreateMethod("Heal", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("amount", ParameterAttributes.None, md.ImportReference(typeof(float))));

		methods.Add(CreateMethod("Hurt", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("amount", ParameterAttributes.None, md.ImportReference(typeof(float))));

		methods.Add(CreateMethod("Kick", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("reason", ParameterAttributes.None, md.ImportReference(typeof(string))));

		methods.Add(CreateMethod("Kill", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));

		methods.Add(CreateMethod("Rename", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, md.ImportReference(typeof(string))));

		methods.Add(CreateMethod("Teleport", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("x", ParameterAttributes.None, md.ImportReference(typeof(float))));
		parameters.Add(new ParameterDefinition("y", ParameterAttributes.None, md.ImportReference(typeof(float))));
		parameters.Add(new ParameterDefinition("z", ParameterAttributes.None, md.ImportReference(typeof(float))));

		methods.Add(CreateMethod("Unban", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));

		methods.Add(CreateMethod("Position", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("x", ParameterAttributes.Out, md.ImportReference(typeof(float))));
		parameters.Add(new ParameterDefinition("y", ParameterAttributes.Out, md.ImportReference(typeof(float))));
		parameters.Add(new ParameterDefinition("z", ParameterAttributes.Out, md.ImportReference(typeof(float))));

		var args = new ParameterDefinition("args", ParameterAttributes.None, md.ImportReference(typeof(object[]))) { CustomAttributes = { new CustomAttribute(md.ImportReference(typeof(ParamArrayAttribute).GetConstructor(Type.EmptyTypes))) } };
	
		methods.Add(CreateMethod("Message", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("message", ParameterAttributes.None, md.ImportReference(typeof(string))));
		parameters.Add(new ParameterDefinition("prefix", ParameterAttributes.None, md.ImportReference(typeof(string))));
		parameters.Add(args);

		methods.Add(CreateMethod("Message", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("message", ParameterAttributes.None, md.ImportReference(typeof(string))));

		methods.Add(CreateMethod("Reply", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("message", ParameterAttributes.None, md.ImportReference(typeof(string))));
		parameters.Add(new ParameterDefinition("prefix", ParameterAttributes.None, md.ImportReference(typeof(string))));
		parameters.Add(args);

		methods.Add(CreateMethod("Reply", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("message", ParameterAttributes.None, md.ImportReference(typeof(string))));

		methods.Add(CreateMethod("Command", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("command", ParameterAttributes.None, md.ImportReference(typeof(string))));
		parameters.Add(args);

		methods.Add(CreateMethod("HasPermission", typeof(bool), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("perm", ParameterAttributes.None, md.ImportReference(typeof(string))));

		methods.Add(CreateMethod("GrantPermission", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("perm", ParameterAttributes.None, md.ImportReference(typeof(string))));

		methods.Add(CreateMethod("RevokePermission", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("perm", ParameterAttributes.None, md.ImportReference(typeof(string))));

		methods.Add(CreateMethod("BelongsToGroup", typeof(bool), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("group", ParameterAttributes.None, md.ImportReference(typeof(string))));

		methods.Add(CreateMethod("AddToGroup", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("group", ParameterAttributes.None, md.ImportReference(typeof(string))));

		methods.Add(CreateMethod("RemoveFromGroup", typeof(void), MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Abstract, out parameters, ref md));
		parameters.Add(new ParameterDefinition("group", ParameterAttributes.None, md.ImportReference(typeof(string))));

		md.Types.Add(type);
		return type;
	}
	internal static TypeDefinition CreateType(ModuleDefinition module, string name, TypeAttributes attribute,
		out Mono.Collections.Generic.Collection<PropertyDefinition> properties,
		out Mono.Collections.Generic.Collection<MethodDefinition> methods,
		out Mono.Collections.Generic.Collection<FieldDefinition> fields,
		string @namespace = null,
		TypeReference baseType = null)
	{
		var definition = new TypeDefinition(@namespace, name, attribute);
		properties = definition.Properties;
		methods = definition.Methods;
		fields = definition.Fields;

		if (baseType != null) definition.BaseType = baseType;

		return definition;
	}

	internal static PropertyDefinition CreateProperty(string name, object type, ref TypeDefinition parent, ref ModuleDefinition md)
	{
		var retType = (TypeReference)null;

		switch (type)
		{
			case Type _type:
				retType = md.ImportReference(_type);
				break;

			case TypeReference _ref:
				retType = _ref;
				break;
		}

		var method = new MethodDefinition($"get_{name}", MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot, retType)
		{
			HasThis = true,
			IsGetter = true
		};
		parent.Methods.Add(method);

		var property = new PropertyDefinition(name, PropertyAttributes.None, retType)
		{
			GetMethod = method
		};

		return property;
	}
	internal static MethodDefinition CreateMethod(string name, object type, MethodAttributes attribute, out Mono.Collections.Generic.Collection<ParameterDefinition> parameters, ref ModuleDefinition md)
	{
		var retType = (TypeReference)null;

		switch (type)
		{
			case Type _type:
				retType = md.ImportReference(_type);
				break;

			case TypeReference _ref:
				retType = _ref;
				break;
		}

		var method = new MethodDefinition(name, attribute, retType);
		parameters = method.Parameters;

		return method;
	}
}
