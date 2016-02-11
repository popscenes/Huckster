using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace infrastructure.Utility
{
    public static class StringExtensionMethods
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return String.IsNullOrEmpty(str);
        }
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !String.IsNullOrEmpty(str);
        }
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return String.IsNullOrWhiteSpace(str);
        }
        public static bool IsNotNullOrWhiteSpace(this string str)
        {
            return !String.IsNullOrWhiteSpace(str);
        }
        public static string Right(this string str, int len)
        {
            if (str.IsNullOrEmpty())
                return "";
            int start = Math.Max(str.Length - len, 0);
            return str.Substring(start);
        }
        public static string Left(this string str, int len)
        {
            if (str.IsNullOrEmpty())
                return "";
            int end = Math.Min(len, str.Length);
            return str.Substring(0, end);
        }
        public static string Append(this string str, string append, string delimiter = "")
        {
            if (str.IsNullOrEmpty())
                return append;
            if (append.IsNullOrEmpty())
                return str;
            if (delimiter == null)
                delimiter = "";
            return str + delimiter + append;
        }
        public static void Append(ref string str, string append, string delimiter = "")
        {
            str = str.Append(append, delimiter);
        }

        public static string RemoveLineBreak(this string str)
        {
            return Regex.Replace(str, @"\r\n?|\n", "<br />");
        }

        public static string InsertBeforeExtension(this string str, string insert)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            string extension = Path.GetExtension(str);
            if (extension.Length == 0 && str.EndsWith("."))
                extension = ".";

            string filename = str.Substring(0, str.Length - extension.Length);
            return filename + insert + extension;
        }

        public static string ToSafeFileName(this string originalFileName)
        {
            if (originalFileName == null)
                return "";

            var invalidChars = Path.GetInvalidFileNameChars();
            var directoryTokens = originalFileName.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < directoryTokens.Count; i++)
            {
                directoryTokens[i] = new string(directoryTokens[i].Where(_ => !invalidChars.Contains(_)).ToArray());
            }

            return String.Join("\\", directoryTokens);
        }
    }
}
