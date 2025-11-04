using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate;

public class Calculator
{
    public static readonly IReadOnlyDictionary<char, Func<int,int,int>> MathematicalOperations = new Dictionary<char, Func<int, int, int>>()
    {{ '+', Add }, { '-', Subtract }, { '*', Multiply }, { '/', Divide }};

    private static readonly CompositeFormat AcceptableFormat = CompositeFormat.Parse(" {0} ");

    public static int Add(int a, int b) => checked(a + b);

    public static int Subtract(int a, int b) => checked(a - b);

    public static int Multiply(int a, int b) => checked(a * b);

    public static int Divide(int a, int b)
    {
        if (b == 0) throw new DivideByZeroException($"Param {nameof(b)} was 0!");

        return checked(a / b);
    }

    private static bool TryGetOperation(string input, out char operation)
    {
        operation = '_';
        if (string.IsNullOrEmpty(input)) return false;

        foreach (char iOperation in MathematicalOperations.Keys)
        {
            
            if (input.Contains(string.Format(CultureInfo.InvariantCulture, AcceptableFormat, iOperation)))
            {
                operation = iOperation;
                return true;
            }
        }
        return false;
    }

    public static bool TryCalculate(string input, out int value)
    {
        value = 0;

        if (string.IsNullOrEmpty(input)) return false;

        if (!TryGetOperation(input, out char operation)) return false;

        string[] parts = input.Split(operation);

        if (parts.Length != 2) return false;

        if (!int.TryParse(parts[0].Trim(), out int lhs)) return false;

        if(!int.TryParse(parts[1].Trim(), out int rhs)) return false;

        try
        {
            value = MathematicalOperations[operation](lhs, rhs);
            return true;
        }
        catch (Exception ex)
        {
            if(ex is DivideByZeroException || ex is OverflowException)
                return false;
            throw;
        }
    }

}
