using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BradleyAPC ), "OnKilled" )]
    public class OnEntityDestroy
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityDestroy" );
        }
    }
}