using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PhotoFrame ), "LockSign" )]
    public class OnSignLocked
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnSignLocked" );
        }
    }
}