using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobooJson.Benchmark
{
    
    static class Helper
    {
        static readonly char[] ASCII;
        static Random Rand = new Random(314159265);

        static Helper()
        {
            var cs = new List<char>();

            for (var i = 0; i <= byte.MaxValue; i++)
            {
                var c = (char)i;
                if (char.IsControl(c)) continue;

                cs.Add(c);
            }

            ASCII = cs.ToArray();
        }
        public static object RandomValue(this Type t, int depth = 0)
        {
            if (t.IsPrimitive)
            {
                if (t == typeof(byte))
                {
                    return (byte)(Rand.Next(byte.MaxValue - byte.MinValue + 1) + byte.MinValue);
                }

                if (t == typeof(sbyte))
                {
                    return (sbyte)(Rand.Next(sbyte.MaxValue - sbyte.MinValue + 1) + sbyte.MinValue);
                }

                if (t == typeof(short))
                {
                    return (short)(Rand.Next(short.MaxValue - short.MinValue + 1) + short.MinValue);
                }

                if (t == typeof(ushort))
                {
                    return (ushort)(Rand.Next(ushort.MaxValue - ushort.MinValue + 1) + ushort.MinValue);
                }

                if (t == typeof(int))
                {
                    var bytes = new byte[4];
                    Rand.NextBytes(bytes);

                    return BitConverter.ToInt32(bytes, 0);
                }

                if (t == typeof(uint))
                {
                    var bytes = new byte[4];
                    Rand.NextBytes(bytes);

                    return BitConverter.ToUInt32(bytes, 0);
                }

                if (t == typeof(long))
                {
                    var bytes = new byte[8];
                    Rand.NextBytes(bytes);

                    return BitConverter.ToInt64(bytes, 0);
                }

                if (t == typeof(ulong))
                {
                    var bytes = new byte[8];
                    Rand.NextBytes(bytes);

                    return BitConverter.ToUInt64(bytes, 0);
                }

                if (t == typeof(float))
                {
                    var bytes = new byte[4];
                    Rand.NextBytes(bytes);

                    return BitConverter.ToSingle(bytes, 0);
                }

                if (t == typeof(double))
                {
                    var bytes = new byte[8];
                    Rand.NextBytes(bytes);

                    return BitConverter.ToDouble(bytes, 0);
                }

                if (t == typeof(char))
                {
                    var roll = Rand.Next(ASCII.Length);

                    return ASCII[roll];
                }

                if (t == typeof(bool))
                {
                    return (Rand.Next(2) == 1);
                }

                throw new InvalidOperationException();
            }

            if (t == typeof(decimal))
            {
                return new decimal((int)typeof(int).RandomValue(), (int)typeof(int).RandomValue(), (int)typeof(int).RandomValue(),false,28);
            }

            if (t == typeof(string))
            {
                var len = Rand.Next(40);
                var c = new char[len];
                for (var i = 0; i < c.Length; i++)
                {
                    c[i] = (char)typeof(char).RandomValue(depth + 1);
                }

                return new string(c);
            }

            if (t == typeof(DateTime))
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                var bytes = new byte[4];
                Rand.NextBytes(bytes);

                var secsOffset = BitConverter.ToInt32(bytes, 0);

                var retDate = epoch.AddSeconds(secsOffset);

                return retDate;
            }

            if (t.IsNullable())
            {
                // leave it unset
                if (Rand.Next(2) == 0)
                {
                    // null!
                    return Activator.CreateInstance(t);
                }

                var underlying = Nullable.GetUnderlyingType(t);
                var val = underlying.RandomValue(depth + 1);

                var cons = t.GetConstructor(new[] { underlying });

                return cons.Invoke(new object[] { val });
            }

            if (t.IsEnum)
            {
                var allValues = Enum.GetValues(t);
                var ix = Rand.Next(allValues.Length);

                return allValues.GetValue(ix);
            }

            if (t.IsArray)
            {
                var valType = t.GetElementType();
                var len = Rand.Next(20, 50);
                var ret = Array.CreateInstance(valType, len);
                //var add = t.GetMethod("SetValue");
                for (var i = 0; i < len; i++)
                {
                    var elem = valType.RandomValue(depth + 1);
                    ret.SetValue(elem, i);
                }

                return ret;
            }

            if (t.IsList())
            {
                var valType = t.GetGenericArguments()[0];
                var ret = Activator.CreateInstance(t);
                var add = t.GetMethod("Add");

                var len = Rand.Next(20, 50);
                for (var i = 0; i < len; i++)
                {
                    var elem = valType.RandomValue(depth + 1);
                    add.Invoke(ret, new object[] { elem });
                }

                return ret;
            }

            if (t.IsDictionary())
            {
                var valType = t.GetGenericArguments()[1];
                var ret = Activator.CreateInstance(t);
                var add = t.GetMethod("Add");

                var len = Rand.Next(30);
                for (var i = 0; i < len; i++)
                {
                    var key = (string)typeof(string).RandomValue(depth + 1);
                    if (key == null)
                    {
                        i--;
                        continue;
                    }

                    var val = valType.RandomValue(depth + 1);

                    add.Invoke(ret, new object[] { key, val });
                }

                return ret;

            }

            if (t == typeof(Guid))
                return Guid.NewGuid();

            //model
            var retObj = Activator.CreateInstance(t);
            foreach (var p in t.GetFields())
            {
                //if (Rand.Next(5) == 0) continue;

                var fieldType = p.FieldType;

                p.SetValue(retObj, fieldType.RandomValue(depth + 1));
            }

            return retObj;
        }
        public static bool IsNullable(this Type t)
        {
            return Nullable.GetUnderlyingType(t) != null;
        }
        public static bool IsList(this Type t)
        {
            return
                (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>));
        }
        public static bool IsDictionary(this Type t)
        {
            return
                (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>));
        }

        public static bool IsEqual(this Entity[] a, Entity[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (!a[i].IsEqual(b[i]))
                    return false;
            }
            return true;
        }
        public static bool IsEqual(this Dictionary<string, Entity> a, Dictionary<string, Entity> b)
        {
            foreach (var item in a)
            {
                if (!item.Value.IsEqual(b[item.Key]))
                    return false;
            }
            return true;
        }

        public static bool IsEqual(this List<Entity> a, List<Entity> b)
        {
            for (int i = 0; i < a.Count; i++)
            {
                if (!a[i].IsEqual(b[i]))
                    return false;
            }
            return true;
        }

        public static bool IsEqual (this Entity r,Entity b)
        {
            return b.A == r.A && b.B == r.B && b.C == r.C && b.D == r.D && b.E == r.E && /*(b.F == r.F) ? true : (b.F.ToString() == r.F.ToString()) && *//*b.I == r.I && b.J == r.J &&*/ /*(b.K == r.K) ? true : (b.K.ToString() == r.K.ToString()) &&*/ b.R == r.R && b.M == r.M && b.N == r.N ? true : (b.N.ToString() == r.N.ToString());
        }
    }
}
