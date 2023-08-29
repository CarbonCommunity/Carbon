using System;
using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Fixes
{
	public partial class Fixes_FixDel
	{
		[HookAttribute.Patch("IDelFix", "IDelFix", typeof(ConVar.Hierarchy), "del", new System.Type[] { typeof(ConsoleSystem.Arg) })]
		[HookAttribute.Identifier("877fdf4a8e0a44abbf15701db6641672")]
		[HookAttribute.Options(HookFlags.Hidden | HookFlags.Static | HookFlags.IgnoreChecksum)]

		public class Fixes_Del_877fdf4a8e0a44abbf15701db6641672 : Patch
		{
			public static bool Prefix(ConsoleSystem.Arg args)
			{
				if (!args.HasArgs())
				{
					return false;
				}

				using var pool = Entities.GetAll(inherited: true);

				var count = 0;
				var invalidEntities = 0;
				var fullString = args.FullString.ToLower();

				pool.Each(entity =>
				{
					if (entity.name.ToLower() == fullString || entity.name.StartsWith(fullString, StringComparison.CurrentCultureIgnoreCase) || entity.GetType().Name.ToLower() == fullString)
					{
						if (entity.IsValid())
						{
							if(entity is BasePlayer player)
							{
								if (player.IsConnected)
								{
									return;
								}
							}

							count++;
							entity.Kill();
						}
						else
						{
							invalidEntities++;
						}
					}
				});

				args.ReplyWith($"Deleted {count:n0} entities (found {invalidEntities:n0} invalid).");
				return false;
			}
		}
	}
}
