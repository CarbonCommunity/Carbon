using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Hooks;
using Carbon.Hooks;

namespace Carbon.Contracts
{
	public interface IHook
	{
		string HookFullName { get; }
		string HookName { get; }
		string Identifier { get; }
		bool IsStaticHook { get; }
		bool IsPatch { get; }
		bool IsHidden { get; }
		bool IsInstalled { get; }
		HookState Status { get; }
	}
}
