using System;
using System.Runtime.InteropServices;

namespace Kooboo.Json
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct GuidStruct
    {
        [FieldOffset(0)]
        internal Guid Value;

        [FieldOffset(0)]
        public  byte B00;
        [FieldOffset(1)]
        public  byte B01;
        [FieldOffset(2)]
        public  byte B02;
        [FieldOffset(3)]
        public  byte B03;
        [FieldOffset(4)]
        public  byte B04;
        [FieldOffset(5)]
        public  byte B05;

        [FieldOffset(6)]
        public  byte B06;
        [FieldOffset(7)]
        public  byte B07;
        [FieldOffset(8)]
        public  byte B08;
        [FieldOffset(9)]
        public  byte B09;

        [FieldOffset(10)]
        public  byte B10;
        [FieldOffset(11)]
        public  byte B11;

        [FieldOffset(12)]
        public  byte B12;
        [FieldOffset(13)]
        public  byte B13;
        [FieldOffset(14)]
        public  byte B14;
        [FieldOffset(15)]
        public  byte B15;

        public GuidStruct(Guid invisibleMembers)
            : this()
        {
            Value = invisibleMembers;
        }
    }
}
