using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingBlock ), "DoRotation" )]
    public class OnStructureRotate
    {
        public static void Postfix ( BaseEntity.RPCMessage msg , ref BuildingBlock __instance )
        {
            HookExecutor.CallStaticHook ( "OnStructureRotate" );
        }
    }
}