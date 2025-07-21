using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFind
{
    public interface ILineSource
    {
        string Name { get; }
        Line? ReadLine();
        void Open();
        void Close();
    }
}
