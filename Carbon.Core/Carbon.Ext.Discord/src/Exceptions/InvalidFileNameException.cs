/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Exceptions
{
	// Token: 0x0200003C RID: 60
	public class InvalidFileNameException : BaseDiscordException
	{
		// Token: 0x060001B6 RID: 438 RVA: 0x0000DB4E File Offset: 0x0000BD4E
		public InvalidFileNameException(string fileName) : base("'" + fileName + "' is not a valid filename for discord. Valid filename characters are alphanumeric with underscores, dashes, or dots")
		{
		}
	}
}
