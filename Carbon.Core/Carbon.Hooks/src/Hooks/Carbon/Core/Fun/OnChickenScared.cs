
/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Fun
{
	public partial class Fun_AnimalBrain
	{
		/*
		[CarbonHook("OnChickenScared"), CarbonHook.Category(Hook.Category.Enum.Entity)]
		[CarbonHook.Parameter("this", typeof(Chicken))]
		[CarbonHook.Parameter("threat", typeof(BaseEntity))]
		[CarbonHook.Info("Gets triggered when a chicken gets scared by something.")]
		[CarbonHook.Patch(typeof(AnimalBrain.FleeState), "StateEnter")]
		*/

		public class Fun_AnimalBrain_FleeState_StateEnter_4af63eb71cfc44f7a66cb1c16974a5c7
		{
			public static Metadata metadata = new Metadata("OnChickenScared",
				typeof(AnimalBrain.FleeState), "StateEnter", new System.Type[] { typeof(BaseAIBrain), typeof(BaseEntity) });

			static Fun_AnimalBrain_FleeState_StateEnter_4af63eb71cfc44f7a66cb1c16974a5c7()
			{
				metadata.SetIdentifier("4af63eb71cfc44f7a66cb1c16974a5c7");
			}

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
}
