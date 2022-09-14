using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConnectionQueue ), "CanJumpQueue" )]
    public class CanBypassQueue
    {
        public static bool Prefix ( Network.Connection connection, ref System.Boolean __result )
        {
            CarbonCore.Log ( $"Prefix CanBypassQueue" );

            var result = HookExecutor.CallStaticHook ( "CanBypassQueue" );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}