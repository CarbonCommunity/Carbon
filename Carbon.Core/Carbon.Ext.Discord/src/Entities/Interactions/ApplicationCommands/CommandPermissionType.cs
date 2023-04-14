using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
	/// <summary>
	/// Represents <a href="https://discord.com/developers/docs/interactions/application-commands#application-command-permissions-object-application-command-permission-type">ApplicationCommandPermissionType</a>
	/// </summary>
	public enum CommandPermissionType
	{
		/// <summary>
		/// This permissions uses Role ID
		/// </summary>
		[System.ComponentModel.Description("ROLE")]
		Role = 1,

		/// <summary>
		/// This permission uses User ID
		/// </summary>
		[System.ComponentModel.Description("USER")]
		User = 2
	}
}
