using Microsoft.VisualStudio.TestTools.UnitTesting;
using PART17ADONET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PART17ADONET.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void TestDBConnectionTest()
        {
            Program.TestDBConnection();
        }
    }
}