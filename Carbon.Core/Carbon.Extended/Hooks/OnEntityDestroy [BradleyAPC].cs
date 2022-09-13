using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BradleyAPC ), "OnKilled" )]
    public class OnEntityDestroy [BradleyAPC]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityDestroy [BradleyAPC]" );
        }
    }
}