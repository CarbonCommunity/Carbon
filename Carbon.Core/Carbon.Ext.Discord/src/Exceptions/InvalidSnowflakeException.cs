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
	// Token: 0x02000040 RID: 64
	public class InvalidSnowflakeException : BaseDiscordException
	{
		// Token: 0x060001BA RID: 442 RVA: 0x0000DB68 File Offset: 0x0000BD68
		internal InvalidSnowflakeException(string paramName) : base("Invalid Snowflake ID. Parameter Name: " + paramName)
		{
		}
	}
}
