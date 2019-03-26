using System;
using System.Runtime.CompilerServices;

namespace Kooboo.Json.Deserialize
{
    internal  class BaseTypeResolve: DefaultJsonResolve 
    {
        [FuncLable(FuncType.BaseType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Exception ReadException(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            if (reader.ReadNullOrObjLeft())
                return null;
            if (reader.ReadBoolObjRight())
                return new Exception();

            string source = null, message = null, helpLink = null;

            int i = 1;
            char c = ' ';
            while (i-- > 0)
            {
                reader.ReadQuotes();
                switch (handler.Option.JsonCharacterReadState)
                {
                    case JsonCharacterReadStateEnum.None:
                        c = reader.GetChar();
                        break;
                    case JsonCharacterReadStateEnum.InitialUpper:
                        c = char.ToUpper(reader.GetChar());
                        break;
                    case JsonCharacterReadStateEnum.InitialLower:
                        c = char.ToLower(reader.GetChar());
                        break;
                }
                switch (c)
                {
                    case 'S':
                        c = reader.GetChar();
                        if (c == 'o' && reader.StrCompair("urce\""))
                        {
                            reader.ReadColon();
                            source = PrimitiveResolve.ReadEscapeString(ref reader, handler);
                        }
                        else if (c == 't' && reader.StrCompair("ackTrace\""))
                        {
                            //只读
                            reader.ReadColon();
                            PrimitiveResolve.ReadEscapeString(ref reader, handler);
                        }
                        else
                            throw new Exception();
                        break;
                    case 'M':
                        {
                            if (reader.StrCompair("essage\""))
                            {
                                reader.ReadColon();
                                message = PrimitiveResolve.ReadEscapeString(ref reader, handler);
                            }
                            else
                                throw new Exception();
                            break;
                        }
                    case 'H':
                        {
                            if ( reader.StrCompair("elpLink\""))
                            {
                                reader.ReadColon();
                                helpLink = PrimitiveResolve.ReadEscapeString(ref reader, handler);
                            }
                            else
                                throw new Exception();
                            break;
                        }
                    default:
                        throw new Exception();
                }
                if (reader.ReadBoolComma())
                    i++;
            }

            var exception = message != null ? new Exception(message) : new Exception();

            exception.Source = source;
            exception.HelpLink = helpLink;

            return exception;
        }

        [FuncLable(FuncType.BaseType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Type ReadType(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            var typeName = PrimitiveResolve.ReadEscapeString(ref reader, handler);
            return typeName != null ? Type.GetType(typeName) : null;
        }
    }
}
