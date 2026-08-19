using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static Axiom.Utilities.RandomUtilities;

namespace Axiom.Extensions
{
    public static class StringExtensions
    {
        public static string ClearTags(this string input) =>
            NoRichtextTags(input);

        public static string ToTitleCase(string text) =>
            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());

        public static string Hash(this string input) =>
            GetSHA256(input);

        public static string EnforceLength(this string str, int maxLength) =>
            str.Length > maxLength ? str[..maxLength] : str;

        public static string Random(this string _, int length) =>
            RandomString(length);

        public static string NoRichtextTags(string input, string replace = "")
        {
            Regex notags = new Regex("<.*?>", RegexOptions.IgnoreCase);
            return notags.Replace(input, replace);
        }

        public static string GetSHA256(string input)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in bytes)
                stringBuilder.Append(b.ToString("x2"));

            return stringBuilder.ToString();
        }
    }
}
