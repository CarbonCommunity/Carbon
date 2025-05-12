namespace Carbon.Core;

public partial class CorePlugin
{
	[AuthLevel(2)]
	[ConsoleCommand("vault", "Prints a whole list of all vault factory and item keys without any protected values")]
	private void GetVault(ConsoleSystem.Arg arg)
	{
		using var table = new StringTable("factory", "items", "encrypted", "value");
		var factories = Vault.GetFactories();

		foreach (var factory in factories)
		{
			var first = factory.Count > 0 ? factory[0] : null;
			table.AddRow(" " + Vault.Pool.Get(factory.id), first == null ? string.Empty : Vault.Pool.Get(first.id), first?.encrypted, first == null || first.encrypted ? string.Empty : first.cache);
			for(int i = 1; i < factory.Count; i++)
			{
				var item = factory[i];
				table.AddRow(string.Empty, string.Empty, Vault.Pool.Get(item.id), item.encrypted, item.encrypted ? string.Empty : item.cache);
			}
		}

		arg.ReplyWith(table.ToStringMinimal());
	}

	[AuthLevel(2)]
	[ConsoleCommand("vault_add", "Adds a new element to the vault")]
	private void VaultAdd(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(2))
		{
			arg.ReplyWith($"Syntax: c.vault_add <key> <value> [encrypted|true] [factory|global]");
			return;
		}

		var key = arg.GetString(0);
		var value = arg.GetString(1);
		var encrypted = arg.GetBool(2, true);
		var factory = arg.GetString(3, Vault.Global);
		var isOverriden = (Vault.GetFactory(Vault.Pool.Get(factory))?.HasItem(Vault.Pool.Get(key))).GetValueOrDefault();
		arg.ReplyWith(Vault.Add(factory, key, value, encrypted) ? isOverriden
				? $"Updated vault factory {(encrypted ? "encrypted" : "unencrypted")} item '{key}' for factory '{factory}'"
				: $"Added new vault factory {(encrypted ? "encrypted" : "unencrypted")} item '{key}' for factory '{factory}'"
				: "Couldn't add a new vault factory item in Carbon.Vault, probably because invalid parameters");
	}

	[AuthLevel(2)]
	[ConsoleCommand("vault_remove", "Removes an element from the vault")]
	private void VaultRemove(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1))
		{
			arg.ReplyWith($"Syntax: c.vault_remove <key> [factory|global]");
			return;
		}

		var key = arg.GetString(0);
		var factory = arg.GetString(1, Vault.Global);

		arg.ReplyWith(Vault.Remove(factory, key)
			? $"Removed vault factory item '{key}' for factory '{factory}'"
			: "Couldn't remove a vault factory item from Carbon.Vault, probably because it doesn't");
	}
}
