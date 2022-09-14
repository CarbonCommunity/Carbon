using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemContainer ), "CanAcceptItem" )]
    public class CanAcceptItem
    {
        public static bool Prefix ( Item item, System.Int32 targetPos, ref ItemContainer+CanAcceptResult __result )
        {
            CarbonCore.Log ( $"Prefix CanAcceptItem" );

            var result = HookExecutor.CallStaticHook ( "CanAcceptItem" );
            
            if ( result != null )
            {
                __result = ( ItemContainer+CanAcceptResult ) result;
                return false;
            }

            return true;
        }
    }
}