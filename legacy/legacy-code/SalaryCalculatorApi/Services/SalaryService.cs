using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalaryCalculatorApi.Models;

namespace SalaryCalculatorApi.Services
{
    public class SalaryService
    {
        public double CalculateSalary(Employee emp)
        {
            double baseSalary;
            double regularHourRate = emp.SalaryCoefficient * 100_000;

            if (emp.WorkingHours > 160)
            {
                double overtimeHours = emp.WorkingHours - 160;
                double regularPay = 160 * regularHourRate;
                double overtimePay = overtimeHours * regularHourRate * 1.5;
                baseSalary = regularPay + overtimePay;
            }
            else
            {
                baseSalary = emp.WorkingHours * regularHourRate;
            }

            if (emp.Position == "Manager")
            {
                baseSalary *= 1.1;
            }

            double tax = 0;
            
            if (baseSalary > 20_000_000)
            {
                tax = baseSalary * 0.2;
            }
            else if (baseSalary > 10_000_000)
            {
                tax = baseSalary * 0.1;
            }

            return baseSalary - tax;
        }
    }
}