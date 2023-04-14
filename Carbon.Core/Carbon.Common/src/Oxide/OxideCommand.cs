using System;
using API.Commands;
using Carbon.Base;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

public class OxideCommand : AuthenticatedCommand
{
	public string Command { get { return Name; } set { Name = value; } }
	public BaseHookable Plugin { get; set; }
	public new Action<BasePlayer, string, string[]> Callback { get; set; }
	public string[] Permissions { get { return Auth == null ? default : Auth.Permissions; } set { if (Auth != null) Auth.Permissions = value; } }
	public string[] Groups { get { return Auth == null ? default : Auth.Groups; } set { if (Auth != null) Auth.Groups = value; } }
	public int AuthLevel { get { return Auth == null ? default : Auth.AuthLevel; } set { if (Auth != null) Auth.AuthLevel = value; } }
	public int Cooldown { get { return Auth == null ? default : Auth.Cooldown; } set { if (Auth != null) Auth.Cooldown = value; } }
	public bool IsHidden { get { return HasFlag(CommandFlags.Hidden); } set { SetFlag(CommandFlags.Hidden, value); } }
	public bool Protected { get { return HasFlag(CommandFlags.Protected); } set { SetFlag(CommandFlags.Protected, value); } }
}
