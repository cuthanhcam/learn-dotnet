using System;

namespace Practice
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
    }
    public class Program 
    {
        public static void Main(string[] args)
        {
            Point p1 = new Point { X = 1, Y = 2 };
            Point p2 = p1;
            p2.X = 10;
            Console.WriteLine($"p1.X: {p1.X}, p2.X: {p2.X}"); // Output: p1.X: 1, p2.X: 10

            Person person1 = new Person { Name = "Alice" };
            Person person2 = person1;
            person2.Name = "Bob";
            Console.WriteLine($"person1.Name: {person1.Name}, person2.Name: {person2.Name}"); // Output: person1.Name: Bob, person2.Name: Bob

            string s1 = "Hello";
            string s2 = s1;
            s2 = "World";
            Console.WriteLine($"s1: {s1}, s2: {s2}"); // Output: s1: Hello, s2: World

        }
    }
}