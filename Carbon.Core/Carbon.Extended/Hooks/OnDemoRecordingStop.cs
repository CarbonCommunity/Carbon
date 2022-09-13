using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "StopDemoRecording" )]
    public class OnDemoRecordingStop
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnDemoRecordingStop" );
        }
    }
}