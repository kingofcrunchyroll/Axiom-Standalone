using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Axiom.Utilities
{
    public class FileUtilities
    {
        public static string GetFileExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            var cleanName = fileName.Split('?')[0];
            return Path.GetExtension(cleanName).TrimStart('.').ToLowerInvariant();
        }

        public static string RemoveLastDirectory(string directory) =>
            string.IsNullOrEmpty(directory) || directory.LastIndexOf('/') <= 0
                ? string.Empty
                : directory[..directory.LastIndexOf('/')];

        public static string RemoveFileExtension(string file)
        {
            if (string.IsNullOrEmpty(file))
                return string.Empty;

            int index = 0;
            string output = "";
            string[] split = file.Split(".");
            foreach (string data in split)
            {
                index++;
                if (index == split.Length) continue;
                if (index > 1)
                    output += ".";

                output += data;
            }
            return output;
        }

        public static string GetFullPath(Transform transform)
        {
            if (transform == null)
                return string.Empty;

            string path = "";
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = path == "" ? transform.name : transform.name + "/" + path;
            }
            return path;
        }

        public static string GetGamePath() =>
            AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

        public static string SanitizeFileName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "file";

            input = input.Trim();
            char[] illegalChars = Path.GetInvalidFileNameChars();
            input = illegalChars.Aggregate(input, (current, c) => current.Replace(c, '_'));

            input = input.Replace("../", "")
                         .Replace("..\\", "")
                         .Replace("./", "")
                         .Replace(".\\", "");

            input = input.Replace(":", "")
                         .Replace("\\", "")
                         .Replace("/", "");

            if (input.Length > 64)
                input = input[..64];

            if (string.IsNullOrWhiteSpace(input))
                input = "file";

            return input;
        }
    }
}
