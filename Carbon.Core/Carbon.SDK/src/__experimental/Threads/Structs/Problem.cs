using Microsoft.CodeAnalysis;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Threads;

public struct Problem
{
	public DiagnosticSeverity Severity;
	public int Column, Row;
	public string Description;
	public string File, ErrorID;
}
