///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using Carbon.Core;

public class OxideCommand
{
	public string Command { get; set; }
	public BaseHookable Plugin { get; set; }
	public Action<BasePlayer, string, string []> Callback { get; set; }
	public bool SkipOriginal { get; set; }
	public string Help { get; set; }

	public OxideCommand () { }
	public OxideCommand (string command, Action<BasePlayer, string, string []> callback, bool skipOriginal)
	{
		Command = command;
		Plugin = CarbonCore.Instance.CorePlugin;
		Callback = callback;
		SkipOriginal = skipOriginal;
	}
}
