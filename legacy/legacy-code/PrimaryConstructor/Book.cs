using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PrimaryConstructor
{
    public class Book(string title, string author, int year)
    {
        public string Title { get; } = title;
        public string Author { get; } = author;
        public int Year { get; } = year;
        public bool IsAvailable { get; set; } = true;

        public void PrintInfo()
        {
            Console.WriteLine($"Title: {Title}, Author: {Author}, Year: {Year}, {(IsAvailable ? "Available" : "Not Available")}");
        }
    }
}