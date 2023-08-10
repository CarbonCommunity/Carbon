using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using API.Threads;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Threads;

public class CompilationJob : IThreadJob
{
	private bool _processing = true;
	public bool IsDone { get => !_processing; }
	public bool IsProcessing { get => _processing; }


	public SynchronizationContext MainThreadContext { get; set; }
	public Action<Result> Callback { get; set; }
	public object Input { get; set; }

	//private string _contents = null;
	private Result _result = new();


	private static readonly List<MetadataReference> _references = new();

	private static readonly CSharpParseOptions _parse = new(
		languageVersion: LanguageVersion.Latest, preprocessorSymbols: new string[] { });

	private static readonly CSharpCompilationOptions _compilation = new(
		warningLevel: 4,
		deterministic: true,
		optimizationLevel: OptimizationLevel.Release,
		outputKind: OutputKind.DynamicallyLinkedLibrary
	);

	static CompilationJob()
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

		foreach (var extension in Carbon.Bootstrap.AssemblyEx.Extensions.Loaded)
		{
			try
			{
				byte[] raw = Carbon.Bootstrap.AssemblyEx.Read(extension.Value)
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

	public void Awake()
	{
		// do nothing
	}

	public Result DoWork()
	{
		List<SyntaxTree> tree = new() {
			CSharpSyntaxTree.ParseText((string)Input, _parse)
		};

		CSharpCompilation compilation = CSharpCompilation.Create(
			$"Plugin.{Guid.NewGuid():N}", tree, _references, _compilation);

		using MemoryStream stream = new MemoryStream();
		EmitResult ilcode = compilation.Emit(stream);

		foreach (Diagnostic diagnostic in ilcode.Diagnostics)
		{
			LinePositionSpan span = diagnostic.Location.GetMappedLineSpan().Span;

			_result.Diagnostics.Add(new Problem
			{
				Column = span.Start.Character + 1,
				Description = diagnostic.GetMessage(),
				ErrorID = diagnostic.Id,
				File = (string)Input,
				Row = span.Start.Line + 1,
				Severity = diagnostic.Severity,
			});
		}

		if (ilcode.Success)
		{
			_result.Output = stream.ToArray();
			_processing = false;
		}

		return _result;
	}

	public void Destroy()
	{

	}

	public override string ToString()
	{
		return "Script complier ({FileName})";
	}
}
