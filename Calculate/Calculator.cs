using System.Numerics;

namespace Calculate;

public class Calculator
{
    public IReadOnlyDictionary<char, Func<int, int, int>> MathematicalOperations { get; } =
        new Dictionary<char, Func<int, int, int>>
        {
            ['+'] = Add<int>,
            ['-'] = Subtract<int>,
            ['*'] = Multiply<int>,
            ['/'] = Divide<int>,
        };
    
    public static T Add<T>(T left, T right)
        where T : INumber<T>
    {
        return left + right;
    }

    public static T Subtract<T>(T left, T right)
        where T : INumber<T>
    {
        return left - right;
    }

    public static T Multiply<T>(T left, T right)
        where T : INumber<T>
    {
        return left * right;
    }

    public static T Divide<T>(T left, T right)
        where T : INumber<T>
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