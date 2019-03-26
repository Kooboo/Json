using System;

namespace Kooboo.Json
{
    /// <summary>
    ///      对Model进行Json反序列化时全局值格式化器
    ///      Global value formatter for Json deserialization of Model
    /// </summary>
    /// <param name="jsonFragment">Json字符串中的片段,Fragments in the Json string</param>
    /// <param name="elementType">该jsonFragment所对应的类型,The type corresponding to the jsonFragment</param>
    /// <param name="jsonDeserializeHandler">提供一些选项进行访问,Provides options for access</param>
    /// <param name="isValueFormat">决定最终是否进行值格式化,Determines whether the value is ultimately formatted</param>
    /// <returns></returns>
    public delegate object JsonDeserializeGlobalValueFormatDelegate(string jsonFragment, Type elementType, JsonDeserializeHandler jsonDeserializeHandler, out bool isValueFormat);
}
