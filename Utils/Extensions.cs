using System.Text.RegularExpressions;

namespace DDSS_LobbyGuard.Utils
{
    internal static class Extensions
    {
        private static Regex _rtRegex = new Regex("<.*?>", RegexOptions.Compiled);

        internal static string RemoveRichText(this string val)
            => _rtRegex.Replace(val, string.Empty);
    }
}