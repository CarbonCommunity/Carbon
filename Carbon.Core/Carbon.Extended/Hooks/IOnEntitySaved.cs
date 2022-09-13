using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseNetworkable ), "ToStream" )]
    public class IOnEntitySaved
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "IOnEntitySaved" );
        }
    }
}