using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseEntity ), "GetBuildingPrivilege" )]
    public class OnBuildingPrivilege
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnBuildingPrivilege" );
        }
    }
}