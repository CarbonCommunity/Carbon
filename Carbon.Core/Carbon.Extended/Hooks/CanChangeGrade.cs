using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingBlock ), "CanChangeToGrade" )]
    public class CanChangeGrade
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanChangeGrade" );
        }
    }
}