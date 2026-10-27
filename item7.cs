int[] numbers = [1, 2, 3, 4];
int result = numbers.SingleOrDefault(number => number % 2 == 0);

Console.WriteLine(result);
