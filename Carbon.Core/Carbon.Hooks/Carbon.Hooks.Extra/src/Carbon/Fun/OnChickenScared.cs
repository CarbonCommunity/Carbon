using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Fun
{
	public partial class Fun_AnimalBrain
	{
		[HookAttribute.Patch("OnChickenScared", "OnChickenScared", typeof(AnimalBrain.FleeState), "StateEnter", new System.Type[] { typeof(BaseAIBrain), typeof(BaseEntity) })]
		[HookAttribute.Identifier("4af63eb71cfc44f7a66cb1c16974a5c7")]

		[MetadataAttribute.Info("Gets called whenever a chicken gets scared due to the presence of another potential threat.")]
		[MetadataAttribute.Parameter("chicken", typeof(Chicken))]
		[MetadataAttribute.Parameter("threat", typeof(BaseEntity))]

		public class Fun_AnimalBrain_4af63eb71cfc44f7a66cb1c16974a5c7 : Patch
		{
			public static void Postfix(BaseAIBrain brain)
			{
				try
				{
					if (brain != null && brain.baseEntity is Chicken chicken)
					{
						HookCaller.CallStaticHook(3244860512, chicken,
							brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot));
					}
				}
				catch { }
			}
		}
	}
}
