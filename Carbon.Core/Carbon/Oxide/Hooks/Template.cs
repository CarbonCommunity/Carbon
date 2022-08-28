using ConVar;
using Facepunch;
using Harmony;
using Carbon.Core.Harmony;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HarmonyPatch ( typeof ( BasePlayer ), "PlayerInit" )]
public class Class_Method
{
    public static void Postfix ()
    {

    }
}