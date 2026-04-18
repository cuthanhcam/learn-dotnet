namespace CSharpBasics.Exercises.Hard;

public static class BasicCalculator
{
    public static double Evaluate(double left, double right, char operation)
    {
        return operation switch
        {
            '+' => left + right,
            '-' => left - right,
            '*' => left * right,
            '/' when right != 0 => left / right,
            '/' => throw new DivideByZeroException("Cannot divide by zero."),
            _ => throw new ArgumentException("Unsupported operation.", nameof(operation))
        };
    }
}
