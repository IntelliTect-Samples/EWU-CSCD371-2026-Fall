using System;
using System.Collections.Generic;
using System.Globalization;
using Math = System.Math;

namespace Calculate;

public class Calculator
{
    private const NumberStyles OperandStyle = NumberStyles.Integer;  //number style for parsing operands

    //Define static Add, Subtract, Multiply, and Divide methods
    public static int Add(int left, int right) => checked(left + right);
    public static int Subtract(int left, int right) => checked(left - right);
    public static int Multiply(int left, int right) => checked(left * right);
    public static int Divide(int left, int right) =>
        right == 0 ? throw new DivideByZeroException("Division by zero is undefined.") : checked((int)Math.Round((double)left / right));

    // Define a read-only property, MathematicalOperations, of type IReadOnlyDictionary<char, Func<int, int, int>>

    public IReadOnlyDictionary<char, Func<int, int, int>> MathematicalOperations { get; } =
        new Dictionary<char, Func<int, int, int>>
        {
            ['+'] = Add,
            ['-'] = Subtract,
            ['*'] = Multiply,
            ['/'] = Divide,
        };

    //Implement a TryCalculate method
    public bool TryCalculate(string? calculation, out int result)
    {
        result = 0;

        if (calculation?.Split(' ') is [var left, [var op], var right]
            && int.TryParse(left, OperandStyle, CultureInfo.InvariantCulture, out int leftOperand)
            && int.TryParse(right, OperandStyle, CultureInfo.InvariantCulture, out int rightOperand)
            && MathematicalOperations.TryGetValue(op, out Func<int, int, int>? operation))
        {
            try
            {
                // This safely catches overflows (from checked blocks) and division-by-zero
                result = operation(leftOperand, rightOperand);
                return true;
            }
            catch (Exception ex) when (ex is OverflowException or DivideByZeroException)
            {
                // Instead of crashing, we gracefully fail and return false
                return false;
            }
        }

        return false;
    }
}