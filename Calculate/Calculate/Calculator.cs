using System;
using System.Collections.Generic;

namespace Calculate;

public class Calculator
{
    private readonly Calculator<int> _impl = new();

    public IReadOnlyDictionary<char, Func<int, int, int>> MathematicalOperations 
        => _impl.MathematicalOperations;

    public static int Add(int a, int b) => a + b;

    public static int Subtract(int a, int b) => a - b;

    public static int Multiply(int a, int b) => a * b;

    public static int Divide(int a, int b) => a / b;


    public bool TryCalculate(string? expression, out int result)
        => _impl.TryCalculate(expression, out result);
}
