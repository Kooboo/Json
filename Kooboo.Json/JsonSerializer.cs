using System;

namespace Kooboo.Json
{
    /// <summary>
    ///      KoobooJson Serializer
    /// </summary>
    public class JsonSerializer
    {
        private static readonly JsonSerializerOption defaultSerializerOption = new JsonSerializerOption();
        private static readonly JsonDeserializeOption defaultDeserializeOption = new JsonDeserializeOption();

        /// <summary>
        ///     Serialize objects into JSON strings
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>JSON strings</returns>
        public static string ToJson<T>(T value)
        {
            return ToJson(value, defaultSerializerOption);
        }

        /// <summary>
        ///     Serialize objects into JSON strings
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="option">Serialize option</param>
        /// <returns>JSON strings</returns>
        public static string ToJson<T>(T value, JsonSerializerOption option)
        {
            var handler = new JsonSerializerHandler { Option = option };
            Serializer.FormattingProvider<T>.Get(value, handler);
            return handler.ToString();
        }

        /// <summary>
        ///     Converting Json strings to objects
        /// </summary>
        /// <typeparam name="T">Types converted</typeparam>
        /// <param name="json">Json string</param>
        /// <returns>Object</returns>
        public static T ToObject<T>(string json)
        {
            return ToObject<T>(json, defaultDeserializeOption);
        }

        /// <summary>
        ///     Converting Json strings to objects
        /// </summary>
        /// <param name="json">Json string</param>
        /// <param name="type">Types converted</param>
        /// <returns>Object</returns>
        public static object ToObject(string json, Type type)
        {
            var handler = new JsonDeserializeHandler
            {
                Option = defaultDeserializeOption
            };
            return Deserialize.DeserializeObjectJump.GetThreadSafetyJumpFunc(json, type, handler);
        }

        /// <summary>
        ///     Converting Json strings to objects
        /// </summary>
        /// <typeparam name="T">Types converted</typeparam>
        /// <param name="json">Json string</param>
        /// <param name="option">Json Deserialize Option</param>
        /// <returns>Object</returns>
        public static T ToObject<T>(string json, JsonDeserializeOption option)
        {
            var handler = new JsonDeserializeHandler
            {
                Option = option
            };
            return Deserialize.ResolveProvider<T>.Convert(json, handler);
        }

        /// <summary>
        ///     Converting Json strings to objects
        /// </summary>
        /// <param name="json">Json string</param>
        /// <param name="type">Types converted</param>
        /// <param name="option">Json Deserialize Option</param>
        /// <returns>Object</returns>
        public static object ToObject(string json, Type type, JsonDeserializeOption option)
        {
            var handler = new JsonDeserializeHandler
            {
                Option = option
            };
            return Deserialize.DeserializeObjectJump.GetThreadSafetyJumpFunc(json, type, handler);
        }
    }
}
