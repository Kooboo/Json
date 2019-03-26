using System;
using Kooboo.Json.Deserialize;
using Kooboo.Json.Serializer;

namespace Kooboo.Json
{
    internal class ExpressionBuildTypeAttribute : Attribute
    {
        internal DeserializeBuildTypeEnum _deserializeBuildType;

        internal SerializerBuildTypeEnum _serializerBuildTypeEnum;

        internal ExpressionBuildTypeAttribute(DeserializeBuildTypeEnum buildType)
        {
            _deserializeBuildType = buildType;
        }

        internal ExpressionBuildTypeAttribute(SerializerBuildTypeEnum buildType)
        {
            _serializerBuildTypeEnum = buildType;
        }
    }
}
