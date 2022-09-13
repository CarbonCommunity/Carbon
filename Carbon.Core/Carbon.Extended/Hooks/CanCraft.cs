using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerBlueprints ), "CanCraft" )]
    public class CanCraft
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanCraft" );
        }
    }
}