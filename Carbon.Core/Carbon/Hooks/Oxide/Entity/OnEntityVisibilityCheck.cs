///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using Carbon.Core;
using Oxide.Core;

namespace Carbon.Extended
{
	[OxideHook("OnEntityVisibilityCheck", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Network)]
	[OxideHook.Parameter("entity", typeof(BaseEntity))]
	[OxideHook.Parameter("observer", typeof(BasePlayer))]
	[OxideHook.Parameter("id", typeof(uint))]
	[OxideHook.Parameter("debugName", typeof(string))]
	[OxideHook.Parameter("maxDistance", typeof(float))]
	[OxideHook.Info("Called when a networked entity checks for player visibility.")]
	[OxideHook.Patch(typeof(BaseEntity.RPC_Server.IsVisible), "Test", typeof(uint), typeof(string), typeof(BaseEntity), typeof(BasePlayer), typeof(float))]
	public class BaseEntity_IsVisible_Test
	{
		public static bool Prefix(uint id, string debugName, BaseEntity ent, BasePlayer player, float maximumDistance, ref bool __result)
		{
			if (ent == null || player == null)
			{
				return false;
			}

			var obj = Interface.CallHook("OnEntityVisibilityCheck", ent, player, id, debugName, maximumDistance);

			if (obj is bool)
			{
				__result = (bool)obj;
				return false;
			}

			return true;
		}
	}
}
