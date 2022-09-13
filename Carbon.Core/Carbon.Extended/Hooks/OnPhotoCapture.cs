using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( InstantCameraTool ), "TakePhoto" )]
    public class OnPhotoCapture
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPhotoCapture" );
        }
    }
}