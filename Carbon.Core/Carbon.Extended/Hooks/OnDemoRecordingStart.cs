using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "StartDemoRecording" )]
    public class OnDemoRecordingStart
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnDemoRecordingStart" );
        }
    }
}