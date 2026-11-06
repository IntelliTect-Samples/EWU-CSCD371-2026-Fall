using System.Globalization;

namespace Calculate;

public class Calculator
{
    public static int Add(int left, int right) => left + right;

    public static int Subtract(int left, int right) => left - right;

    public static int Multiply(int left, int right) => left * right;

    public static int Divide(int left, int right) => left / right;

    public IReadOnlyDictionary<char, Func<int, int, int>> MathematicalOperations { get; } =
        new Dictionary<char, Func<int, int, int>>
        {
            ['+'] = Add,
            ['-'] = Subtract,
            ['*'] = Multiply,
            ['/'] = Divide
        };

    public bool TryCalculate(string? calculation, out int result)
    {
        result = 0;

        if (calculation?.Split(' ') is [var left, [var op], var right]
            && int.TryParse(left, CultureInfo.InvariantCulture, out int leftOperand)
            && int.TryParse(right, CultureInfo.InvariantCulture, out int rightOperand)
            && MathematicalOperations.TryGetValue(op, out Func<int, int, int>? operation)
            && !(op is '/' && rightOperand is 0))
        {
            result = operation(leftOperand, rightOperand);
            return true;
        }

        return false;
    }
}
