namespace Carbon.Plugins
{
    [Info ( "CarbonTemplate", "<author>", "1.0.0" )]
    [Description ( "<optional_description>" )]
    public class CarbonTemplate : CarbonPlugin
    {
        private void OnServerInitialized ()
        {
            Puts ( "Hello world!" );
        }
    }
}