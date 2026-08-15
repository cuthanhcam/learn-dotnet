namespace OopBasics.Exercises;

public static class PolymorphismExercises
{
    public abstract class Shape
    {
        public abstract double GetArea();
    }

    public class Circle : Shape
    {
        public double Radius { get; }
        public Circle(double radius)
        {
            if (radius < 0)
                throw new ArgumentOutOfRangeException(nameof(radius), "Radius cannot be negative.");
            Radius = radius;
        }
        public override double GetArea() => Math.PI * Radius * Radius;
    }

    public class Rectangle : Shape
    {
        public double Width { get; }
        public double Height { get; }
        public Rectangle(double width, double height)
        {
            if (width < 0 || height < 0)
                throw new ArgumentOutOfRangeException("Width and Height cannot be negative.");
            Width = width;
            Height = height;
        }
        public override double GetArea() => Width * Height;
    }

    public static double SumAreas(IEnumerable<Shape> shapes)
    {
        ArgumentNullException.ThrowIfNull(shapes);

        double sum = 0;
        foreach (Shape shape in shapes)
        {
            // Calling through the abstract base type demonstrates runtime dispatch.
            sum += shape.GetArea();
        }

        return sum;
    }
}
