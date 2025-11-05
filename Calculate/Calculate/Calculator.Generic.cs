using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Numerics;

namespace Calculate;

public sealed class Calculator<T> where T : INumber<T>
{

    public IReadOnlyDictionary<char, Func<T, T, T>> MathematicalOperations { get; }
    
    public Calculator()
    {
        MathematicalOperations = new Dictionary<char, Func<T, T, T>>
        {
            ['+'] = (a, b) => a + b,
            ['-'] = (a, b) => a - b,
            ['*'] = (a, b) => a * b,
            ['/'] = (a, b) => a / b
        };
     
    }

    public T Add(T a, T b) => a + b;
    public T Subtract(T a, T b) => a - b;
    public T Multiply(T a, T b) => a * b;
    public T Divide(T a, T b) => a / b;

    public bool TryCalculate(string? input, out T result)
    {
        result = T.Zero;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3) return false;

        if (!T.TryParse(parts[0], CultureInfo.InvariantCulture, out var left)) return false;
        if (!T.TryParse(parts[2], CultureInfo.InvariantCulture, out var right)) return false;

        if (parts[1].Length != 1) return false;

        var op = parts[1][0];
        
        if (!MathematicalOperations.TryGetValue(op, out var operation)) return false;
        if (op == '/' && right == T.Zero) return false;

        result = operation(left, right);
        return true;
    }
}
