using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingBlock ), "DoRotation" )]
    public class OnStructureRotate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnStructureRotate" );
        }
    }
}