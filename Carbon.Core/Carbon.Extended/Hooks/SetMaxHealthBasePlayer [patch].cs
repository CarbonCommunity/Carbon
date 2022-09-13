using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "MaxHealth" )]
    public class SetMaxHealthBasePlayer [patch]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "SetMaxHealthBasePlayer [patch]" );
        }
    }
}