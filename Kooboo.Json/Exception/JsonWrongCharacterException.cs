using Kooboo.Json.Deserialize;
using System;
using System.Reflection;

namespace Kooboo.Json
{
    /// <summary>
    ///     错误的json字符
    ///     Json Wrong Character
    /// </summary>
    public class JsonWrongCharacterException : Exception
    {
        #region pregenerated metas
        internal static ConstructorInfo _JsonWrongCharacterExceptionCtor =
            typeof(JsonWrongCharacterException).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                new[] { typeof(JsonReader) }, null);
        #endregion

        internal JsonWrongCharacterException() : base("Incorrect JSON format")
        {
        }

        internal JsonWrongCharacterException(string msg) : base("Incorrect JSON format : " + msg)
        {

        }

        internal JsonWrongCharacterException(int index) : base(
            $"Incorrect JSON format : An error occurred on symbol {index} ")
        {

        }

        internal JsonWrongCharacterException(int index, string correctString) : base(
            $"Incorrect JSON format : An error occurred on symbol {index} ,  {correctString}")
        {

        }

        internal JsonWrongCharacterException(int index, char correctChar, char errorChar) : base(
            $"Incorrect JSON format : An error occurred on symbol {index} , It should be {correctChar}, but it's actually {errorChar}")
        {

        }

        internal JsonWrongCharacterException(JsonReader reader) : this(
            reader.Length - reader.Remaining)
        {

        }


        internal JsonWrongCharacterException( JsonReader reader, string correctString) : this(
            reader.Length - reader.Remaining, correctString)
        {

        }


        internal JsonWrongCharacterException(JsonReader reader, char correctChar) : this(
            reader.Length - reader.Remaining, correctChar,
            reader.Json[reader.Length - reader.Remaining])
        {

        }

    }
}
