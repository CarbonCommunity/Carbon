using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConsoleSystem ), "UpdateValuesFromCommandLine" )]
    public class IOnRunCommandLine
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "IOnRunCommandLine" );
        }
    }
}