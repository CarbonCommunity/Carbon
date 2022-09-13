using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseFishingRod ), "Server_Cancel" )]
    public class OnFishingStopped
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnFishingStopped" );
        }
    }
}