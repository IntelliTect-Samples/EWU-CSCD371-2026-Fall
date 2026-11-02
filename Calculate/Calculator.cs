namespace Calculate;

public class Calculator
{
    public IReadOnlyDictionary<char, Func<int, int, decimal>> MathematicalOperations { get; } =
        new Dictionary<char, Func<int, int, decimal>>
        {
            ['+'] = Add,
            ['-'] = Subtract,
            ['*'] = Multiply,
            ['/'] = Divide
        };

    public static decimal Add(int left, int right) => (decimal)left + right;

    public static decimal Subtract(int left, int right) => (decimal)left - right;

    public static decimal Multiply(int left, int right) => (decimal)left * right;

    public static decimal Divide(int left, int right) => (decimal)left / right;

    public bool TryCalculate(string? calculation, out decimal result)
    {
        result = 0;

        if (calculation?.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
            is [var leftText, [var operation], var rightText]
            && operation is '+' or '-' or '*' or '/'
            && int.TryParse(leftText, out int left)
            && int.TryParse(rightText, out int right)
            && (operation != '/' || right != 0))
        {
            result = MathematicalOperations[operation](left, right);
            return true;
        }

        return false;
    }
}
