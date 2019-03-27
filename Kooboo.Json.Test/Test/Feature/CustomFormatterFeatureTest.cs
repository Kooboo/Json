using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class CustomFormatterFeatureTest
    {
        class VersionClass
        {
            public VersionClass(string version1, string version2)
            {
                StringProperty1 = "StringProperty1";
                Version1 = new Version(version1);
                Version2 = new Version(version2);
                StringProperty2 = "StringProperty2";
            }

            public VersionClass()
            {
            }

            public string StringProperty1 { get; set; }
            [VersionAsString]
            public Version Version1 { get; set; }
            [VersionAsString]
            public Version Version2 { get; set; }
            public string StringProperty2 { get; set; }
        }

        class VersionAsStringAttribute : ValueFormatAttribute
        {
            public override string WriteValueFormat(object value, Type type, JsonSerializerHandler handler, out bool isValueFormat)
            {
                isValueFormat = true;
                if (value == null)
                    return null;
                else if (value is Version)
                    return value.ToString();
                else
                    throw new Exception("Expected Version object value");
            }

            public override object ReadValueFormat(string value, Type type, JsonDeserializeHandler handler, out bool isValueFormat)
            {
                isValueFormat = true;
                if (value == null)
                    return null;
                try
                {
                    if (value.Length <= 2)
                        throw new Exception("Version format error");
                    return new Version(value);
                }
                catch
                {
                    throw new Exception("Version format error");
                }
            }
        }

        [TestMethod]
        public void CustomFormatterFeature_should_be_work_correct()
        {
            Version expectedVersion1 = new Version("1.0.0.0");
            Version expectedVersion2 = new Version("2.0.0.0");
            string json = string.Format(@"{{""StringProperty1"": ""StringProperty1"", ""Version1"": ""{0}"", ""Version2"": ""{1}"", ""StringProperty2"": ""StringProperty2""}}", "1.0.0.0", "2.0.0.0");
            VersionClass versionClass = JsonSerializer.ToObject<VersionClass>(json);
            Assert.AreEqual("StringProperty1", versionClass.StringProperty1);
            Assert.AreEqual(expectedVersion1, versionClass.Version1);
            Assert.AreEqual(expectedVersion2, versionClass.Version2);
            Assert.AreEqual("StringProperty2", versionClass.StringProperty2);
        }
    }
}