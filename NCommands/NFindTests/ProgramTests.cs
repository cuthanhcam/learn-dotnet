using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Assert.IsTrue(Assert.IsInstanceOfType(options, typeof(FindOptions)));

            Assert.Fail();
        }
    }
}