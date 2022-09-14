using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TriggerBase ), "OnEntityLeave" )]
    public class OnEntityLeave
    {
        public static void Postfix ( BaseEntity ent )
        {
            HookExecutor.CallStaticHook ( "OnEntityLeave" );
        }
    }
}