using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class IgnoreCommentFeatureTest
    {
        class I
        {
            public string Name;
            public int[] Children;
        }

        [TestMethod]
        public void IgnoreCommentFeature_should_be_work_correct()
        {
            string json = @"
                 /*注释*/
                {//注释  
                     /*注释*/""Name"" /*注释*/: /*注释*/""CMS"" /*注释*/,//注释
                    /*注释*/
                    ""Children"":[//注释
                                    1/*注释*/,
                                    2/*注释*/
                                ]//注释
                }//注释
                /*注释*/
                ";
            var obj = JsonSerializer.ToObject<I>(json);
            Assert.AreEqual("CMS", obj.Name);
            Assert.AreEqual(1, obj.Children[0]);
            Assert.AreEqual(2, obj.Children[1]);
        }
    }
}