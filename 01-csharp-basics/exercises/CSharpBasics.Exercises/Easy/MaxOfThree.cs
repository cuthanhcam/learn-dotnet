namespace CSharpBasics.Exercises.Easy;

public static class MaxOfThree
{
    public static int GetMax(int a, int b, int c)
    {
        int max = a;

        if (b > max)
        {
            max = b;
        }

        if (c > max)
        {
            max = c;
        }

        return max;
    }
}
