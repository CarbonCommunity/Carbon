using Facepunch;

namespace Carbon;

public static partial class WebControlPanel
{
	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.PermissionsView)]
	private static void RPC_GetPermissionsMetadata(BridgeRead read)
	{
		var permission = Community.Runtime.Core.permission;

		using var plugins = Pool.Get<PooledList<HookableInfo>>();
		using var modules = Pool.Get<PooledList<HookableInfo>>();

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

				plugins.Add(HookableInfo.Get(plugin, permissions));
			}
		}
		foreach (var module in Community.Runtime.ModuleProcessor.Modules)
		{
			var permissions = permission.GetPermissions(module);

			if (permissions == null || permissions.Length == 0)
			{
				continue;
			}

			modules.Add(HookableInfo.Get(module, permissions));
		}

		var write = StartRpcResponse();
		var groups = permission.GetGroups();
		var allPermissions = permission.GetPermissions();

		write.WriteObject(groups.Length);
		for (int i = 0; i < groups.Length; i++)
		{
			write.WriteObject(groups[i]);
		}
		write.WriteObject(allPermissions.Length);
		for (int i = 0; i < allPermissions.Length; i++)
		{
			write.WriteObject(allPermissions[i]);
		}
		write.WriteObject(plugins.Count);
		for (int i = 0; i < plugins.Count; i++)
		{
			plugins[i].Serialize(write);
		}
		write.WriteObject(modules.Count);
		for (int i = 0; i < modules.Count; i++)
		{
			modules[i].Serialize(write);
		}
		SendRpcResponse(read.Connection, write);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.PermissionsView)]
	private static void RPC_GetGroupPermissions(BridgeRead read)
	{
		var group = read.String();
		var permissions = Community.Runtime.Core.permission.GetGroupPermissions(group, true);
		var write = StartRpcResponse();
		write.WriteObject(permissions.Length);
		for (int i = 0; i < permissions.Length; i++)
		{
			write.WriteObject(permissions[i]);
		}
		SendRpcResponse(read.Connection, write);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.PermissionsEdit)]
	private static void RPC_TogglePermission(BridgeRead read)
	{
		var group = read.String();
		var permission = read.String();
		var plugin = read.String();
		var hookable = ModLoader.FindPlugin(plugin) ?? Community.Runtime.ModuleProcessor.Modules.FirstOrDefault(x => x.Name.Equals(plugin));

		switch (permission)
		{
			case "grantall":
				foreach (var perm in Community.Runtime.Core.permission.GetPermissions(hookable))
				{
					Community.Runtime.Core.permission.GrantGroupPermission(group, perm, null);
				}
				return;
			case "revokeall":
				foreach (var perm in Community.Runtime.Core.permission.GetPermissions(hookable))
				{
					Community.Runtime.Core.permission.RevokeGroupPermission(group, perm);
				}
				return;
		}

		if (Community.Runtime.Core.permission.GroupHasPermission(group, permission))
		{
			Community.Runtime.Core.permission.RevokeGroupPermission(group, permission);
		}
		else
		{
			Community.Runtime.Core.permission.GrantGroupPermission(group, permission, null);
		}
	}

	public struct HookableInfo
	{
		private string name;
		private string author;
		private string[] permissions;

		public static HookableInfo Get(BaseHookable hookable, string[] permissions)
		{
			HookableInfo info = default;
			info.name = hookable.Name;
			if (hookable is RustPlugin rustPlugin)
			{
				info.author = rustPlugin.Author;
			}
			info.permissions = permissions;
			return info;
		}

		public void Serialize(BridgeWrite write)
		{
			write.WriteObject(name);
			write.WriteObject(author);
			if (permissions == null)
			{
				write.WriteObject(0);
			}
			else
			{
				write.WriteObject(permissions.Length);
				for (int i = 0; i < permissions.Length; i++)
				{
					write.WriteObject(permissions[i]);
				}
			}
		}
	}
}
