using Oxide.Plugins;

namespace Oxide.Ext.Discord.Helpers
{
    /// <summary>
    /// Converts discord locale codes into oxide locale codes
    /// </summary>
    public static class LocaleConverter
    {
        private static readonly Hash<string, string> DiscordToOxide = new Hash<string, string>();
        private static readonly Hash<string, string> OxideToDiscord = new Hash<string, string>();
        
        static LocaleConverter()
        {
            AddLocale("en","en-US");
            AddLocale("bg-","bg");
            AddLocale("zh","zh-CN");
            AddLocale("hr","hr");
            AddLocale("cs","cs");
            AddLocale("da","da");
            AddLocale("nl","nl");
            AddLocale("fi","fi");
            AddLocale("fr","fr");
            AddLocale("de","de");
            AddLocale("el","el");
            AddLocale("hi","hi");
            AddLocale("hu","hu");
            AddLocale("it","it");
            AddLocale("ja","ja");
            AddLocale("ko","ko");
            AddLocale("lt","lt");
            AddLocale("no","no");
            AddLocale("pl","pl");
            AddLocale("pt","pt-BR");
            AddLocale("ro","ro");
            AddLocale("ru","ru");
            AddLocale("es","es-ES");
            AddLocale("sv","sv-SE");
            AddLocale("th","th");
            AddLocale("tr","tr");
            AddLocale("uk","uk");
            AddLocale("vi","vi");
            
            DiscordToOxide["en-GB"] = "en";
            DiscordToOxide["zh-TW"] = "zh";
        }

        private static void AddLocale(string oxide, string discord)
        {
            DiscordToOxide[discord] = oxide;
            OxideToDiscord[oxide] = discord;
        }

        /// <summary>
        /// Returns the oxide locale for a given discord locale
        /// </summary>
        /// <param name="discordLocale">Discord locale to get oxide locale for</param>
        /// <returns>Oxide locale if it exists; null otherwise</returns>
        public static string GetOxideLocale(string discordLocale)
        {
            return !string.IsNullOrEmpty(discordLocale) ? DiscordToOxide[discordLocale] : string.Empty;
        }
        
        /// <summary>
        /// Returns the discord locale for a given oxide locale
        /// </summary>
        /// <param name="oxideLocale">oxide locale to get discord locale for</param>
        /// <returns>Discord locale if it exists; null otherwise</returns>
        public static string GetDiscordLocale(string oxideLocale)
        {
            return !string.IsNullOrEmpty(oxideLocale) ? OxideToDiscord[oxideLocale] : string.Empty;
        }
    }
}