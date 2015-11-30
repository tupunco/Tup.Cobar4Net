using NUnit.Framework;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    [TestFixture(Category = "SoloParserTest")]
    public class SoloParserTest
    {
        [Test]
        public virtual void TestMain()
        {
            SoloParser.Main(null);
            Assert.IsTrue(true);
        }
    }
}