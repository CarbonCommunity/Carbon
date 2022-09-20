using Oxide.Plugins;
using System;
using Carbon.Core;

public class OxideCommand
{
    public string Command { get; set; }
    public RustPlugin Plugin { get; set; }
    public Action<BasePlayer, string, string []> Callback { get; set; }
    public bool SkipOriginal { get; set; }
    public string Help { get; set; }

    public OxideCommand () { }
    public OxideCommand ( string command, Action<BasePlayer, string, string []> callback, bool skipOriginal )
    {
        Command = command;
        Plugin = CarbonCore.Instance.CorePlugin;
        Callback = callback;
        SkipOriginal = skipOriginal;
    }
}