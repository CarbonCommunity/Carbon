namespace Oxide.Plugins
{
    [Info ( "OxideTemplate", "<author>", "1.0.0" )]
    [Description ( "<optional_description>" )]
    public class OxideTemplate : RustPlugin
    {
        private void OnServerInitialized ()
        {
            Puts ( "Hello world!" );
        }
    }
}