namespace OopBasics.Examples.Polymorphism
{
    /// <summary>
    /// Demonstrates:
    /// - virtual / override
    /// - Runtime polymorphism
    /// - Dynamic dispatch
    /// </summary>
    public class VirtualOverrideExample
    {
        public static void Run()
        {
            Console.WriteLine("VirtualOverrideExample: Runtime polymorphism");

            Shape[] shapes =
            {
                new Circle(5),
                new Rectangle(4, 6)
            };

            foreach (var shape in shapes)
            {
                Console.WriteLine($"{shape.GetType().Name} Area: {shape.GetArea()}");
            }

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- 'virtual' allows overriding behavior.");
            Console.WriteLine("- 'override' provides specific implementation.");
            Console.WriteLine("- Method call is resolved at runtime.");
        }
    }

    public abstract class Shape
    {
        public abstract double GetArea();
    }

    public class Circle : Shape
    {
        private double Radius { get; }

        public Circle(double radius)
        {
            Radius = radius;
        }

        public override double GetArea()
        {
            return Math.PI * Radius * Radius;
        }
    }

    public class Rectangle : Shape
    {
        private double Width { get; }
        private double Height { get; }

        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public override double GetArea()
        {
            return Width * Height;
        }
    }
}
