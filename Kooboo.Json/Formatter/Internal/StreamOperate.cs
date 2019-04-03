using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace Kooboo.Json
{
    internal static class StreamOperate
    {
        internal static Func<StreamReader, char[]> GetStreamReaderCharBuffer = BuildStreamReaderCharBufferFunc();
        internal static Func<StreamReader, int> GetStreamReaderCharLen = BuildStreamReaderCharLenFunc();


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
            var len = Expression.MakeMemberAccess(instance, info);
            return Expression.Lambda<Func<StreamReader, int>>(len, new[] { instance }).Compile();
        }


        internal static Func<StreamWriter, char[]> GetStreamWriterCharBuffer = BuildStreamWriterCharBufferFunc();
        internal static Func<StreamWriter, int> GetStreamWriterCharLen = BuildStreamWriterCharPosFunc();
        internal static Action<StreamWriter, int> SetStreamWriterCharLen = BuildSetStreamWriterCharPosFunc();


        static Func<StreamWriter, char[]> BuildStreamWriterCharBufferFunc()
        {
            var instance = Expression.Parameter(typeof(StreamWriter));
            var info = typeof(StreamWriter).GetField("_charBuffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) ?? typeof(StreamWriter).GetField("charBuffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var buf = Expression.MakeMemberAccess(instance, info);
            return Expression.Lambda<Func<StreamWriter, char[]>>(buf, new[] { instance }).Compile();
        }
        static Func<StreamWriter, int> BuildStreamWriterCharPosFunc()
        {
            var instance = Expression.Parameter(typeof(StreamWriter));
            var info = typeof(StreamWriter).GetField("_charPos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) ?? typeof(StreamWriter).GetField("charPos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var pos = Expression.MakeMemberAccess(instance, info);
            return Expression.Lambda<Func<StreamWriter, int>>(pos, new[] { instance }).Compile();
        }
        static Action<StreamWriter, int> BuildSetStreamWriterCharPosFunc()
        {
            var instance = Expression.Parameter(typeof(StreamWriter));
            var num = Expression.Parameter(typeof(int));
            var info = typeof(StreamWriter).GetField("_charPos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) ?? typeof(StreamWriter).GetField("charPos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var pos = Expression.MakeMemberAccess(instance, info);
            var assign = Expression.Assign(pos, num);
            return Expression.Lambda<Action<StreamWriter, int>>(assign, new[] { instance, num }).Compile();
        }


        internal unsafe static void Remove(this StreamWriter streamWriter, int start, int length)
        {
            int pos = GetStreamWriterCharLen(streamWriter);
            SetStreamWriterCharLen(streamWriter, pos - length);
            if (start + length == pos)
                return;
            char[] buf = GetStreamWriterCharBuffer(streamWriter);
            Array.Copy(buf, start + length, buf, start, pos - (start + length));
        }
    }
}
