using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kooboo.Json.Deserialize
{
    internal unsafe class JsonReader
    {
        #region pregenerated metas
        internal static FieldInfo _Json =
            typeof(JsonReader).GetField(nameof(Json), BindingFlags.NonPublic | BindingFlags.Instance);

        internal static FieldInfo _Buffer =
           typeof(JsonReader).GetField(nameof(Buffer), BindingFlags.NonPublic | BindingFlags.Instance);

        internal static FieldInfo _Length =
            typeof(JsonReader).GetField(nameof(Length), BindingFlags.NonPublic | BindingFlags.Instance);

        internal static FieldInfo _Remaining =
            typeof(JsonReader).GetField(nameof(Remaining), BindingFlags.NonPublic | BindingFlags.Instance);

        internal static MethodInfo _RollbackChar = GetMethodInfo(nameof(RollbackChar));

        internal static MethodInfo _Rollback = GetMethodInfo(nameof(Rollback));

        internal static MethodInfo _ReadObjLeft = GetMethodInfo(nameof(ReadObjLeft));

        internal static MethodInfo _ReadNullOrObjLeft = GetMethodInfo(nameof(ReadNullOrObjLeft));

        internal static MethodInfo _ReadObjRight = GetMethodInfo(nameof(ReadObjRight));

        internal static MethodInfo _ReadBoolObjRight = GetMethodInfo(nameof(ReadBoolObjRight));

        internal static MethodInfo _ReadBoolComma = GetMethodInfo(nameof(ReadBoolComma));

        internal static MethodInfo _ReadComma = GetMethodInfo(nameof(ReadComma));

        internal static MethodInfo _ReadColon = GetMethodInfo(nameof(ReadColon));

        internal static MethodInfo _ReadQuotes = GetMethodInfo(nameof(ReadQuotes));

        internal static MethodInfo _ReadArrayLeft = GetMethodInfo(nameof(ReadArrayLeft));

        internal static MethodInfo _ReadNullOrArrayLeft = GetMethodInfo(nameof(ReadNullOrArrayLeft));

        internal static MethodInfo _ReadArrayRight = GetMethodInfo(nameof(ReadArrayRight));

        internal static MethodInfo _ReadBoolArrayRight = GetMethodInfo(nameof(ReadBoolArrayRight));

        internal static MethodInfo _ReadEnd = GetMethodInfo(nameof(ReadEnd));

        internal static MethodInfo _BeforAnnotation = GetMethodInfo(nameof(BeforAnnotation));

        internal static MethodInfo _ReadAnnotation = GetMethodInfo(nameof(ReadAnnotation));

        internal static MethodInfo _ReadString = GetMethodInfo(nameof(ReadString));

        internal static MethodInfo _ReadBoolNull = GetMethodInfo(nameof(ReadBoolNull));

        internal static MethodInfo _SkipObj = GetMethodInfo(nameof(SkipObj));

        internal static MethodInfo _GetArrayLength = GetMethodInfo(nameof(GetArrayLength));

        internal static MethodInfo _RemoveQuoteAndSubString = GetMethodInfo(nameof(RemoveQuoteAndSubString));

        internal static MethodInfo _GetChar = GetMethodInfo(nameof(GetChar));
        #endregion

        internal StringBuilder CharBufferSb;
        internal string Json;
        internal char[] Buffer;
        internal int Length;
        internal char* Pointer;
        internal int Remaining;


        internal JsonReader(string json, char* c)
        {
            Json = json;
            Length = json.Length;
            Pointer = c;
            Remaining = Length;
            CharBufferSb = null;
            Buffer = null;
        }

        internal JsonReader(char[] buffer,int length, char* c)
        {
            Buffer = buffer;
            Length = length;
            Pointer = c;
            Remaining = Length;
            CharBufferSb = null;
            Json = null;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal string SubString(int start, int length)
        {
            if (Json != null)
                return Json.Substring(start, length);
            else//char[]
                return new string(Buffer, start, length);
        }

        internal char this[int idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (Json != null)
                    return Json[idx];
                else
                    return Buffer[idx];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal char GetChar()
        {
            var c = *Pointer;
            Pointer++;
            Remaining--;
            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetInt()
        {
            var i = *(int*)Pointer;
            Pointer += 2;
            Remaining -= 2;
            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal long GetLong()
        {
            var i = *(long*)Pointer;
            Pointer += 4;
            Remaining -= 4;
            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RollbackChar()
        {
            Pointer--;
            Remaining++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RollbackInt()
        {
            Pointer -= 2;
            Remaining += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Rollback(int num)
        {
            Pointer -= num;
            Remaining += num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReadObjLeft()
        {
            var c = BeforAnnotation();
            if (c != '{')
                throw new JsonWrongCharacterException(this, '{');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool ReadNullOrObjLeft()
        {
            var c = BeforAnnotation();
            if (c == 'n' && StrCompair("ull"))
                return true;
            if (c == '{')
                return false;
            throw new JsonWrongCharacterException(this, '{');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReadObjRight()
        {
            var c = BeforAnnotation();
            if (c != '}')
                throw new JsonWrongCharacterException(this, '}');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool ReadBoolObjRight()
        {
            var c = BeforAnnotation();
            if (c == '}') return true;

            RollbackChar();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool ReadBoolComma()
        {
            var c = BeforAnnotation();
            if (c == ',') return true;

            RollbackChar();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReadComma()
        {
            var c = BeforAnnotation();
            if (c != ',')
                throw new JsonWrongCharacterException(this, ',');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReadColon()
        {
            var c = BeforAnnotation();
            if (c != ':')
                throw new JsonWrongCharacterException(this, ':');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReadQuotes()
        {
            var c = BeforAnnotation();
            if (c != '"')
                throw new JsonWrongCharacterException(this, '"');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReadArrayLeft()
        {
            var c = BeforAnnotation();
            if (c != '[')
                throw new JsonWrongCharacterException(this, '[');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool ReadNullOrArrayLeft()
        {
            var c = BeforAnnotation();
            if (c == 'n' && StrCompair("ull"))
                return true;
            if (c == '[')
                return false;
            throw new JsonWrongCharacterException(this, '[');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReadArrayRight()
        {
            var c = BeforAnnotation();
            if (c != ']')
                throw new JsonWrongCharacterException(this, ']');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool ReadBoolArrayRight()
        {
            var c = BeforAnnotation();
            if (c == ']') return true;

            RollbackChar();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool ReadBoolNull()
        {
            var c = BeforAnnotation();
            if (c == 'n' && StrCompair("ull"))
                return true;
            else
            {
                RollbackChar();
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReadEnd()
        {
            if (Remaining > 0)
                ReadAnnotation();
            else if (Remaining < 0)
                throw new JsonWrongCharacterException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal char BeforAnnotation()
        {
            var c = GetChar();
            switch (c)
            {
                case '/':
                    {
                        if (Remaining == 0)
                            throw new JsonWrongCharacterException();
                        c = GetChar();
                        if (c == '*')
                        {
                            var lastChar = ' ';
                            while (Remaining > 0)
                            {
                                lastChar = c;
                                c = GetChar();
                                if (c == '/') //     /* * */ ， /* ***/////
                                    if (lastChar == '*')
                                    {
                                        if (Remaining > 0)
                                        {
                                            c = GetChar();
                                            if (c == '\r' || c == '\n' || c == ' ' || c == '\t')
                                                return BeforAnnotation();
                                            return c;
                                        }

                                        throw new JsonWrongCharacterException("Json is incomplete"); //json不完整
                                    }
                            }
                        }
                        else if (c == '/')
                        {
                            while (Remaining > 0)
                            {
                                c = GetChar();
                                if (c == '\r' || c == '\n')
                                {
                                    if (Remaining > 0)
                                    {
                                        c = GetChar();
                                        if (c == '\r' || c == '\n' || c == ' ' || c == '\t')
                                            return BeforAnnotation();
                                        return c;
                                    }

                                    throw new JsonWrongCharacterException("Json is incomplete"); //json不完整
                                }
                            }
                        }

                        throw new JsonWrongCharacterException();
                    }
                case '\r':
                case '\n':
                case '\t':
                case ' ':
                    if (Remaining > 0)
                        return BeforAnnotation();
                    else
                        throw new JsonWrongCharacterException("Json is incomplete"); //json不完整
            }

            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReadAnnotation()
        {
            var c = GetChar();
            switch (c)
            {
                case '/':
                    {
                        if (Remaining == 0)
                            throw new JsonWrongCharacterException();
                        c = GetChar();
                        if (c == '*')
                        {
                            var lastChar = ' ';
                            while (Remaining > 0)
                            {
                                lastChar = c;
                                c = GetChar();
                                if (c == '/') //     /* * */ ， /* ***/////
                                    if (lastChar == '*')
                                    {
                                        if (Remaining > 0)
                                            ReadAnnotation();
                                        return;
                                    }
                            }
                        }
                        else if (c == '/')
                        {
                            while (Remaining > 0)
                            {
                                c = GetChar();
                                if (c == '\r' || c == '\n')
                                {
                                    if (Remaining > 0)
                                        ReadAnnotation();
                                    return;
                                }
                            }
                            return;
                        }
                        break;
                    }
                case '\r':
                case '\n':
                case '\t':
                case ' ':
                    if (Remaining > 0)
                        ReadAnnotation();
                    return;
            }

            throw new JsonWrongCharacterException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal string ReadString()
        {
            //Currently, this method is only used for dynamic read keys and enums because it does not allow special characters
            var c = BeforAnnotation();
            if (c == '"')
            {
                var idx = Length - Remaining;
                var leng = 0;
                var isCorrectFormat = false;
                while (Remaining > 0)
                {
                    c = GetChar();
                    if (c == '"')
                    {
                        isCorrectFormat = true;
                        break;
                    }

                    leng++;
                }

                if (isCorrectFormat)
                    if (leng == 0)
                        return string.Empty;
                    else
                        return this.SubString(idx, leng);
            }
            else if (c == 'n' && StrCompair("ull"))
            {
                return null;
            }

            throw new JsonDeserializationTypeResolutionException(this, typeof(string));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int SkipObj(JsonDeserializeHandler handler)
        {
            var c = BeforAnnotation();
            var start = Remaining;
            switch (c)
            {
                case 'f':
                    if (Remaining > 3)
                        if (StrCompair("alse"))
                            goto Return;
                    break;
                case 't':
                    if (Remaining > 2)
                        if (StrCompair("rue"))
                            goto Return;
                    break;
                case 'n':
                    if (Remaining > 2)
                        if (StrCompair("ull"))
                            goto Return;
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    {
                        SkipNumber();
                        goto Return;
                    }
                case '[':
                    {
                        if (ReadBoolArrayRight())
                            goto Return;
                        var moveNext = 1;
                        while (moveNext-- > 0)
                        {
                            SkipObj(handler);
                            if (ReadBoolComma())
                                moveNext++;
                        }

                        ReadArrayRight();
                        goto Return;
                    }
                case '{':
                    {
                        if (ReadBoolObjRight())
                            goto Return;
                        var moveNext = 1;
                        while (moveNext-- > 0)
                        {
                            SkipString();
                            ReadColon();
                            SkipObj(handler);
                            if (ReadBoolComma())
                                moveNext++;
                        }

                        ReadObjRight();
                        goto Return;
                    }
                case '"':
                    {
                        RollbackChar();
                        SkipString();
                        goto Return;
                    }
            }

            throw new JsonWrongCharacterException(this);
        Return:
            return start + 1 - Remaining;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SkipString()
        {
            var c = BeforAnnotation();
            if (c == '"')
                while (Remaining > 0)
                {
                    c = GetChar();
                    if (c == '"')
                        return;
                    if (c != '\\')
                        continue;
                    c = GetChar();
                    switch (c)
                    {
                        case '"': continue;
                        case '\\': continue;
                        case '/': continue;
                        case 'b': continue;
                        case 'f': continue;
                        case 'n': continue;
                        case 'r': continue;
                        case 't': continue;
                        case 'u':
                            {
                                c = GetChar();
                                if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'))) throw new JsonDeserializationTypeResolutionException(this, typeof(string));
                                c = GetChar();
                                if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'))) throw new JsonDeserializationTypeResolutionException(this, typeof(string));
                                c = GetChar();
                                if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'))) throw new JsonDeserializationTypeResolutionException(this, typeof(string));
                                c = GetChar();
                                if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'))) throw new JsonDeserializationTypeResolutionException(this, typeof(string));
                                continue;
                            }
                    }
                }
            else if (c == 'n' && StrCompair("ull")) return;

            throw new JsonDeserializationTypeResolutionException(this, typeof(string));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SkipNumber()
        {
            var seenDecimal = false;
            var seenExponent = false;
            char c;
            while (Remaining > 0)
            {
                c = GetChar();
                if (c >= '0' && c <= '9') continue;

                if (c == '.' && !seenDecimal)
                {
                    seenDecimal = true;
                    continue;
                }

                if ((c == 'e' || c == 'E') && !seenExponent)
                {
                    c = GetChar();
                    seenExponent = true;
                    seenDecimal = true;

                    c = GetChar();
                    if (c == '-' || c == '+' || c >= '0' && c <= '9') continue;
                    throw new JsonWrongCharacterException(this, "Expected -, or a digit");
                }

                RollbackChar();

                return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetArrayLength(JsonDeserializeHandler handler)
        {
            //ReadObjLeft();
            if (ReadBoolArrayRight())
            {
                RollbackChar();
                return 0;
            }

            var start = Remaining;
            var elements = 0;
            var moveNext = 1;
            while (moveNext-- > 0)
            {
                ++elements;
                SkipObj(handler);
                if (ReadBoolComma())
                    moveNext++;
            }

            ReadArrayRight();
            Rollback(start - Remaining);
            return elements;
            throw new JsonWrongCharacterException("Array character error");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool StrCompair(string str)
        {
            if (Remaining < str.Length)
                return false;
            //2,3,4,5,6,7,8,9
            fixed (char* p = str)
            {
                var o = p;
                switch (str.Length)
                {
                    case 2:
                        if (*(int*)Pointer != *(int*)o) return false;
                        goto True;
                    case 3:
                        if (*(int*)Pointer != *(int*)o) return false;
                        if (*(Pointer + 2) != *(o + 2)) return false;
                        goto True;
                    case 4:
                        if (*(long*)Pointer != *(long*)o) return false;
                        goto True;
                    case 5:
                        if (*(long*)Pointer != *(long*)o) return false;
                        if (*(Pointer + 4) != *(o + 4)) return false;
                        goto True;
                    case 6:
                        if (*(long*)Pointer != *(long*)o) return false;
                        if (*(int*)(Pointer + 4) != *(int*)(o + 4)) return false;
                        goto True;
                    case 7:
                        if (*(long*)Pointer != *(long*)o) return false;
                        if (*(int*)(Pointer + 4) != *(int*)(o + 4)) return false;
                        if (*(Pointer + 6) != *(o + 6)) return false;
                        goto True;
                    case 8:
                        if (*(long*)Pointer != *(long*)o) return false;
                        if (*(long*)(Pointer + 4) != *(long*)(o + 4)) return false;
                        goto True;
                    case 9:
                        if (*(long*)Pointer != *(long*)o) return false;
                        if (*(long*)(Pointer + 4) != *(long*)(o + 4)) return false;
                        if (*(Pointer + 8) != *(o + 8)) return false;
                        goto True;
                    default:
                        throw new Exception("Check the code,StrCompair no current length ");
                }
            }

        True:
            Remaining -= str.Length;
            Pointer += str.Length;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal string RemoveQuoteAndSubString(int idx, int length)
        {
            if (this[idx] == 'n')
                return null;
            return this.SubString(idx + 1, length - 2); //remove \"   \"
        }

        private static MethodInfo GetMethodInfo(string name)
        {
            return typeof(JsonReader).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}
