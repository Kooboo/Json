using System;
using System.IO;

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
        /// <param name="option">Serialize option</param>
        /// <returns>JSON strings</returns>
        public static string ToJson<T>(T value, JsonSerializerOption option = null)
        {
            var handler = new JsonSerializerHandler(new System.Text.StringBuilder())
            {
                Option = option?? defaultSerializerOption
            };
            Serializer.FormattingProvider<T>.Get(value, handler);
            return handler.ToString();
        }

        /// <summary>
        ///     Serialize Object from the StreamWriter
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="streamWriter">Stream</param>
        /// <param name="option">Serialize option</param>
        public static void ToJson<T>(T value, StreamWriter streamWriter, JsonSerializerOption option = null)
        {
            streamWriter.Flush();//posting set 0
            bool isAutoFlush = false;
            if (streamWriter.AutoFlush)
            {
                isAutoFlush = true;
                streamWriter.AutoFlush = false;
            }
            var handler = new JsonSerializerHandler(streamWriter)
            {
                Option = option ?? defaultSerializerOption
            };
            Serializer.FormattingProvider<T>.Get(value, handler);
            if(isAutoFlush)
                streamWriter.AutoFlush = true;
        }


        /// <summary>
        ///     Converting Json strings to objects
        /// </summary>
        /// <typeparam name="T">Types converted</typeparam>
        /// <param name="json">Json string</param>
        /// <param name="option">Json Deserialize Option</param>
        /// <returns>Object</returns>
        public static T ToObject<T>(string json, JsonDeserializeOption option=null)
        {
            var handler = new JsonDeserializeHandler
            {
                Option = option?? defaultDeserializeOption
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
        public static object ToObject(string json, Type type, JsonDeserializeOption option=null)
        {
            var handler = new JsonDeserializeHandler
            {
                Option = option ?? defaultDeserializeOption
            };
            return Deserialize.DeserializeObjectJump.GetThreadSafetyJumpFunc(json, type, handler);
        }

        /// <summary>
        ///     Deserializes JSON from the StreamReader
        /// </summary>
        /// <typeparam name="T">Types converted</typeparam>
        /// <param name="streamReader">Stream</param>
        /// <param name="option">Json Deserialize Option</param>
        /// <returns>Object</returns>
        public static T ToObject<T>(StreamReader streamReader, JsonDeserializeOption option = null)
        {
            var handler = new JsonDeserializeHandler
            {
                Option = option ?? defaultDeserializeOption
            };
            return Deserialize.ResolveProvider<T>.Convert(streamReader, handler);
        }

        /// <summary>
        ///     Deserializes JSON from the StreamReader
        /// </summary>
        /// <param name="streamReader">Stream</param>
        /// <param name="type">Types converted</param>
        /// <param name="option">Json Deserialize Option</param>
        /// <returns>Object</returns>
        public static object ToObject(StreamReader streamReader, Type type, JsonDeserializeOption option = null)
        {
            var handler = new JsonDeserializeHandler
            {
                Option = option ?? defaultDeserializeOption
            };
            return Deserialize.DeserializeObjectJump.GetThreadSafetyJumpFunc(streamReader, type, handler);
        }

    }
}
