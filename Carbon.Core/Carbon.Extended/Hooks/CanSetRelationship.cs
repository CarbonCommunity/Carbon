using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RelationshipManager ), "SetRelationship" )]
    public class CanSetRelationship
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanSetRelationship" );
        }
    }
}