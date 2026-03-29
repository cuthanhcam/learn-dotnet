using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PART14Interface
{
    internal static class ReadableExts
    {
        public static void WriteName(this IReadable readable)
        {
            Console.WriteLine($"Readable Name: {readable.Name} extension method");
        }
    }
}
