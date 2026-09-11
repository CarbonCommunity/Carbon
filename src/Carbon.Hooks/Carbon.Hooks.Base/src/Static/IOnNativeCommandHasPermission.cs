using API.Hooks;

namespace Carbon.Hooks;

public partial class Category_Server
{
	public partial class Server_ConsoleSystem
	{
		[HookAttribute.Patch("OnNativeCommandHasPermission", "OnNativeCommandHasPermission", typeof(ConsoleSystem.Arg), "HasPermission", new System.Type[] { })]
		[HookAttribute.Options(HookFlags.None)]

		[MetadataAttribute.Info("Overrides Rust's native checks for command execution authorization.")]
		[MetadataAttribute.Info("Example: allow `inventory.give` to be executed by regular players.")]
		[MetadataAttribute.Parameter("arg", typeof(ConsoleSystem.Arg))]
		[MetadataAttribute.Return(typeof(bool))]

		public class IOnNativeCommandHasPermission : Patch
		{
			public static bool Prefix(ref ConsoleSystem.Arg __instance, ref bool __result)
			{
				if (HookCaller.CallStaticHook(938254961, __instance) is bool value)
				{
					__result = value;
					return false;
				}

				return true;
			}
		}
	}
}
