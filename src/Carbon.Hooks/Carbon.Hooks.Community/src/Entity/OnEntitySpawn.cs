using API.Hooks;

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Entity
{
	public partial class Entity_BaseNetworkable
	{
		[HookAttribute.Patch("OnEntitySpawn", "OnEntitySpawn", typeof(BaseNetworkable), "Spawn", new System.Type[] { })]

		[MetadataAttribute.Info("Called before any networked entity has spawned (including trees).")]
		[MetadataAttribute.Parameter("networkable", typeof(BaseNetworkable))]

		public class OnEntitySpawn : Patch
		{
			public static void Prefix(ref BaseNetworkable __instance)
				=> HookCaller.CallStaticHook(4136550545, __instance);
		}
	}
}
