using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kooboo.Json.Deserialize
{
    internal class SpecialTypeResolve : DefaultJsonResolve
    {
        //---------Special
        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static DataTable ReadDataTable(JsonReader reader, JsonDeserializeHandler handler)
        {
            //[{ \"Id\":1 , \"Name\":\"ss\" }]
            if (reader.ReadNullOrArrayLeft())
                return null;
            DataTable dt = new DataTable();

            int moveNext = 1;
            while (moveNext-- > 0)
            {
                reader.ReadObjLeft();
                DataRow dr = dt.NewRow();
                int move = 1;
                while (move-- > 0)
                {
                    string columnName = PrimitiveResolve.ReadEscapeString(reader,handler);
                    reader.ReadColon();
                    DataColumn column = dt.Columns[columnName];
                    if (column == null)
                    {
                        column = new DataColumn(columnName, typeof(object));
                        dt.Columns.Add(column);
                    }

                    object rowValue = PrimitiveResolve.ReadObject(reader, handler);
                    dr[columnName] = rowValue;

                    if (reader.ReadBoolComma())
                        move++;
                }
                dr.EndEdit();
                dt.Rows.Add(dr);

                reader.ReadObjRight();
                if (reader.ReadBoolComma())
                    moveNext++;
            }

            reader.ReadArrayRight();
            return dt;
        }

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static DBNull ReadDBNull(JsonReader reader, JsonDeserializeHandler handler)
        {
            char c = reader.BeforAnnotation();
            if (c == 'n' && reader.StrCompair("ull"))
                return DBNull.Value;
            throw new JsonDeserializationTypeResolutionException(typeof(DBNull));
        }

        private static ResolveDelegate<byte[]> byteArray;
        private static readonly object ObjLock = new object();
        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte[] ReadBytes(JsonReader reader, JsonDeserializeHandler handler)
        {
            char c = reader.BeforAnnotation();
            if (c == 'n' && reader.StrCompair("ull"))
                return null;
            else if (c == '[')
            {
                reader.RollbackChar();
                if (byteArray == null)
                {
                    lock (ObjLock)
                    {
                        if (byteArray == null)
                            byteArray = BuildFactory.Build<byte[]>(DeserializeBuildTypeEnum.Array, typeof(byte[]));
                    }
                }
                return byteArray(reader, handler);
            }
            else
            {
                reader.RollbackChar();
                return Convert.FromBase64String(PrimitiveResolve.ReadEscapeString(reader,handler));
            }
        }

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Guid ReadGuid(JsonReader reader, JsonDeserializeHandler handler)
        {
            var c = reader.BeforAnnotation();
            if (c == '"')
            {
                if (reader.Remaining > 36)
                {
                    // 1314FAD4-7505-439D-ABD2-DBD89242928C
                    // 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ
                    //
                    // Guid is guaranteed to be a 36 character string

                    // Bytes are in a different order than you might expect
                    // For: 35 91 8b c9 - 19 6d - 40 ea  - 97 79  - 88 9d 79 b7 53 f0 
                    // Get: C9 8B 91 35   6D 19   EA 40    97 79    88 9D 79 B7 53 F0 
                    // Ix:   0  1  2  3    4  5    6  7     8  9    10 11 12 13 14 15
                    var asStruct = new GuidStruct
                    {
                        B03 = ReadGuidByte(reader),
                        B02 = ReadGuidByte(reader),
                        B01 = ReadGuidByte(reader),
                        B00 = ReadGuidByte(reader)
                    };

                    c = reader.GetChar();
                    if (c != '-')
                        throw new JsonWrongCharacterException(reader, "Guid format error");

                    asStruct.B05 = ReadGuidByte(reader);
                    asStruct.B04 = ReadGuidByte(reader);

                    c = reader.GetChar();
                    if (c != '-')
                        throw new JsonWrongCharacterException(reader, "Guid format error");

                    asStruct.B07 = ReadGuidByte(reader);
                    asStruct.B06 = ReadGuidByte(reader);

                    c = reader.GetChar();
                    if (c != '-')
                        throw new JsonWrongCharacterException(reader, "Guid format error");

                    asStruct.B08 = ReadGuidByte(reader);
                    asStruct.B09 = ReadGuidByte(reader);

                    c = reader.GetChar();
                    if (c != '-')
                        throw new JsonWrongCharacterException(reader, "Guid format error");

                    asStruct.B10 = ReadGuidByte(reader);
                    asStruct.B11 = ReadGuidByte(reader);
                    asStruct.B12 = ReadGuidByte(reader);
                    asStruct.B13 = ReadGuidByte(reader);
                    asStruct.B14 = ReadGuidByte(reader);
                    asStruct.B15 = ReadGuidByte(reader);
                    if (reader.GetChar() == '"')
                        return asStruct.Value;
                }
            }
            throw new JsonDeserializationTypeResolutionException(reader, typeof(Guid));
        }

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Uri ReadUri(JsonReader reader, JsonDeserializeHandler handler)
        {
            char c = reader.BeforAnnotation();
            if (c == 'n' && reader.StrCompair("ull"))
                return null;
            else
            {
                reader.RollbackChar();
                return new Uri(PrimitiveResolve.ReadEscapeString(reader,handler));
            }
        }

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static NameValueCollection ReadNameValueCollection(JsonReader reader, JsonDeserializeHandler handler)
        {
            if (reader.ReadNullOrObjLeft())
                return null;

            NameValueCollection nameValueCollection = new NameValueCollection();

            int moveNext = 1;
            while (moveNext-- > 0)
            {
                var key = PrimitiveResolve.ReadEscapeString( reader, handler);
                reader.ReadColon();
                var value = PrimitiveResolve.ReadEscapeString(reader, handler);
                nameValueCollection.Add(key, value);
                if (reader.ReadBoolComma())
                    moveNext++;
            }
            reader.ReadObjRight();
            return nameValueCollection;
        }

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static StringDictionary ReadStringDictionary(JsonReader reader, JsonDeserializeHandler handler)
        {
            if (reader.ReadNullOrObjLeft())
                return null;

            StringDictionary nameValueCollection = new StringDictionary();

            int moveNext = 1;
            while (moveNext-- > 0)
            {
                var key = PrimitiveResolve.ReadEscapeString(reader, handler);
                reader.ReadColon();
                var value = PrimitiveResolve.ReadEscapeString(reader, handler);
                nameValueCollection.Add(key, value);
                if (reader.ReadBoolComma())
                    moveNext++;
            }
            reader.ReadObjRight();
            return nameValueCollection;
        }

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ExpandoObject ReadExpandoObject(JsonReader reader, JsonDeserializeHandler handler)
        {
            if (reader.ReadNullOrObjLeft())
                return null;

            ExpandoObject obj = new ExpandoObject();
            var dic = (IDictionary<string, object>)obj;

            int moveNext = 1;
            while (moveNext-- > 0)
            {
                var key = PrimitiveResolve.ReadEscapeString(reader, handler);
                reader.ReadColon();
                var value = PrimitiveResolve.ReadObject(reader, handler);
                dic.Add(key, value);
                if (reader.ReadBoolComma())
                    moveNext++;
            }
            reader.ReadObjRight();
            return obj;
        }

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static StringBuilder ReadStringBuilder(JsonReader reader, JsonDeserializeHandler handler)
        {
            var c = reader.BeforAnnotation();
            if (c == '"')
            {
                int start = reader.Length - reader.Remaining;
                int length = 0;
                StringBuilder charBufferSb = new StringBuilder();
                while (reader.Remaining > 0)
                {
                    c = reader.GetChar();
                    if (c == '"')//end
                    {
                        if (length > 0)
                        {
                            if (reader.Json != null)
                                charBufferSb.Append(reader.Json, start, length);
                            else
                                charBufferSb.Append(reader.Buffer, start, length);
                        }
                        return charBufferSb;
                    }

                    if (c != '\\')
                    {
                        ++length;
                        continue;
                    }
                    if (length > 0)
                    {
                        if (reader.Json != null)
                            charBufferSb.Append(reader.Json, start, length);
                        else
                            charBufferSb.Append(reader.Buffer, start, length);
                        start += length;
                        length = 0;
                    }

                    /*
                     string s = "a\\\\b"; 
                     string s = "a\\u005cb";
                     */
                    start += 2;
                    if (reader.Remaining < 1)
                        throw new JsonDeserializationTypeResolutionException(reader, typeof(string));

                    c = reader.GetChar();
                    switch (c)
                    {
                        case '"': charBufferSb.Append('"'); continue;
                        case '\\': charBufferSb.Append('\\'); continue;
                        case '/': charBufferSb.Append('/'); continue;
                        case 'b': charBufferSb.Append('\b'); continue;
                        case 'f': charBufferSb.Append('\f'); continue;
                        case 'n': charBufferSb.Append('\n'); continue;
                        case 'r': charBufferSb.Append('\r'); continue;
                        case 't': charBufferSb.Append('\t'); continue;
                        case 'u':
                            {   // \\uXXXX
                                if (reader.Remaining > 4)//4+'"'
                                {
                                    int idx = charBufferSb.Length;
                                    charBufferSb.Append(reader.GetChar());
                                    charBufferSb.Append(reader.GetChar());
                                    charBufferSb.Append(reader.GetChar());
                                    charBufferSb.Append(reader.GetChar());
                                    int unicode = (((((charBufferSb[idx].CharToNumber() * 16) + charBufferSb[++idx].CharToNumber()) * 16) + charBufferSb[++idx].CharToNumber()) * 16) + charBufferSb[++idx].CharToNumber();
                                    charBufferSb.Remove(idx - 3, 4);
                                    charBufferSb.Append((char)unicode);

                                    start += 4;
                                    continue;
                                }
                                throw new JsonWrongCharacterException(reader, "It should be four hexadecimal digits , such as \\uFFFF");
                            }
                        default:
                            throw new JsonWrongCharacterException(reader, c);
                    }
                }
            }
            else if (c == 'n' && reader.StrCompair("ull"))
                return null;

            throw new JsonDeserializationTypeResolutionException(reader, typeof(string));

        }

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static BitArray ReadBitArray(JsonReader reader, JsonDeserializeHandler handler)
        {
            if (reader.ReadNullOrArrayLeft())
                return null;

            if (reader.ReadBoolArrayRight())
                return new BitArray(0);

            bool[] values = new bool[reader.GetArrayLength(handler)];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = PrimitiveResolve.ReadBool(reader, handler);
                if (i != values.Length - 1)
                    reader.ReadComma();
            }
            reader.ReadArrayRight();
            return new BitArray(values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ReadGuidByte(JsonReader reader)
        {
            int a = reader.GetChar();

            if (!((a >= '0' && a <= '9') || (a >= 'A' && a <= 'F') || (a >= 'a' && a <= 'f')))
                throw new JsonWrongCharacterException(reader, "Guid format error");

            int b = reader.GetChar();

            if (!((b >= '0' && b <= '9') || (b >= 'A' && b <= 'F') || (b >= 'a' && b <= 'f')))
                throw new JsonWrongCharacterException(reader, "Guid format error");

            if (a <= '9')
                a -= '0';
            else
            {
                if (a <= 'F')
                    a -= ('A' - 10);
                else
                    a -= ('a' - 10);
            }

            if (b <= '9')
                b -= '0';
            else
            {
                if (b <= 'F')
                    b -= ('A' - 10);
                else
                    b -= ('a' - 10);
            }

            return (byte)(a * 16 + b);
        }
    }
}
