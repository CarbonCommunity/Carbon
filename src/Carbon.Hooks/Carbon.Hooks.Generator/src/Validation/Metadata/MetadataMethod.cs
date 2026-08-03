using System.Reflection;
using System.Reflection.Metadata;

namespace Carbon.Validation.Metadata;

/// <summary>
///     A method definition with lazily decoded signature, body and MSIL hash.
/// </summary>
internal sealed class MetadataMethod
{
	private readonly MetadataAssembly _assembly;
	private readonly MethodDefinitionHandle _handle;
	private readonly Lazy<MetadataGenericContext> _genericContext;
	private readonly Lazy<MethodSignature<string>> _signature;
	private readonly Lazy<string[]> _parameterNames;
	private readonly Lazy<BodyData?> _body;
	private readonly Lazy<string?> _msilHash;

	private sealed record BodyData(IlInstruction[] Instructions, int LocalCount);

	public MetadataMethod(MetadataAssembly assembly, MethodDefinitionHandle handle)
	{
		_assembly = assembly;
		_handle = handle;

		var definition = assembly.Reader.GetMethodDefinition(handle);
		Name = assembly.Reader.GetString(definition.Name);
		Attributes = definition.Attributes;
		DeclaringTypeName = PatcherSignatureProvider.GetTypeDefinitionName(assembly.Reader, definition.GetDeclaringType());

		_genericContext = new Lazy<MetadataGenericContext>(() => assembly.GetGenericContext(handle));
		_signature = new Lazy<MethodSignature<string>>(() => assembly.DecodeMethodSignature(handle, _genericContext.Value));
		_parameterNames = new Lazy<string[]>(ReadParameterNames);
		_body = new Lazy<BodyData?>(DecodeBody);
		_msilHash = new Lazy<string?>(() => Instructions == null ? null : Validation.MsilHash.Compute(Instructions));
	}

	public string Name { get; }

	public string DeclaringTypeName { get; }

	public MethodAttributes Attributes { get; }

	public int GenericParameterCount => _genericContext.Value.MethodParameters.Length;

	public bool IsStatic => (Attributes & MethodAttributes.Static) != 0;

	public string ReturnType => _signature.Value.ReturnType;

	public string[] ParameterTypes => [.. _signature.Value.ParameterTypes];

	/// <summary>Parameter names by position, excluding 'this'. Unnamed parameters yield empty strings.</summary>
	public string[] ParameterNames => _parameterNames.Value;

	/// <summary>The decoded body, or null when the method has none (abstract/extern).</summary>
	public IlInstruction[]? Instructions => _body.Value?.Instructions;

	public int LocalCount => _body.Value?.LocalCount ?? 0;

	/// <summary>
	///     The Oxide-patcher-compatible MSIL hash of the body, or null when the method has no body.
	/// </summary>
	public string? MsilHash => _msilHash.Value;

	public string RenderSignature()
	{
		return $"{ReturnType} {DeclaringTypeName}::{Name}({string.Join(",", _signature.Value.ParameterTypes)})";
	}

	private string[] ReadParameterNames()
	{
		var definition = _assembly.Reader.GetMethodDefinition(_handle);
		var names = new string[_signature.Value.ParameterTypes.Length];
		Array.Fill(names, string.Empty);

		foreach (var handle in definition.GetParameters())
		{
			var parameter = _assembly.Reader.GetParameter(handle);
			// Sequence 0 describes the return value; parameters are 1-based.
			if (parameter.SequenceNumber >= 1 && parameter.SequenceNumber <= names.Length)
			{
				names[parameter.SequenceNumber - 1] = parameter.Name.IsNil ? string.Empty : _assembly.Reader.GetString(parameter.Name);
			}
		}

		return names;
	}

	private BodyData? DecodeBody()
	{
		try
		{
			var definition = _assembly.Reader.GetMethodDefinition(_handle);
			var block = _assembly.GetMethodBody(definition.RelativeVirtualAddress);
			var bytes = block?.GetILBytes();
			if (block == null || bytes == null)
			{
				return null;
			}

			var localCount = 0;
			if (!block.LocalSignature.IsNil)
			{
				localCount = _assembly.DecodeLocalSignature(block.LocalSignature, _genericContext.Value).Length;
			}

			return new BodyData(IlDecoder.Decode(bytes, this), localCount);
		}
		catch (Exception ex)
		{
			Utility.Logger.Warning($"failed to decode body of {DeclaringTypeName}::{Name}: {ex.Message}");
			return null;
		}
	}

	internal string RenderTokenOperand(int token)
	{
		return _assembly.RenderToken(token, _genericContext.Value);
	}

	internal string RenderUserString(int token)
	{
		try
		{
			return _assembly.Reader.GetUserString(System.Reflection.Metadata.Ecma335.MetadataTokens.UserStringHandle(token));
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	/// <summary>
	///     Renders a ldarg/starg operand the way Cecil does: the parameter's name, where slot 0 is
	///     'this' for instance methods (rendered as an empty string, like Cecil's ThisParameter).
	/// </summary>
	internal string RenderArgumentOperand(int slot)
	{
		var index = IsStatic ? slot : slot - 1;
		if (index < 0 || index >= ParameterNames.Length)
		{
			return string.Empty;
		}

		return ParameterNames[index];
	}
}
