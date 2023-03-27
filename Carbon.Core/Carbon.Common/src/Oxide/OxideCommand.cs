using System;
using Carbon;
using Carbon.Base;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

public class OxideCommand
{
	public string Command { get; set; }
	public BaseHookable Plugin { get; set; }
	public Action<BasePlayer, string, string[]> Callback { get; set; }
	public string[] Permissions { get; set; }
	public string[] Groups { get; set; }
	public int AuthLevel { get; set; } = 0;
	public int Cooldown { get; set; } = 0;
	public bool SkipOriginal { get; set; }
	public string Help { get; set; }
	public bool IsHidden { get; set; }
	public object Reference { get; set; }
	public bool Protected { get; set; }
}
