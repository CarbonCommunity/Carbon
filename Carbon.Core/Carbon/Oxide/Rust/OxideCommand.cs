using Oxide.Plugins;
using System.Collections.Generic;
using System;

public class OxideCommand
{
    public string Command { get; set; }
    public RustPlugin Plugin { get; set; }
    public Action<BasePlayer, string, string []> Callback { get; set; }

    public OxideCommand () { }
    public OxideCommand ( string command, Action<BasePlayer, string, string []> callback )
    {
        Command = command;
        Plugin = CarbonCore.Instance.CorePlugin;
        Callback = callback;
    }
}