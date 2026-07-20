using API.Hooks;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Fun
{
	public partial class Fun_AnimalBrain
	{
		[HookAttribute.Patch("OnChickenScared", "OnChickenScared", typeof(AnimalBrain.FleeState), "StateEnter", new System.Type[] { typeof(BaseAIBrain), typeof(BaseEntity) })]

		[MetadataAttribute.Info("Gets called whenever a chicken gets scared due to the presence of another potential threat.")]
		[MetadataAttribute.Parameter("chicken", typeof(Chicken))]
		[MetadataAttribute.Parameter("threat", typeof(BaseEntity))]

		public class OnChickenScared : Patch
		{
			public static void Postfix(BaseAIBrain brain)
			{
				try
				{
					if (brain != null && brain.baseEntity is Chicken chicken)
					{
						HookCaller.CallStaticHook(1990146717, chicken,
							brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot));
					}
				}
				catch { }
			}
		}
	}
}
