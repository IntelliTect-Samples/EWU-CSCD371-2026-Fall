using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;


namespace Calculate;

public class Calculator <T> where T : INumber<T> {
    
    public IReadOnlyDictionary<char, Func<T, T, T>> MathematicalOperations { get; } =
        new Dictionary<char, Func<T, T, T>>
        {
            ['+'] = CalculatorOperations.Add,
            ['-'] = CalculatorOperations.Subtract,
            ['*'] = CalculatorOperations.Multiple,
            ['/'] = CalculatorOperations.Divide 
        };
        
    public bool TryCalculate(string calculation, out T result)
    {
        result = T.Zero;

        if (string.IsNullOrWhiteSpace(calculation))
            return false;

        var parts = calculation.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3)
            return false;

        var leftText = parts[0];
        var opText = parts[1];
        var rightText = parts[2];

        if (opText.Length != 1)
            return false;

        var op = opText[0];

        if (!T.TryParse(leftText, CultureInfo.InvariantCulture, out var left))
            return false;
        if (!T.TryParse(rightText, CultureInfo.InvariantCulture, out var right))
            return false;
        if (!MathematicalOperations.TryGetValue(op, out var operation))
            return false;

        try
        {
            result = operation(left, right);
            return true;
        }
        catch (DivideByZeroException)
        {
            return false;
        }
    }
}