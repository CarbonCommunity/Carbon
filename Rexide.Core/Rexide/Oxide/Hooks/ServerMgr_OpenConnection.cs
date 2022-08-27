using ConVar;
using Facepunch;
using Harmony;
using Rexide.Core.Harmony;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HarmonyPatch ( typeof ( ServerMgr ), "OpenConnection" )]
public class ServerMgr_OpenConnection
{
    public static void Postfix ()
    {
        HookExecutor.CallStaticHook ( "OnServerInitialized" );
    }
}