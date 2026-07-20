using API.Hooks;
using Carbon.Core;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Entity
{
	public partial class Entity_Hooks
	{
		[HookAttribute.Patch("OnEntityTakeDamage", "OnEntityTakeDamage", typeof(CorePlugin), "IOnBasePlayerAttacked")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Entity")]
		[MetadataAttribute.Info("Called when a player gets attacked.")]
		[MetadataAttribute.Parameter("entity", typeof(BaseCombatEntity))]
		[MetadataAttribute.Parameter("info", typeof(HitInfo))]
		[MetadataAttribute.Return(typeof(bool))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class OnEntityTakeDamage : Patch;
	}
}
