using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kooboo.Json
{
    /// <summary>
    ///     Json Deserialize Handler
    /// </summary>
    public class JsonDeserializeHandler
    {
        #region pregenerated metas
        internal static FieldInfo _Types =
            typeof(JsonDeserializeHandler).GetField(nameof(Types), BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo _Option =
            typeof(JsonDeserializeHandler).GetField(nameof(Option), BindingFlags.Instance | BindingFlags.Public);
        #endregion

        /// <summary>
        ///     Json Serializer Option
        /// </summary>
        public JsonDeserializeOption Option = null;

        internal Queue<Type> Types = new Queue<Type>();
    }
}
