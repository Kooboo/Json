using System;
using System.Collections.Generic;

namespace Kooboo.Json
{
    /// <summary>
    /// Json Object
    /// </summary>
    public class JObject : Dictionary<string, object>
    {
        public object this[int index] { get { return base[index.ToString()];  }set { base[index.ToString()] = value; } }
        public object this[long index] { get { return base[index.ToString()]; } set { base[index.ToString()] = value; } }
        public object this[Guid index] { get { return base[index.ToString()]; } set { base[index.ToString()] = value; } }
        public object this[DateTime index] { get { return base[index.ToString()]; } set { base[index.ToString()] = value; } }
        public object this[float index] { get { return base[index.ToString()]; } set { base[index.ToString()] = value; } }
        public object this[double index] { get { return base[index.ToString()]; } set { base[index.ToString()] = value; } }
    }
}
