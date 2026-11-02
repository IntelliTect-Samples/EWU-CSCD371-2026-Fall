namespace Calculate;

public class Calculator
{
    public static int Add(int a, int b) => a + b;
    public static int Subtract(int a, int b) => a - b;
    public static int Multiply(int a, int b) => a * b;
    public static int Divide(int a, int b)
    {
        if (b == 0)
            throw new DivideByZeroException("Cannot divide by zero.");
        return a / b;
    }

    public IReadOnlyDictionary<char, Func<int, int, int>> MathematicalOperations { get; } =
        new Dictionary<char, Func<int, int, int>>
        {
            ['+'] = Add,
            ['-'] = Subtract,
            ['*'] = Multiply,
            ['/'] = Divide,
        };

    public bool TryCalculate(string? calculation, out int result)
    {
        
        if (calculation?.Split(' ') is [string left, [char @operator], string right]
            && int.TryParse(left, out int leftOperand)
            && int.TryParse(right, out int rightOperand)
            && MathematicalOperations.TryGetValue(@operator, out Func<int, int, int>? operation)
            && !(@operator == '/' && rightOperand == 0))
        {
            result = operation(leftOperand, rightOperand);
            return true;
        }

        result = 0;
        return false;
    }
}