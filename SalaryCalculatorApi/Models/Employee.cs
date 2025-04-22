using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryCalculatorApi.Models
{
    public class Employee
    {
        public string? Name { get; set; }
        public int WorkingHours { get; set; }
        public double SalaryCoefficient { get; set; }
        public string? Position { get; set; }
    }
}