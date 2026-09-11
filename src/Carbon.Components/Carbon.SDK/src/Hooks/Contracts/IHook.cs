using System;
using System.Collections.Generic;
using System.Reflection;

namespace API.Hooks;

public interface IHook
{
	bool IsHidden { get; }
	bool IsInstalled { get; }
	bool IsPatch { get; }
	bool IsStaticHook { get; }

	HookFlags Options { get; set; }
	HookState Status { get; }

	string HookFullName { get; }
	string HookName { get; }
	string Identifier { get; }

	Type TargetType { get; }
	string TargetMethod { get; }
	List<MethodBase> TargetMethods { get; }
}
