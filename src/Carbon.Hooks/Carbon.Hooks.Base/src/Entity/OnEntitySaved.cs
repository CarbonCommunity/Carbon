using API.Hooks;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Entity
{
	public partial class Entity_Outdated
	{
		[HookAttribute.Patch("OnEntitySaved", "OnEntitySaved", typeof(BaseNetworkable), nameof(BaseNetworkable.ToStream), null)]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Info("Gets called whenever the entity is about to be saved for streaming.")]
		[MetadataAttribute.Parameter("networkable", typeof(BaseNetworkable))]
		[MetadataAttribute.Parameter("info", typeof(BaseNetworkable.SaveInfo))]
		[MetadataAttribute.OxideCompatible]

		public class OnEntitySaved : Patch;
	}
}
