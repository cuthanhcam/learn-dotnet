using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PART14Interface
{
    internal class DatabaseReadable : IReadable
    {
        public string Name => "DatabaseReadable";

        public int ReadInt()
        {
            return 100;
        }

        public string ReadString()
        {
            return "Database String Example";
        }
    }
}
