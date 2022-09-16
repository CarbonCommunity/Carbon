using Carbon.Core;
using Harmony;
using UnityEngine;

[HarmonyPatch ( typeof ( ServerConsole ), "HandleLog" )]
public class ServerConsoleLog
{
    public static bool Prefix ( string message, string stackTrace, LogType type )
    {
        if ( message.StartsWith ( "Trying to load assembly: DynamicAssembly" ) ) return false;

        return true;
    }
}