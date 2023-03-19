using API.Hooks;

namespace API.Hooks
{
	public interface IHook
	{
		bool IsHidden { get; }
		bool IsInstalled { get; }
		bool IsPatch { get; }
		bool IsStaticHook { get; }

		HookState Status { get; }

		string HookFullName { get; }
		string HookName { get; }
		string Identifier { get; }
	}
}
