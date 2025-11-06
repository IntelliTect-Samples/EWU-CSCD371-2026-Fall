namespace Calculate;

public class Calculator
{
    public IReadOnlyDictionary<char, Func<int, int, int>> MathematicalOperations { get; }
            = new Dictionary<char, Func<int, int, int>>
            {
                ['+'] = Add,
                ['-'] = Subtract,
                ['*'] = Multiply,
                ['/'] = Divide
            };

    public static int Add(int a, int b) => a + b;

    public static int Subtract(int a, int b) => a - b;

    public static int Multiply(int a, int b) => a * b;

    public static int Divide(int a, int b) => a / b;

    public bool TryCalculate(string calculation, out int result)
    {
        result = 0;
        if (string.IsNullOrWhiteSpace(calculation))
            return false;

        string[] tokens = calculation.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        List<string> postfix = [];
        Stack<char> operators = new();
        Stack<int> evaluationStack = new();

        Dictionary<char, int> precedence = new()
        {
            ['+'] = 1,
            ['-'] = 1,
            ['*'] = 2,
            ['/'] = 2
        };

        foreach (string token in tokens)
        {
            if (int.TryParse(token, out _))
            {
                postfix.Add(token);
            }
            else if (token.Length == 1 && MathematicalOperations.ContainsKey(token[0]))
            {
                char op = token[0];
                while (operators.Count > 0 && precedence[operators.Peek()] >= precedence[op])
                    postfix.Add(operators.Pop().ToString());

                operators.Push(op);
            }
            else
            {
                // Invalid token, not a number or operator
                return false;
            }
        }

        while (operators.Count > 0)
            postfix.Add(operators.Pop().ToString());

        foreach (string token in postfix)
        {
            if (int.TryParse(token, out int number))
            {
                evaluationStack.Push(number);
            }
            else if (token.Length == 1 && MathematicalOperations.TryGetValue(token[0], out var op))
            {
                // if two operands are not available, invalid postfix syntax has occurred
                if (evaluationStack.Count < 2)
                    return false;

                int b = evaluationStack.Pop();
                int a = evaluationStack.Pop();

                // divide by 0 is not allowed
                if (token[0] == '/' && b == 0)
                    return false;

                evaluationStack.Push(op(a, b));
            }
            else
            {
                // invalid token in postfix expression
                return false;
            }
        }

        // invalid postfix expression
        if (evaluationStack.Count != 1)
            return false;

        result = evaluationStack.Pop();
        return true;
    }
}
