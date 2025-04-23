using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DI_Test.Interfaces;

namespace DI_Test.Services
{
    public class MyTransientService : IMyTransientService
    {
        private static int Id = 0;
        public MyTransientService()
        {
            Console.WriteLine($"Transient!: {++Id}");
        }
    }
}