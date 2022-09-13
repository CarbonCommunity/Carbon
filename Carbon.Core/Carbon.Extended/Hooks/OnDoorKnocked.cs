using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DoorKnocker ), "Knock" )]
    public class OnDoorKnocked
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnDoorKnocked" );
        }
    }
}