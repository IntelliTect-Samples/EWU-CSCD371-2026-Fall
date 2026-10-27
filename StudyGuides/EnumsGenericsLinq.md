# .NET Fundamentals: Enums, Generics, and LINQ

This study guide covers three foundational concepts in C# and .NET development: Enums, Generics, and LINQ.

---

## 1. Enums (Enumerations)

An **enum** (short for enumeration) is a distinct value type that represents a group of named constants. Enums make code more readable, maintainable, and strongly typed compared to using magic numbers or plain strings.

### Key Concepts

- **Underlying Type:** By default, enums are backed by `int` values starting from `0`.
- **Custom Values:** You can explicitly assign integer values to enum members.
- **Type Safety:** Enums prevent passing invalid numbers or strings to methods expecting a predefined set of options.

### Code Example

```csharp
using System;

// 1. Defining an Enum
public enum OrderStatus
{
    Pending,    // 0
    Processing, // 1
    Shipped,    // 2
    Delivered,  // 3
    Canceled    // 4
}

// Custom underlying values
public enum Priority
{
    Low = 10,
    Medium = 20,
    High = 30
}

class Program
{
    static void Main()
    {
        // Usage
        OrderStatus currentStatus = OrderStatus.Processing;

        // Enum to int
        int statusValue = (int)currentStatus; // Output: 1

        // Int to Enum
        OrderStatus parsedStatus = (OrderStatus)2; // Output: Shipped

        // String to Enum
        Enum.TryParse("Delivered", out OrderStatus newStatus);

        Console.WriteLine($"Current Status: {currentStatus} (Value: {statusValue})");
    }
}
```

---

## 2. Generics

**Generics** allow you to write classes, interfaces, and methods with placeholders for types. This enables code reuse, type safety, and better performance (by eliminating boxing/unboxing overhead for value types).

### Key Concepts

- **Type Parameter `<T>`:** Represents an unknown type to be specified when instantiated or called.
- **Type Safety:** The compiler enforces that only the specified type can be stored or used.
- **Generic Constraints (`where` keyword):** Restricts the type arguments allowed (e.g., must be a class, struct, or implement a specific interface).

### Code Example

```csharp
using System;
using System.Collections.Generic;

// 1. Generic Class
public class DataResult<T>
{
    public T Data { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }

    public DataResult(T data)
    {
        Data = data;
        IsSuccess = true;
    }
}

// 2. Generic Method with a Constraint
public class Utility
{
    // Restricts T to types that implement IComparable<T>
    public static T GetMax<T>(T first, T second) where T : IComparable<T>
    {
        return first.CompareTo(second) > 0 ? first : second;
    }
}

class Program
{
    static void Main()
    {
        // Using Generic Class with int
        var intResult = new DataResult<int>(42);

        // Using Generic Class with string
        var stringResult = new DataResult<string>("Operation complete");

        // Using Generic Method
        int maxInt = Utility.GetMax(10, 25);         // Output: 25
        string maxStr = Utility.GetMax("Apple", "Banana"); // Output: Banana
    }
}
```

---

## 3. LINQ (Language Integrated Query)

**LINQ** provides a consistent, readable syntax for querying and transforming data collections (like arrays, lists, or database sets) directly in C#.

### Key Concepts

- **Query Syntax vs. Method Syntax:** Query syntax resembles SQL, while Method syntax uses extension methods with Lambda expressions (e.g., `x => x.Property`). Method syntax is most common.
- **Deferred Execution:** LINQ queries are generally not executed when created; they execute when the results are actually iterated over (e.g., via `foreach` or calling `.ToList()`).

### Common LINQ Operators

- `Where`: Filters elements based on a condition.
- `Select`: Projects/transforms each element into a new form.
- `OrderBy` / `OrderByDescending`: Sorts elements.
- `FirstOrDefault`: Returns the first matching element, or a default value if none found.
- `ToList` / `ToArray`: Executes the query and returns a concrete collection.

### Code Example

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class Student
{
    public string Name { get; set; }
    public int Score { get; set; }
}

class Program
{
    static void Main()
    {
        List<Student> students = new List<Student>
        {
            new Student { Name = "Alice", Score = 85 },
            new Student { Name = "Bob", Score = 60 },
            new Student { Name = "Charlie", Score = 92 },
            new Student { Name = "Diana", Score = 74 }
        };

        // 1. Filtering (Where) and Transforming (Select) - Method Syntax
        var highScorers = students
                            .Where(s => s.Score >= 80)
                            .OrderByDescending(s => s.Score)
                            .Select(s => s.Name);

        Console.WriteLine("High Scorers:");
        foreach (var name in highScorers)
        {
            Console.WriteLine(name); // Output: Charlie, Alice
        }

        // 2. Query Syntax Example
        var querySyntaxResult = from s in students
                                where s.Score < 80
                                select s.Name;

        // 3. Finding a Single Element
        Student topStudent = students.OrderByDescending(s => s.Score).FirstOrDefault();
        Console.WriteLine($"Top Student: {topStudent?.Name}"); // Output: Charlie
    }
}
```

---

## Summary Checklist

- [ ] **Enums:** Use when you have a fixed set of named constant options.
- [ ] **Generics:** Use when creating reusable components that work across multiple data types without losing type safety.
- [ ] **LINQ:** Use to filter, transform, sort, or query collections concisely using method or query syntax.