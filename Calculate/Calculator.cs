namespace Calculate;

public class Calculator
{
    public static int Add(int a, int b) => a + b;
    public static int Subtract(int a, int b) => a - b;
    public static int Multiply(int a, int b) => a * b;
    public static int Divide(int a, int b) => a / b;

    public IReadOnlyDictionary<char, Func<int, int, int>> MathematicalOperations { get; } =
        new Dictionary<char, Func<int, int, int>>
        {
            ['+'] = Add,
            ['-'] = Subtract,
            ['*'] = Multiply,
            ['/'] = Divide
        };

    public bool TryCalculate(string calculation, out int result)
    {

        result = 0;
        var parts = calculation.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3 ||
            parts[1].Length != 1 ||
            !int.TryParse(parts[0], out int left) ||
            !int.TryParse(parts[2], out int right) ||
            !MathematicalOperations.TryGetValue(parts[1][0], out var operation))
        {
            return false;
        }

        // prevent division by zero
        if (parts[1][0] == '/' && right == 0)
        {
            return false;
        }

        result = operation(left, right);
        return true;
    }
}
