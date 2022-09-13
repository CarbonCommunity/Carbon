using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemModStudyBlueprint ), "ServerCommand" )]
    public class OnPlayerStudyBlueprint
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerStudyBlueprint" );
        }
    }
}