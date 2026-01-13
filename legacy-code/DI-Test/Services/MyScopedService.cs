using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DI_Test.Interfaces;

namespace DI_Test.Services
{
    public class MyScopedService : IMyScopedService
    {
        private static int Id = 0;
        public MyScopedService()
        {
            Console.WriteLine($"Scoped!: {++Id}");
        }
    }
}