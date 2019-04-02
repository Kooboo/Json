using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace Kooboo.Json.Deserialize
{
    internal class StreamOperate
    {
        internal static Func<StreamReader, char[]> StreamReader_CharBuffer = BuildStreamReaderCharBufferFunc();
        internal static Func<StreamReader, int> StreamReader_CharLen = BuildStreamReaderCharLenFunc();


        static Func<StreamReader, char[]> BuildStreamReaderCharBufferFunc()
        {
            var instance = Expression.Parameter(typeof(StreamReader));
            var info = typeof(StreamReader).GetField("_charBuffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) ?? typeof(StreamReader).GetField("charBuffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var buf = Expression.MakeMemberAccess(instance, info);
            return Expression.Lambda<Func<StreamReader, char[]>>(buf, new[] { instance }).Compile();
        }
        static Func<StreamReader, int> BuildStreamReaderCharLenFunc()
        {
            var instance = Expression.Parameter(typeof(StreamReader));
            var info = typeof(StreamReader).GetField("_charLen", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) ?? typeof(StreamReader).GetField("charLen", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var buf = Expression.MakeMemberAccess(instance, info);
            return Expression.Lambda<Func<StreamReader, int>>(buf, new[] { instance }).Compile();
        }
    }
}
