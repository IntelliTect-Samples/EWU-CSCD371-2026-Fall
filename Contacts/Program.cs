namespace Contacts;

public class Program
{
    public static void Main()
    {
        var book = new ContactBook();
        var nextId = 1;


        while (true)
        {
            Console.WriteLine("\n=== Contacts Book CLI ===");
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. Add Employee");
            Console.WriteLine("3. List Contacts");
            Console.WriteLine("4. Remove Contact");
            Console.WriteLine("5. Exit");
            Console.Write("Choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Add a Student");
                    AddStudent(book, nextId++);
                    break;

                case "2":
                    Console.WriteLine("Add an Employee");
                    AddEmployee(book, nextId++);
                    break;

                case "3":
                    List(book);
                    break;

                case "4":
                    Console.WriteLine("Remove a Contact");
                    Remove(book);
                    break;

                case "5":
                    Console.WriteLine("Goodbye");
                    return;

                default:
                    Console.WriteLine("Invalid choice. Try again");
                    break;
            }
        }
    }

    private static void AddStudent(ContactBook book, int id)
    {
        var name = ReadName();

        Console.Write("Major: ");
        var major = Console.ReadLine() ?? "";

        var student = new Student(id, name, major);

        if (!book.Add(student))
            Console.WriteLine("Duplicate student — not added.");
        else
            Console.WriteLine("Student added.");
    }

    private static void AddEmployee(ContactBook book, int id)
    {
        var name = ReadName();

        Console.Write("Department: ");
        var dept = Console.ReadLine() ?? "";

        var employee = new Employee(id, name, dept);

        if (!book.Add(employee))
            Console.WriteLine("Duplicate employee — not added.");
        else
            Console.WriteLine("Employee added.");
    }

    private static void List(ContactBook book)
    {
        var all = book.GetAll();

        if (book.IsEmpty) //(!all.Any())
        {
            Console.WriteLine("No contacts stored.");
            return;
        }

        Console.WriteLine("\n--- Contacts ---");

        foreach (var contact in all)
        {
            var details = contact switch
            {
                Student student => $"Student — Major: {student.Major}",
                Employee employee => $"Employee — Department: {employee.Department}",
                _ => contact.ContactInfo
            };

            Console.WriteLine($"{contact.Id}: {details} — {contact.Name}");
        }
    }

    private static void Remove(ContactBook book)
    {
        if (book.IsEmpty)
        {
            Console.WriteLine("No contacts stored.");
            return;
        }
        Console.Write("ID to remove: ");

        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        if (book.RemoveById(id))
            Console.WriteLine("Contact removed.");
        else
            Console.WriteLine("Contact not found.");
    }

    private static FullName ReadName()
    {
        Console.Write("First Name: ");
        var first = Console.ReadLine() ?? "";

        Console.Write("Middle Name (optional): ");
        var middle = Console.ReadLine();

        Console.Write("Last Name: ");
        var last = Console.ReadLine() ?? "";

        return new FullName(first, last, string.IsNullOrWhiteSpace(middle) ? null : middle);
    }
}