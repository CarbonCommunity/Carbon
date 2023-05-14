using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using API.Threads;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Threads;

public class ScriptCompilerJob : IThreadedJob
{
	private bool _processing = true;
	public bool IsDone { get => !_processing; }
	public bool IsProcessing { get => _processing; }


	public byte[] Output { get; private set; }
	public string FileName { get; set; }
	public string Input { get; set; }


	private List<Diagnostic> _diagnostics = new();

	private static readonly List<MetadataReference> _references = new();

	private static readonly CSharpParseOptions _parse = new(
		languageVersion: LanguageVersion.Latest, preprocessorSymbols: new string[] { });

	private static readonly CSharpCompilationOptions _compilation = new(
		warningLevel: 4,
		deterministic: true,
		optimizationLevel: OptimizationLevel.Release,
		outputKind: OutputKind.DynamicallyLinkedLibrary
	);

	static ScriptCompilerJob()
	{
		foreach (string reference in Carbon.Bootstrap.AssemblyEx.RefWhitelist)
		{
			try
			{
				byte[] raw = Carbon.Bootstrap.AssemblyEx.Read(reference)
					?? throw new Exception();

				using MemoryStream stream = new(raw);
				_references.Add(MetadataReference.CreateFromStream(stream));
			}
			catch (System.Exception)
			{
				Logger.Warn($"Error loading common reference '{reference}'");
				continue;
			}
		}

		foreach (string extension in Carbon.Bootstrap.AssemblyEx.Extensions.Loaded)
		{
			try
			{
				byte[] raw = Carbon.Bootstrap.AssemblyEx.Read(extension)
					?? throw new Exception();

				using MemoryStream stream = new(raw);
				_references.Add(MetadataReference.CreateFromStream(stream));
			}
			catch (System.Exception)
			{
				Logger.Warn($"Error loading extension reference '{extension}'");
				continue;
			}
		}
	}

	public void Start()
	{
	}

	public void Awake()
	{
		if (!File.Exists(Path.Combine(Context.Carbon, "test", FileName))) throw new Exception();
		Input = File.ReadAllText(Path.Combine(Context.Carbon, "test", FileName));
	}

	public void DoWork()
	{
		List<SyntaxTree> tree = new() {
			CSharpSyntaxTree.ParseText(Input, _parse)
		};

		CSharpCompilation compilation = CSharpCompilation.Create(
			$"Plugin.{Guid.NewGuid():N}", tree, _references, _compilation);

		using MemoryStream stream = new MemoryStream();
		EmitResult ilcode = compilation.Emit(stream);
		_diagnostics = ilcode.Diagnostics.ToList();

		if (ilcode.Success)
		{
			Output = stream.ToArray();
			_processing = false;
		}
	}

	public void Destroy()
	{

	}
}