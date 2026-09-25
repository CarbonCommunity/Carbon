using System;

namespace Carbon;

/*
 * Packages are optional component assemblies in carbon/managed/packages. They load before Carbon.dll,
 * and can declare the following assembly attributes, which Carbon.Startup reads (without loading the
 * package) before Rust itself loads.
 */

/// <summary>
/// Adds a public, non-serialized field to a Rust type while the server assembly gets patched,
/// e.g. to attach package data to BasePlayer.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class InjectFieldAttribute : Attribute
{
	public string TargetType { get; }
	public string FieldName { get; }
	public Type FieldType { get; }

	public InjectFieldAttribute(string targetType, string fieldName, Type fieldType)
	{
		TargetType = targetType;
		FieldName = fieldName;
		FieldType = fieldType;
	}
}

public enum StartupAction
{
	/// <summary>Deletes the file or folder at the path.</summary>
	Delete,

	/// <summary>Moves files in the path folder whose name contains the filter into the target folder.</summary>
	MoveMatching,

	/// <summary>Copies the path folder into the target folder, only on a fresh install (every copy target empty).</summary>
	CopyIfEmpty
}

/// <summary>
/// A file operation Carbon.Startup performs before Rust loads. Paths accept the tokens
/// {rust} (server root), {rust_managed}, {carbon} and {extensions}.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class StartupTaskAttribute : Attribute
{
	public StartupAction Action { get; }
	public string Path { get; }
	public string Target { get; set; }
	public string Filter { get; set; }

	public StartupTaskAttribute(StartupAction action, string path)
	{
		Action = action;
		Path = path;
	}
}
