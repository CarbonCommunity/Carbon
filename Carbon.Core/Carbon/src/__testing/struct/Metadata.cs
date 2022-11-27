
using System;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public class Metadata
{
	public Metadata(string name, Type target, string method, Type[] args = null)
	{
		HookName = name;
		Identifier = $"{Guid.NewGuid():N}";
		TargetMethod = method;
		TargetMethodArgs = args;
		TargetType = target;
	}


	public void SetIdentifier(string identifier)
		=> Identifier = identifier;

	public void SetChecksum(string checksum)
		=> Checksum = checksum;

	public void SetDependencies(string[] depedencies)
		=> DependsOn = depedencies;

	public void SetAlwaysPatch(bool value)
		=> AlwaysApplyPatch = value;

	public void SetHidden(bool value)
		=> HideFromListings = value;


	internal string HookName
	{ get; private set; }

	internal string TargetMethod
	{ get; private set; }

	internal Type TargetType
	{ get; private set; }

	internal Type[] TargetMethodArgs
	{ get; private set; }

	internal string Identifier
	{ get; private set; }

	internal string Checksum
	{ get; private set; }

	internal string[] DependsOn
	{ get; private set; }

	internal bool AlwaysApplyPatch
	{ get; private set; }

	internal bool HideFromListings
	{ get; private set; }
}