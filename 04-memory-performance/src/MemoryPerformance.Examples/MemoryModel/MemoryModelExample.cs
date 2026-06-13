namespace MemoryPerformance.Examples.MemoryModel;

/// <summary>
/// Demonstrates value semantics, reference semantics, stack frames, and object identity.
/// </summary>
public static class MemoryModelExample
{
    public static void Run()
    {
        Console.WriteLine("Stack calculation result: " + StackCalculation(10, 32));
        Console.WriteLine("Value copy result: " + ValueTypeCopyExample());
        Console.WriteLine("Reference alias result: " + ReferenceAliasExample());
        Console.WriteLine("Array mutation result: " + MutateArrayThroughReference());
        Console.WriteLine("Readonly input result: " + DistanceFromOrigin(new Point(3, 4)));
    }

    public static int StackCalculation(int left, int right)
    {
        int result = left + right;
        return result;
    }

    public static Customer CreateCustomer(string name, int loyaltyPoints)
    {
        return new Customer(name, loyaltyPoints);
    }

    public static string ValueTypeCopyExample()
    {
        var original = new Point(2, 4);
        Point copy = original with { X = 99 };

        return $"original={original}; copy={copy}";
    }

    public static string ReferenceAliasExample()
    {
        var first = new Customer("Mina", 10);
        Customer second = first;

        second.Name = "Updated";
        second.LoyaltyPoints += 5;

        return $"first={first}; second={second}; same={ReferenceEquals(first, second)}";
    }

    public static int MutateArrayThroughReference()
    {
        int[] values = [1, 2, 3];
        int[] alias = values;

        alias[0] = 100;

        return values[0];
    }

    public static double DistanceFromOrigin(in Point point)
    {
        return Math.Sqrt((point.X * point.X) + (point.Y * point.Y));
    }
}

public readonly record struct Point(int X, int Y);

public sealed class Customer
{
    public Customer(string name, int loyaltyPoints)
    {
        Name = name;
        LoyaltyPoints = loyaltyPoints;
    }

    public string Name { get; set; }
    public int LoyaltyPoints { get; set; }

    public override string ToString()
    {
        return $"{Name}:{LoyaltyPoints}";
    }
}
