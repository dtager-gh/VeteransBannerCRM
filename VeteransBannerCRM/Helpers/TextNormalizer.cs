using System;
using System.Globalization;
using System.Linq;

namespace VeteransBannerCRM.Helpers
{
    public static class TextNormalizer
    {
        public static string CleanText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            return string.Join(
                " ",
                value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        public static string ToTitleCase(string value)
        {
            value = CleanText(value).ToLower();

            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
        }

        public static string CleanEmail(string value)
        {
            return CleanText(value).ToLower();
        }
    }
}