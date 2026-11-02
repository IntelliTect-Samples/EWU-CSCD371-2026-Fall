namespace Calculate;

public class Calculator
{
    public static int Add(int a, int b)
    {
        return a + b;
    }

    public static int Subtract(int a, int b)
    {
        return a - b;
    }

    public static int Multiply(int a, int b)
    {
        return a * b;
    }

    public static int Divide(int a, int b)
    {
        return a / b;
    }
    public IReadOnlyDictionary<char, Func<int, int, int>> MathematicalOperations { get; }
    = new Dictionary<char, Func<int, int, int>>
    {
        { '+', Add },
        { '-', Subtract },
        { '*', Multiply },
        { '/', Divide }
    };

    public bool TryCalculate(string? calculation, out int result)
    {
        result = 0;

        if (calculation?.Split(' ') is [var left, var op, var right]
            && int.TryParse(left, out int a)
            && op.Length == 1
            && MathematicalOperations.TryGetValue(op[0], out var operation)
            && int.TryParse(right, out int b)
            && !(op[0] == '/' && b == 0))
        {
            result = operation(a, b);
            return true;
        }

        return false;
    }
}