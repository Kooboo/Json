using System;
using System.Runtime.Serialization;
using Kooboo.Json;
using Kooboo.Json.Deserialize;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class EnumsDeserializeTest
    {
        [Flags]
        enum _FlagsEnum
        {
            A = 1,
            B = 2,
            C = 4
        }
        class fds
        {
            public string nAMEr;
        }
     

        [TestMethod]
        public void FlagsEnum_with_number_deserialize_should_be_correct()
        {
            Assert.AreEqual(_FlagsEnum.A, JsonSerializer.ToObject<_FlagsEnum>("1"));
            Assert.AreEqual(_FlagsEnum.A | _FlagsEnum.B | _FlagsEnum.C, JsonSerializer.ToObject<_FlagsEnum>("7"));

        }
        [TestMethod]
        public void FlagsEnumString_deserialize_should_be_correct()
        {
            Assert.AreEqual(_FlagsEnum.A, JsonSerializer.ToObject<_FlagsEnum>("\"A\""));
            Assert.AreEqual(_FlagsEnum.B, JsonSerializer.ToObject<_FlagsEnum>("\"B\""));
            Assert.AreEqual(_FlagsEnum.C, JsonSerializer.ToObject<_FlagsEnum>("\"C\""));
        }

        [TestMethod]
        public void FlagsEnumString_with_order_deserialize_should_be_correct()
        {
            Assert.AreEqual(_FlagsEnum.A | _FlagsEnum.B, JsonSerializer.ToObject<_FlagsEnum>("\"A,B\""));
            Assert.AreEqual(_FlagsEnum.A | _FlagsEnum.B, JsonSerializer.ToObject<_FlagsEnum>("\"B,A\""));
        }

        [TestMethod]
        public void FlagsEnumString_with_space_deserialize_should_be_correct()
        {
            Assert.AreEqual(_FlagsEnum.A | _FlagsEnum.B, JsonSerializer.ToObject<_FlagsEnum>("\"A, B\""));
            Assert.AreEqual(_FlagsEnum.A | _FlagsEnum.B, JsonSerializer.ToObject<_FlagsEnum>("\"B, A\""));
        }

        enum _EnumMemberAttribute
        {
            [EnumMember(Value = "1")]
            A = 1,
            [EnumMember(Value = "2")]
            B = 2,
            [EnumMember(Value = "4")]
            C = 4
        }

        [TestMethod]
        public void EnumMemberString_deserialize_should_be_correct()
        {
            Assert.AreEqual(_EnumMemberAttribute.A, JsonSerializer.ToObject<_EnumMemberAttribute>("\"1\""));
            Assert.AreEqual(_EnumMemberAttribute.B, JsonSerializer.ToObject<_EnumMemberAttribute>("\"2\""));
            Assert.AreEqual(_EnumMemberAttribute.C, JsonSerializer.ToObject<_EnumMemberAttribute>("\"4\""));
        }
    }
}
