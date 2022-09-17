namespace Oxide.Plugins
{
    [Info ( "Dummy", "Carbon", "1.0.0" )]
    [Description ( "yeet." )]
    public class Dummy : RustPlugin
    {
        private void OnServerInitialized ()
        {
            Puts ( $"This plugin works :D" );
        }
    }
}