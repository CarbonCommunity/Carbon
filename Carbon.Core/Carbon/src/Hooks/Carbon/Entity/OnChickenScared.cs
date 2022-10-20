///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System.Linq;
using Carbon.Core;

namespace Carbon.Hooks
{
	[CarbonHook("OnChickenScared"), CarbonHook.Category(Hook.Category.Enum.Entity)]
	[CarbonHook.Parameter("this", typeof(Chicken))]
	[CarbonHook.Parameter("threat", typeof(BaseEntity))]
	[CarbonHook.Info("Gets triggered when a chicken gets scared by something.")]
	[CarbonHook.Patch(typeof(AnimalBrain.FleeState), "StateEnter")]
	public class OnChickenScared
	{
		public static void Postfix(BaseAIBrain brain, BaseEntity entity)
		{
			try
			{
				if (brain != null && brain.baseEntity is Chicken chicken)
				{
					HookCaller.CallStaticHook("OnChickenScared", chicken, brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot));
				}
			}
			catch { }
		}
	}
}
