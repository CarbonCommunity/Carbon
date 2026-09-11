using API.Hooks;
using Carbon.Core;
using Network;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnWireClear", "OnWireClear", "WireTool", "AttemptClearSlot", ["BaseNetworkable", "BasePlayer", "System.Int32", "System.Boolean"])]
		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Gets called when a player attempts to clear an IO slot.")]
		[MetadataAttribute.Parameter("player", "BasePlayer")]
		[MetadataAttribute.Parameter("ioA", "IOEntity")]
		[MetadataAttribute.Parameter("clearIndex", typeof(int))]
		[MetadataAttribute.Parameter("ioB", "IOEntity")]
		[MetadataAttribute.Parameter("isInput", typeof(bool))]
		[MetadataAttribute.Return(typeof(bool))]
		[MetadataAttribute.OxideCompatible]

		public class OnWireClear : Patch
		{
			public static bool Prefix(ref bool __result, BaseNetworkable clearEnt, BasePlayer ply, int clearIndex, bool isInput)
			{
				if (clearEnt is not IOEntity ioEntity)
				{
					return true;
				}

				var connectedTo = (isInput ? ioEntity.inputs[clearIndex] : ioEntity.outputs[clearIndex]).connectedTo.Get();
				if (!connectedTo.IsValid())
				{
					return true;
				}

				// OnWireClear
				if (HookCaller.CallStaticHook(1879512085, ply, ioEntity, clearIndex, connectedTo, isInput) is bool resultValue)
				{
					__result = resultValue;
					return false;
				}

				return true;
			}
		}
	}
}
