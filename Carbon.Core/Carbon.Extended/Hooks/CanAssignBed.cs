using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SleepingBag ), "AssignToFriend" )]
    public class CanAssignBed
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanAssignBed" );
        }
    }
}