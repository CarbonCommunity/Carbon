using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TriggerBase ), "OnEntityEnter" )]
    public class OnEntityEnter
    {
        public static void Postfix ( BaseEntity ent )
        {
            HookExecutor.CallStaticHook ( "OnEntityEnter" );
        }
    }
}