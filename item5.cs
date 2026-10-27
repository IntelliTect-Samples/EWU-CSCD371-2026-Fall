int[] numbers = [5, 2, 4, 1, 3];

var result = numbers
    .Where(number => number % 2 == 0)
    .OrderBy(number => number)
    .Select(number => number * 10);

Console.WriteLine(string.Join(", ", result));
