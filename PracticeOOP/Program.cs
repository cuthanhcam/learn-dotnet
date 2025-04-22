using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeOOP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int x; 
            Animal dog = new Dog("Dog", "male");
            dog.Speak();
        }
    }
}
