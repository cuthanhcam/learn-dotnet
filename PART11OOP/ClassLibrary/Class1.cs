using PART11OOP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    internal class Class1
    {
        internal void P()
        {
            var printer = new Printer();
            printer.Print("Hello, World!");
        }

        private void P2()
        {
            P();    
        }
    }
}
