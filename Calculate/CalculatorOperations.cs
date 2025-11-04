using System;
using System.Numerics;

namespace Calculate;

public static class CalculatorOperations
{
    public static T Add<T>(T a, T b) where T : INumber<T> => a + b;
    public static T Subtract<T>(T a, T b) where T : INumber<T> => a - b;
    public static T Multiple<T>(T a, T b) where T : INumber<T> => a * b;
    public static T Divide<T>(T a, T b) where T : INumber<T>
    {
        if (b == T.Zero) throw new DivideByZeroException();
        return a / b;
    }

}