/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core.Libraries;

public class Time : Library
{
	internal static readonly DateTime Epoch = new DateTime(1970, 1, 1);

	public DateTime GetCurrentTime()
	{
		return DateTime.UtcNow;
	}

	public DateTime GetDateTimeFromUnix(uint timestamp)
	{
		return Epoch.AddSeconds(timestamp);
	}

	public uint GetUnixTimestamp()
	{
		return (uint)DateTime.UtcNow.Subtract(Epoch).TotalSeconds;
	}

	public uint GetUnixFromDateTime(DateTime time)
	{
		return (uint)time.Subtract(Epoch).TotalSeconds;
	}
}
