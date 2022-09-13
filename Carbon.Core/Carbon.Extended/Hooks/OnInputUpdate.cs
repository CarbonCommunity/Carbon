using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( IOEntity ), "UpdateFromInput" )]
    public class OnInputUpdate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnInputUpdate" );
        }
    }
}