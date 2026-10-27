List<int> numbers = [1, 2, 3];
var result = numbers.Where(number => number % 2 == 0).ToList();

numbers.Add(4);

Console.WriteLine(string.Join(", ", result));
