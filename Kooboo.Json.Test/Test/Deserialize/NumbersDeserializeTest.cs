using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class NumbersDeserializeTest
    {
        [TestMethod]
        public void int_deserialize_should_be_correct()
        {
            var str = int.MaxValue.ToString();
            var res = JsonSerializer.ToObject<int>(str);
            Assert.AreEqual(int.MaxValue, res);

            str = "0";
            res = JsonSerializer.ToObject<int>(str);
            Assert.AreEqual(0, res);

            str = int.MinValue.ToString();
            res = JsonSerializer.ToObject<int>(str);
            Assert.AreEqual(int.MinValue, res);
        }

        [TestMethod]
        public void uint_deserialize_should_be_correct()
        {
            var str = uint.MaxValue.ToString();
            var res = JsonSerializer.ToObject<uint>(str);
            Assert.AreEqual(uint.MaxValue, res);

            str = "0";
            res = JsonSerializer.ToObject<uint>(str);
            Assert.AreEqual((uint)0, res);

            str = uint.MinValue.ToString();
            res = JsonSerializer.ToObject<uint>(str);
            Assert.AreEqual(uint.MinValue, res);
        }

        [TestMethod]
        public void null_int_deserialize_should_be_correct()
        {
            var res = JsonSerializer.ToObject<int?>("null");
            Assert.AreEqual(null, res);
        }

        [TestMethod]
        public void short_deserialize_should_be_correct()
        {
            var str = short.MaxValue.ToString();
            var res = JsonSerializer.ToObject<short>(str);
            Assert.AreEqual(short.MaxValue, res);

            str = "0";
            res = JsonSerializer.ToObject<short>(str);
            Assert.AreEqual((short)0, res);

            str = short.MinValue.ToString();
            res = JsonSerializer.ToObject<short>(str);
            Assert.AreEqual(short.MinValue, res);
        }

        [TestMethod]
        public void ushort_deserialize_should_be_correct()
        {
            var str = ushort.MaxValue.ToString();
            var res = JsonSerializer.ToObject<ushort>(str);
            Assert.AreEqual(ushort.MaxValue, res);

            str = "0";
            res = JsonSerializer.ToObject<ushort>(str);
            Assert.AreEqual((ushort)0, res);

            str = ushort.MinValue.ToString();
            res = JsonSerializer.ToObject<ushort>(str);
            Assert.AreEqual(ushort.MinValue, res);
        }

        [TestMethod]
        public void long_deserialize_should_be_correct()
        {
            var str = long.MaxValue.ToString();
            var res = JsonSerializer.ToObject<long>(str);
            Assert.AreEqual(long.MaxValue, res);

            str = "0";
            res = JsonSerializer.ToObject<long>(str);
            Assert.AreEqual(0L, res);

            str = long.MinValue.ToString();
            res = JsonSerializer.ToObject<long>(str);
            Assert.AreEqual(long.MinValue, res);
        }

        [TestMethod]
        public void ulong_deserialize_should_be_correct()
        {
            var str = ulong.MaxValue.ToString();
            var res = JsonSerializer.ToObject<ulong>(str);
            Assert.AreEqual(ulong.MaxValue, res);

            str = "0";
            res = JsonSerializer.ToObject<ulong>(str);
            Assert.AreEqual((ulong)0, res);

            str = ulong.MinValue.ToString();
            res = JsonSerializer.ToObject<ulong>(str);
            Assert.AreEqual(ulong.MinValue, res);
        }

        [TestMethod]
        public void decimal_deserialize_should_be_correct()
        {
            var str = decimal.MaxValue.ToString();
            var res = JsonSerializer.ToObject<decimal>(str);
            Assert.AreEqual(decimal.MaxValue, res);

            str = "0";
            res = JsonSerializer.ToObject<decimal>(str);
            Assert.AreEqual(0m, res);

            str = "0.0";
            res = JsonSerializer.ToObject<decimal>(str);
            Assert.AreEqual(0m, res);

            str = (-11.11m).ToString();
            res = JsonSerializer.ToObject<decimal>(str);
            Assert.AreEqual(-11.11m, res);

            str = decimal.MinValue.ToString();
            res = JsonSerializer.ToObject<decimal>(str);
            Assert.AreEqual(decimal.MinValue, res);

            const string sep = ".";
            string[] string_values = new string[7];
            string_values[0] = "1";
            string_values[1] = "1" + sep + "1";
            string_values[2] = "-12";
            string_values[3] = "44" + sep + "444432";
            string_values[4] = "         -221" + sep + "3233";
            string_values[5] = "6" + sep + "28318530717958647692528676655900577";
            string_values[6] = "1e-05";
            foreach (var s in string_values)
            {
                res = JsonSerializer.ToObject<decimal>(s);
                Assert.AreEqual(decimal.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture), res);
            }
        }

        [TestMethod]
        public void float_deserialize_should_be_correct()
        {
            var str = float.MaxValue.ToString("R");
            var res = JsonSerializer.ToObject<float>(str);
            Assert.AreEqual(float.MaxValue, res);

            str = "0";
            res = JsonSerializer.ToObject<float>(str);
            Assert.AreEqual(0f, res);

            str = "0.0";
            res = JsonSerializer.ToObject<float>(str);
            Assert.AreEqual(0f, res);

            str = (-11.11f).ToString();
            res = JsonSerializer.ToObject<float>(str);
            Assert.AreEqual(-11.11f, res);

            str = float.MinValue.ToString("R");
            res = JsonSerializer.ToObject<float>(str);
            Assert.AreEqual(float.MinValue, res);

            const string sep = ".";
            string[] string_values = new string[7];
            string_values[0] = "1";
            string_values[1] = "1" + sep + "1";
            string_values[2] = "-12";
            string_values[3] = "44" + sep + "444432";
            string_values[4] = "         -221" + sep + "3233";
            string_values[5] = "6" + sep + "28318530717958647692528676655900577";
            string_values[6] = "1e-05";
            foreach (var s in string_values)
            {
                res = JsonSerializer.ToObject<float>(s);
                Assert.AreEqual(float.Parse(s, CultureInfo.InvariantCulture), res);
            }
        }

        [TestMethod]
        public void double_deserialize_should_be_correct()
        {
            var str = double.MaxValue.ToString("R");
            var res = JsonSerializer.ToObject<double>(str);
            Assert.AreEqual(double.MaxValue, res);

            str = "0";
            res = JsonSerializer.ToObject<double>(str);
            Assert.AreEqual(0.0, res);

            str = "0.0";
            res = JsonSerializer.ToObject<double>(str);
            Assert.AreEqual(0.0, res);

            str = (-11.11).ToString();
            res = JsonSerializer.ToObject<double>(str);
            Assert.AreEqual(-11.11, res);

            str = double.MinValue.ToString("R");
            res = JsonSerializer.ToObject<double>(str);
            Assert.AreEqual(double.MinValue, res);

            const string sep = ".";
            string[] string_values = new string[10];
            string_values[0] = "1";
            string_values[1] = "1" + sep + "1";
            string_values[2] = "-12";
            string_values[3] = "44" + sep + "444432";
            string_values[4] = "         -221" + sep + "3233";
            string_values[5] = " 1" + sep + "7976931348623157e308 ";
            string_values[6] = "-1" + sep + "7976931348623157e308";
            string_values[7] = "4" + sep + "9406564584124650e-324";
            string_values[8] = "6" + sep + "28318530717958647692528676655900577";
            string_values[9] = "1e-05";
            foreach (var s in string_values)
            {
                res = JsonSerializer.ToObject<double>(s);
                Assert.AreEqual(double.Parse(s, CultureInfo.InvariantCulture), res);
            }
        }

        [TestMethod]
        public void IntPtr_should_be_deserialize_correct()
        {
            var str = "abc";
            IntPtr p = Marshal.StringToHGlobalAnsi(str);
            var json = JsonSerializer.ToJson(p);
            Assert.AreEqual(((long)p).ToString(), json);

            var res = JsonSerializer.ToObject<IntPtr>(json);
            Assert.AreEqual(p, res);
        }

        [TestMethod]
        public void UIntPtr_should_be_deserialize_correct()
        {
            UIntPtr p = new UIntPtr(0xC01E0001);
            var json = JsonSerializer.ToJson(p);
            Assert.AreEqual(p.ToString(), json);

            var res = JsonSerializer.ToObject<UIntPtr>(json);
            Assert.AreEqual(p, res);
        }

        [TestMethod]
        public void Minus_number_deserialize_should_be_correct()
        {
            var str = "-0";
            var res = JsonSerializer.ToObject<int>(str);
            Assert.AreEqual(-0, res);

            str = "-1.2";
            Assert.ThrowsException<JsonWrongCharacterException>(() =>
            {
                JsonSerializer.ToObject<int>(str);
            });

            var res2 = JsonSerializer.ToObject<float>(str);
            Assert.AreEqual(-1.2f, res2);

            var res3 = JsonSerializer.ToObject<double>(str);
            Assert.AreEqual(-1.2, res3);

            var res4 = JsonSerializer.ToObject<decimal>(str);
            Assert.AreEqual(-1.2m, res4);
        }

        [TestMethod]
        public void overflow_number_deserialize_should_not_be_correct()
        {
            Assert.ThrowsException<OverflowException>(() =>
            {
                JsonSerializer.ToObject<short>("32768");
            });

            Assert.ThrowsException<OverflowException>(() =>
            {
                JsonSerializer.ToObject<short>("-32769");
            });

            Assert.ThrowsException<OverflowException>(() =>
            {
                JsonSerializer.ToObject<ushort>("65536");
            });

            Assert.ThrowsException<OverflowException>(() =>
            {
                JsonSerializer.ToObject<int>("2147483648");
            });

            Assert.ThrowsException<OverflowException>(() =>
            {
                JsonSerializer.ToObject<int>("-2147483649");
            });

            Assert.ThrowsException<OverflowException>(() =>
            {
                JsonSerializer.ToObject<uint>("4294967296");
            });

            Assert.ThrowsException<OverflowException>(() =>
            {
                JsonSerializer.ToObject<long>("9223372036854775808");
            });

            Assert.ThrowsException<OverflowException>(() =>
            {
                JsonSerializer.ToObject<long>("-9223372036854775809");
            });

            Assert.ThrowsException<OverflowException>(() =>
            {
                JsonSerializer.ToObject<ulong>("18446744073709551616");
            });
        }
    }
}
