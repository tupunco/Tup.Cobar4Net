using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    [NUnit.Framework.TestFixture(Category = "SoloParserTest")]
    public class SoloParserTest
    {
        [NUnit.Framework.Test]
        public virtual void TestMain()
        {
            SoloParser.Main(null);
            NUnit.Framework.Assert.IsTrue(true);
        }
    }
}
