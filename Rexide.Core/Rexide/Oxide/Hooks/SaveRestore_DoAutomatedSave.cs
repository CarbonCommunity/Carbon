using ConVar;
using Facepunch;
using Harmony;
using Rexide.Core.Harmony;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HarmonyPatch ( typeof ( SaveRestore ), "DoAutomatedSave" )]
public class SaveRestore_DoAutomatedSave
{
    public static void Postfix ()
    {
        HookExecutor.CallStaticHook ( "OnServerSave" );
    }
}