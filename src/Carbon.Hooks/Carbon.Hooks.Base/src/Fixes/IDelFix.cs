using System;
using System.Linq;
using API.Hooks;
using Facepunch;
using Facepunch.Extend;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Fixes
{
	public partial class Fixes_FixDel
	{
		[HookAttribute.Patch("IDelFix", "IDelFix", typeof(ConVar.Hierarchy), "del", [typeof(ConsoleSystem.Arg)])]
		[HookAttribute.Options(HookFlags.Hidden | HookFlags.Static | HookFlags.IgnoreChecksum)]

		public class IDelFix : Patch
		{
			public static bool Prefix(ConsoleSystem.Arg args)
			{
				if (!args.HasArgs())
				{
					return false;
				}

				var count = 0;
				var invalidEntities = 0;
				var failedEntities = 0;
				var fullString = args.GetString(0);

				using var entities = Pool.Get<PooledList<BaseNetworkable>>();
				foreach (var entity in BaseEntity.serverEntities)
				{
					try
					{
						if (entity == null || !(entity.PrefabName.Contains(fullString, StringComparison.InvariantCultureIgnoreCase) ||
						                        entity.GetType().Name.Equals(fullString, StringComparison.InvariantCultureIgnoreCase)))
						{
							continue;
						}

						if (entity.IsValid())
						{
							if (entity is BasePlayer { IsConnected: true })
							{
								continue;
							}

							count++;
							entities.Add(entity);
						}
						else
						{
							invalidEntities++;
						}
					}
					catch (Exception ex)
					{
						Logger.Error($"Failed destroying '{entity}'", ex);
						failedEntities++;
					}
				}
				for(int i = 0; i < entities.Count; i++)
				{
					entities[i].Kill();
				}
				args.ReplyWith($"Deleted {count:n0} entities (found {invalidEntities:n0} invalid, {failedEntities:n0} failed).");
				return false;
			}
		}
	}
}
