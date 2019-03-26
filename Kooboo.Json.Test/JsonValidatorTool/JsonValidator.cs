using System.Collections.Generic;
using System.IO;

namespace JsonValidatorTool
{
    /// <summary>
    /// The JsonValidator class contains all methods to validate json strings.
    /// </summary>
    public static class JsonValidator
    {
        /// <summary>
        /// Validates the specified json.
        /// </summary>
        /// <param name="json">Json.</param>
        /// <param name="depth">It restricts the level of maximum nesting.</param>
        /// <exception cref="JsonNotValidException"></exception>
        public static void Validate(string json, int? depth = null)
        {
            using (var reader = new StringReader(json))
            {
                ValidateJson(ReadChars(reader), depth);
            }
        }

        /// <summary>
        /// Validates the specified json.
        /// </summary>
        /// <param name="json">Json.</param>
        /// <param name="depth">It restricts the level of maximum nesting.</param>
        public static bool IsValid(string json, int? depth = null)
        {
            var isint = int.TryParse(json, out _);
            var isbool = bool.TryParse(json, out _);
            var isdecimal = decimal.TryParse(json,out _);
            var isfloat = float.TryParse(json, out _);
            var isdouble = double.TryParse(json,out _);
            if (json == "null" || isint || json.StartsWith("\"") && json.EndsWith("\"") || isbool|| isdecimal|| isfloat|| isdouble)
                return true;
            try
            {
                Validate(json, depth);
                return true;
            }
            catch (JsonNotValidException)
            {
                return false;
            }

        }

        static void ValidateJson(IEnumerable<char> chars, int? depth = null)
        {
            var checker = depth.HasValue ? new JsonChecker(depth.Value) : new JsonChecker();
            foreach (char ch in chars)
            {
                checker.Check(ch);
            }
            checker.FinalCheck();
        }

        static IEnumerable<char> ReadChars(TextReader reader)
        {
            int ch = reader.Read();
            while (ch != -1)
            {
                yield return (char)ch;
                ch = reader.Read();
            }
        }
    }
}
