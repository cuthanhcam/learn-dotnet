using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFind;

namespace NFind.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void BuildOptionsTest()
        {
            string[] args = ["/v", "/c", "/n"];

            var options = Program.BuildOptions(args);

            Assert.IsNotNull(options);
            Assert.IsTrue(options.FindDontConstain);
            Assert.IsTrue(options.CountMode);
            Assert.IsTrue(options.IsCaseSensitive);
        }
    }
}