namespace Calculate;

public class Calculator
{
    public IReadOnlyDictionary<char, Func<int, int, int>> MathematicalOperations { get; } =
        new Dictionary<char, Func<int, int, int>>
        {
            ['+'] = Add,
            ['-'] = Subtract,
            ['*'] = Multiply,
            ['/'] = Divide,
        };
    
    public static int Add(int left, int right)
    {
        return left + right;
    }

    public static int Subtract(int left, int right)
    {
        return left - right;
    }

    public static int Multiply(int left, int right)
    {
        return left * right;
    }

    public static int Divide(int left, int right)
    {
        return left / right;
    }

    public bool TryCalculate(string? calculation, out int result)
    {
        string[]? parts = calculation?.Split(' ');

        if (parts is [var leftText, var operatorText, var rightText]
            && int.TryParse(leftText, out int left)
            && int.TryParse(rightText, out int right)
            && operatorText.Length == 1
            && MathematicalOperations.TryGetValue(operatorText[0], out Func<int, int, int>? mathematicalOperation))
        {
            try
            {
                result = mathematicalOperation(left, right);
                return true;
            }
            catch (DivideByZeroException)
            {
                result = 0;
                return false;
            }
        }
        
        result = 0;
        return false;
    }
}