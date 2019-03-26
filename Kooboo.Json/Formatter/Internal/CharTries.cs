using System.Collections.Generic;

namespace Kooboo.Json.Deserialize
{
    internal class CharTries
    {
        internal char Val;
        internal MemberExtension Member;
        internal bool IsPeak;
        internal bool IsValue;
        internal CharTries Parent;
        internal List<CharTries> Childrens;

        internal CharTries() {
            Childrens = new List<CharTries>();
            IsValue = false;
        }

        internal void Insert(string str,MemberExtension mem)
        {
            CharTries charTries = this;
            foreach (var c in str)
            {
                var @case = charTries.Childrens.Find(e => e.Val == c);
                if (@case == null)
                {
                    @case = new CharTries() { Val = c, Parent=charTries };
                    charTries.Childrens.Add(@case);
                }
                charTries = @case;
            }
            charTries.IsValue = true;
            charTries.Member = mem;
        }
    }
}
