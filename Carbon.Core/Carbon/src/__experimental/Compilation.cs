using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using API.Threads;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Threads;

public class Compilation : IThreadedJob
{
	private static readonly List<MetadataReference> _references = new();

	private static readonly CSharpParseOptions _parse = new(
		languageVersion: LanguageVersion.Latest, preprocessorSymbols: new string[] { });

	private static readonly CSharpCompilationOptions _compilation = new(
		warningLevel: 4,
		deterministic: true,
		optimizationLevel: OptimizationLevel.Release,
		outputKind: OutputKind.DynamicallyLinkedLibrary
	);


	public string FileName { get; set; }
	public string Source { get; set; }
	public byte[] Output { get; private set; }

	public bool Result { get; private set; }
	public bool IsDone { get; private set; }

	private List<Diagnostic> _diagnostics = null;
	public List<Diagnostic> Diagnostics { get => _diagnostics; }

	static Compilation()
	{
		foreach (string reference in Carbon.Community.Runtime.AssemblyEx.RefWhitelist)
		{
			try
			{
				byte[] raw = Carbon.Community.Runtime.AssemblyEx.Read(reference)
					?? throw new Exception();

				using MemoryStream stream = new(raw);
				_references.Add(MetadataReference.CreateFromStream(stream));
			}
			catch (System.Exception)
			{
				Carbon.Logger.Warn($"Error loading common reference '{reference}'");
				continue;
			}
		}

		foreach (string extension in Carbon.Community.Runtime.AssemblyEx.Extensions.Loaded)
		{
			try
			{
				byte[] raw = Carbon.Community.Runtime.AssemblyEx.Read(extension)
					?? throw new Exception();

				using MemoryStream stream = new(raw);
				_references.Add(MetadataReference.CreateFromStream(stream));
			}
			catch (System.Exception)
			{
				Carbon.Logger.Warn($"Error loading extension reference '{extension}'");
				continue;
			}
		}
	}

	public void Initialize()
	{
		//if (!File.Exists(Path.Combine(Context.Carbon, "test", FileName))) throw new Exception();
		//Source = File.ReadAllText(Path.Combine(Context.Carbon, "test", FileName));
	}

	public void Process()
	{
		List<SyntaxTree> tree = new() {
			CSharpSyntaxTree.ParseText(Source, _parse)
		};

		CSharpCompilation compilation = CSharpCompilation.Create(
			$"Plugin.{Guid.NewGuid():N}", tree, _references, _compilation);

		using MemoryStream stream = new MemoryStream();
		EmitResult ilcode = compilation.Emit(stream);
		_diagnostics = ilcode.Diagnostics.ToList();

		if (!ilcode.Success) return;
		Output = stream.ToArray();

		Result = true;
		IsDone = true;
	}
}