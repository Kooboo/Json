using System;
using System.Collections.Generic;

namespace Kooboo.Json
{
    /// <summary>
    /// Json Object
    /// </summary>
    public class JObject : Dictionary<string, object>
    {
        public object this[int index]=>base[index.ToString()];
        public object this[long index] => base[index.ToString()];
        public object this[Guid index] => base[index.ToString()];
        public object this[DateTime index] => base[index.ToString()];
        public object this[float index] => base[index.ToString()];
        public object this[double index] => base[index.ToString()];
    }
}
