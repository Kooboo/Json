
using System;

namespace Kooboo.Json
{
    /// <summary>
    ///     对Model进行Json序列化时全局值格式化器
    ///     The global value formatter when the Model is Json serialized
    /// </summary>
    /// <param name="value">传进来的值,The value passed in</param>
    /// <param name="type">值的类型,The type of the value</param>
    /// <param name="handler">提供一些选项进行访问,Provides options for access</param>
    /// <param name="isValueFormat">决定最终是否进行值格式化,Determines whether the value is ultimately formatted</param>
    /// <returns></returns>
    public delegate string JsonSerializerGlobalValueFormatDelegate(object value,Type type, JsonSerializerHandler handler, out bool isValueFormat);
}
