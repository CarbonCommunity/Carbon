namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
	/// <summary>
	/// Represents <a href="https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-types">Application Command Type</a>
	/// </summary>
	public enum ApplicationCommandType
	{
		/// <summary>
		/// Slash commands; a text-based command that shows up when a user types /
		/// </summary>
		[System.ComponentModel.Description("CHAT_INPUT")]
		ChatInput = 1,

		/// <summary>
		/// A UI-based command that shows up when you right click or tap on a user
		/// </summary>
		[System.ComponentModel.Description("USER")]
		User = 2,

		/// <summary>
		/// A UI-based command that shows up when you right click or tap on a messages
		/// </summary>
		[System.ComponentModel.Description("MESSAGE")]
		Message = 3
	}
}
