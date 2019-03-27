using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;
using System.Runtime.InteropServices;

namespace Kooboo.Json.Test
{
    //    6.  Numbers

    //   The representation of numbers is similar to that used in most
    //   programming languages.A number is represented in base 10 using
    //   decimal digits.It contains an integer component that may be
    //   prefixed with an optional minus sign, which may be followed by a
    //   fraction part and/or an exponent part.  Leading zeros are not
    //   allowed.

    //   A fraction part is a decimal point followed by one or more digits.



    //Bray Standards Track[Page 6]

    //RFC 7159                          JSON March 2014


    //   An exponent part begins with the letter E in upper or lower case,
    //   which may be followed by a plus or minus sign.  The E and optional
    //   sign are followed by one or more digits.

    //   Numeric values that cannot be represented in the grammar below (such
    //   as Infinity and NaN) are not permitted.

    //      number = [minus] int[frac][exp]

    //      decimal-point = %x2E       ; .

    //      digit1-9 = %x31-39         ; 1-9

    //      e = %x65 / %x45            ; e E

    //      exp = e[minus / plus] 1* DIGIT

    //      frac = decimal-point 1* DIGIT

    //      int = zero / (digit1-9 * DIGIT )

    //      minus = %x2D               ; -

    //      plus = %x2B                ; +

    //      zero = %x30                ; 0

    //   This specification allows implementations to set limits on the range
    //   and precision of numbers accepted.Since software that implements
    //   IEEE 754-2008 binary64(double precision) numbers[IEEE754] is
    //   generally available and widely used, good interoperability can be
    //   achieved by implementations that expect no more precision or range
    //   than these provide, in the sense that implementations will
    //   approximate JSON numbers within the expected precision.A JSON
    //   number such as 1E400 or 3.141592653589793238462643383279 may indicate
    //   potential interoperability problems, since it suggests that the
    //   software that created it expects receiving software to have greater
    //   capabilities for numeric magnitude and precision than is widely
    //   available.

    //   Note that when such software is used, numbers that are integers and
    //   are in the range [-(2**53)+1, (2**53)-1] are interoperable in the
    //   sense that implementations will agree exactly on their numeric
    //   values.

    //Bray Standards Track[Page 7]

    //RFC 7159                          JSON March 2014

    [TestClass]
    public class NumbersFormatTest
    {
        [TestMethod]
        public void Number_should_be_serialize_to_correct_format()
        {
            //      number = [minus] int[frac][exp]

            //      decimal-point = %x2E       ; .

            //      digit1-9 = %x31-39         ; 1-9

            //      frac = decimal-point 1* DIGIT

            //      int = zero / (digit1-9 * DIGIT )

            //      minus = %x2D               ; -

            //      plus = %x2B                ; +

            //      zero = %x30                ; 0

            int i = 1;
            var json = JsonSerializer.ToJson(i);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("1", json);

            i = -1;
            json = JsonSerializer.ToJson(i);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("-1", json);

            i = 0;
            json = JsonSerializer.ToJson(i);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("0", json);

            i = -0;
            json = JsonSerializer.ToJson(i);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreNotEqual("-0", json);
            Assert.AreEqual("0", json);

            uint ui = 123456789;
            json = JsonSerializer.ToJson(ui);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("123456789", json);

            long l = -5000000000;
            json = JsonSerializer.ToJson(l);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("-5000000000", json);

            ulong ul = 8000000000;
            json = JsonSerializer.ToJson(ul);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("8000000000", json);

            float f = 1.234f;
            json = JsonSerializer.ToJson(f);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("1.234", json);

            double d = 1.1;
            json = JsonSerializer.ToJson(d);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("1.1", json);

            d = 0.0;
            json = JsonSerializer.ToJson(d);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreNotEqual("0.0", json);
            Assert.AreEqual("0", json);

            d = -0.0;
            json = JsonSerializer.ToJson(d);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreNotEqual("-0.0", json);
            Assert.AreEqual("0", json);

            decimal m = 4.56m;
            json = JsonSerializer.ToJson(m);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("4.56", json);

            //   This specification allows implementations to set limits on the range
            //   and precision of numbers accepted.Since software that implements
            //   IEEE 754-2008 binary64(double precision) numbers[IEEE754] is
            //   generally available and widely used, good interoperability can be
            //   achieved by implementations that expect no more precision or range
            //   than these provide, in the sense that implementations will
            //   approximate JSON numbers within the expected precision.A JSON
            //   number such as 1E400 or 3.141592653589793238462643383279 may indicate
            //   potential interoperability problems, since it suggests that the
            //   software that created it expects receiving software to have greater
            //   capabilities for numeric magnitude and precision than is widely
            //   available.
            d = 3.141592653589793238462643383279;
            json = JsonSerializer.ToJson(d);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreNotEqual("3.141592653589793238462643383279", json);
        }

        [TestMethod]
        public void Scientific_notation_should_be_serialize_to_correct_format()
        {
            //      e = %x65 / %x45            ; e E
            //      exp = e[minus / plus] 1* DIGIT
            var e = 1.2e+2;
            var json = JsonSerializer.ToJson(e);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreNotEqual("1.2e+2", json);
            Assert.AreEqual("120", json);

            e = -1.2E+2;
            json = JsonSerializer.ToJson(e);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreNotEqual("-1.2E+2", json);
            Assert.AreEqual("-120", json);

            e = 1.2E+2;
            json = JsonSerializer.ToJson(e);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreNotEqual("1.2E+2", json);
            Assert.AreEqual("120", json);

            e = 1.2e-2;
            json = JsonSerializer.ToJson(e);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreNotEqual("1.2e-2", json);
            Assert.AreEqual("0.012", json);

            e = -1.2e-2;
            json = JsonSerializer.ToJson(e);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreNotEqual("-1.2e-2", json);
            Assert.AreEqual("-0.012", json);

            e = 1.2E-2;
            json = JsonSerializer.ToJson(e);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreNotEqual("1.2E-2", json);
            Assert.AreEqual("0.012", json);
        }

        //   Note that when such software is used, numbers that are integers and
        //   are in the range [-(2**53)+1, (2**53)-1] are interoperable in the
        //   sense that implementations will agree exactly on their numeric
        //   values.

        private bool IsInIntegersRange(long l)
        {
            var min = -1 * Math.Pow(2, 53) + 1;
            var max = Math.Pow(2, 53) - 1;
            return l >= min && l <= max;
        }

        [TestMethod]
        public void Integers_Range_limit_data_should_be_serialize_to_correct_format()
        {
            long l = 999999999;
            if (IsInIntegersRange(l))
            {
                var json = JsonSerializer.ToJson(l);
                Assert.IsTrue(JsonValidator.IsValid(json));
                Assert.AreEqual(l.ToString(), json);
            }
        }

        [TestMethod]
        public void NaN_should_not_be_serialize()
        {
            //Assert.ThrowsException<AssertFailedException>(() => { JsonSerializer.ToJson(double.NaN); });
            //Assert.ThrowsException<AssertFailedException>(() => { JsonSerializer.ToJson(float.NaN); });
            var json = JsonSerializer.ToJson(double.NaN);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"NaN\"", json);

            json = JsonSerializer.ToJson(float.NaN);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"NaN\"", json);
        }

        [TestMethod]
        public void PositiveInfinity_should_not_be_serialize()
        {
            var json = JsonSerializer.ToJson(double.PositiveInfinity);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"Infinity\"", json);

            json = JsonSerializer.ToJson(float.PositiveInfinity);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"Infinity\"", json);
        }

        [TestMethod]
        public void NegativeInfinity_should_not_be_serialize()
        {
            var json = JsonSerializer.ToJson(double.NegativeInfinity);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"-Infinity\"", json);

            json = JsonSerializer.ToJson(float.NegativeInfinity);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"-Infinity\"", json);
        }

        [TestMethod]
        public void IntPtr_should_be_serialize_correct()
        {
            var str = "abc";
            IntPtr p = Marshal.StringToHGlobalAnsi(str);
            var json = JsonSerializer.ToJson(p);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual(((long)p).ToString(), json);
        }

        [TestMethod]
        public void UIntPtr_should_be_serialize_correct()
        {
            UIntPtr p = new UIntPtr(0xC01E0001);
            var json = JsonSerializer.ToJson(p);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual(p.ToString(), json);
        }
    }
}
