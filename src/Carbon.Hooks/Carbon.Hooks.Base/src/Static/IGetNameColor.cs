using API.Hooks;
using ConVar;
using HarmonyLib;
using JetBrains.Annotations;
using Patch = API.Hooks.Patch;

namespace Carbon.Hooks;

public partial class Category_Static
{
#if !MINIMAL
	public class Static_Chat
	{
		[HookAttribute.Patch(
				"IGetNameColor", "IGetNameColor",
				typeof(Chat), nameof(Chat.GetNameColor),
				[typeof(ulong), typeof(BasePlayer)]),
		]
		[HookAttribute.Options(HookFlags.Static | HookFlags.Hidden | HookFlags.IgnoreChecksum)]
		public class IGetNameColor : Patch
		{
			private const string UserColor = "#5af";
			private const string AdminColor = "#af5";
			private const string DevColor = "#fa5";

			[UsedImplicitly]
			[HarmonyPostfix]
			public static void Postfix(ref string __result)
			{
				if (string.IsNullOrEmpty(__result))
					return;

				if (Community.Runtime.Core.NoAdminChatColorCache && __result.ToLower() == AdminColor)
				{
					__result = UserColor;
					return;
				}

				if (Community.Runtime.Core.NoDevChatColorCache && __result.ToLower() == DevColor)
					__result = UserColor;
			}
		}
	}
#endif
}
