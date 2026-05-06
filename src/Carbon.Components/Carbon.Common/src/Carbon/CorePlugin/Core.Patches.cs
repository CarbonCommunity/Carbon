using HarmonyLib;

namespace Carbon.Core;

public partial class CorePlugin
{
	[AutoPatch(Silent = true), HarmonyPatch(typeof(BaseEntity), nameof(BaseEntity.Save))]
	public class BaseEntity_Save
	{
		public static void Postfix(BaseEntity __instance, BaseNetworkable.SaveInfo info)
		{
			if (!info.forDisk)
			{
				return;
			}
			if (!__instance.networkEntityScale)
			{
				return;
			}
			info.msg.baseEntity.scale = __instance.transform.localScale;
		}
	}

	[AutoPatch(Silent = true), HarmonyPatch(typeof(BaseEntity), nameof(BaseEntity.Load))]
	public class BaseEntity_Load
	{
		public static void Prefix(BaseEntity __instance, BaseNetworkable.LoadInfo info)
		{
			if (!info.fromDisk)
			{
				return;
			}
			if (info.msg.baseEntity == null)
			{
				return;
			}
			if (info.msg.baseEntity.scale == default)
			{
				return;
			}
			__instance.transform.localScale = info.msg.baseEntity.scale;
			__instance.networkEntityScale = true;
		}
	}
}
