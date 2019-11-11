using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Kooboo.Json.Test
{
    [TestClass]

    public class ExpressionTest
    {

        public static string GenerateSwitch(OffsetTree offsetTree)
        {
            StringBuilder sb = new StringBuilder();
            if (offsetTree != null)
            {
                sb.AppendLine("Switch(getChar())");
                if (offsetTree.C == '"')
                {
                    sb.AppendLine($"case '{offsetTree.C}'");
                    sb.AppendLine($"return '{offsetTree.C}'");
                }
                else
                {
                    sb.AppendLine($"case '{offsetTree.C}'");
                    sb.Append(LoopGenerateSwitch(offsetTree.Childrens));
                }
            }

            return sb.ToString();
        }

        public static string LoopGenerateSwitch(List<OffsetTree> childrens)
        {
            StringBuilder sb = new StringBuilder();
            if (childrens.Count > 0)
            {
                sb.AppendLine("Switch(getChar())");
                foreach (OffsetTree ot in childrens)
                {
                    if (ot.C == '"')
                    {
                        sb.AppendLine($"case '{ot.C}'");
                        sb.AppendLine($"return '{ot.C}'");
                    }
                    else
                    {
                        sb.AppendLine($"case '{ot.C}'");
                        sb.Append(LoopGenerateSwitch(ot.Childrens));
                    }
                }
            }

            return sb.ToString();
        }

        [TestMethod]
        public void Expression_should_be_correct()
        {
            var objOffsetTree = JsonSerializer.ToObject<OffsetTree>("{\"C\":\"\\u0000\",\"Childrens\":[{\"C\":\"N\",\"Childrens\":[{\"C\":\"a\",\"Childrens\":[{\"C\":\"s\",\"Childrens\":[{\"C\":\"\\u0000\",\"Childrens\":null,\"Offset\":4},{\"C\":\"1\",\"Childrens\":null,\"Offset\":4},{\"C\":\"f\",\"Childrens\":null,\"Offset\":4}],\"Offset\":3}],\"Offset\":2}],\"Offset\":1}],\"Offset\":0}");
            var str = GenerateSwitch(objOffsetTree);
            Assert.AreEqual("", str);
        }
    }

    public class OffsetTree
    {
        public List<OffsetTree> Childrens;
        public char C;
        public int Offset;
    }
}