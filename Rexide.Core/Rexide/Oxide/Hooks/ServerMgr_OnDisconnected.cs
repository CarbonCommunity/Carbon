using ConVar;
using Facepunch;
using Harmony;
using Rexide.Core.Harmony;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HarmonyPatch ( typeof ( ServerMgr ), "OnDisconnected" )]
public class ServerMgr_OnDisconnected
{
    public static void Postfix ( string strReason, Network.Connection connection )
    {
        HookExecutor.CallStaticHook ( "OnPlayerDisconnected", connection.player as BasePlayer, strReason );
    }
}