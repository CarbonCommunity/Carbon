using Facepunch;

namespace Carbon.Documentation;

public static partial class WebRCon
{
	[DocsRpc]
	private static DocsRpcResponse GetPermissionsMetadata(ConsoleSystem.Arg _)
	{
		var permission = Community.Runtime.Core.permission;

		using var plugins = Pool.Get<PooledList<object>>();
		using var modules = Pool.Get<PooledList<object>>();

		foreach (var package in ModLoader.Packages)
		{
			if (package.IsCoreMod)
			{
				continue;
			}

			foreach (var plugin in package.Plugins)
			{
				var permissions = permission.GetPermissions(plugin);

				if (permissions == null || permissions.Length == 0)
				{
					continue;
				}

				plugins.Add(new
				{
					Plugin = new
					{
						Name = plugin.Name,
						Author = plugin.Author
					},
					Permissions = permissions
				});
			}
		}

		foreach (var module in Community.Runtime.ModuleProcessor.Modules)
		{
			var permissions = permission.GetPermissions(module);

			if (permissions == null || permissions.Length == 0)
			{
				continue;
			}

			modules.Add(new
			{
				Module = new
				{
					Name = module.Name
				},
				Permissions = permissions
			});
		}

		return Response(new
		{
			Groups = permission.GetGroups(),
			Permissions = Community.Runtime.Core.permission.GetPermissions(),
			Plugins = plugins.ToArray(),
			Modules = modules.ToArray()
		});
	}

	[DocsRpc]
	private static DocsRpcResponse GetGroupPermissions(ConsoleSystem.Arg arg)
	{
		var group = arg.GetString(1);
		return Response(new
		{
			Permissions = Community.Runtime.Core.permission.GetGroupPermissions(group, true)
		});
	}

	[DocsRpc]
	private static DocsRpcResponse TogglePermission(ConsoleSystem.Arg arg)
	{
		var group = arg.GetString(1);
		var permission = arg.GetString(2);
		var plugin = arg.GetString(3);
		var hookable = ModLoader.FindPlugin(plugin) ?? Community.Runtime.ModuleProcessor.Modules.FirstOrDefault(x => x.Name.Equals(plugin));

		switch (permission)
		{
			case "grantall":
				foreach (var perm in Community.Runtime.Core.permission.GetPermissions(hookable))
				{
					Community.Runtime.Core.permission.GrantGroupPermission(group, perm, null);
				}
				return Response();
			case "revokeall":
				foreach (var perm in Community.Runtime.Core.permission.GetPermissions(hookable))
				{
					Community.Runtime.Core.permission.RevokeGroupPermission(group, perm);
				}
				return Response();
		}

		if (Community.Runtime.Core.permission.GroupHasPermission(group, permission))
		{
			Community.Runtime.Core.permission.RevokeGroupPermission(group, permission);
		}
		else
		{
			Community.Runtime.Core.permission.GrantGroupPermission(group, permission, null);
		}
		return Response();
	}
}
