namespace Contacts;

public static class Program
{
    public static void Main()
    {
        ContactBook book = new();

        while (true)
        {
            Console.WriteLine("\n=== Contacts Menu ===");
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. Add Employee");
            Console.WriteLine("3. List Contacts");
            Console.WriteLine("4. Remove Contact");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddStudent(book);
                    break;
                case "2":
                    AddEmployee(book);
                    break;
                case "3":
                    ListContacts(book);
                    break;
                case "4":
                    RemoveContact(book);
                    break;
                case "5":
                    return;
                case null:
                    return;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    private static string? ReadRequired(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("This field is required.");
            return null;
        }

        return input;
    }

    private static FullName? ReadName()
    {
        string? first = ReadRequired("First name: ");
        if (first is null) return null;

        string? last = ReadRequired("Last name: ");
        if (last is null) return null;

        Console.Write("Middle name (optional): ");
        string? middle = Console.ReadLine()?.Trim();

        return new FullName(first, last, middle);
    }

    private static void AddStudent(ContactBook book)
    {
        FullName? name = ReadName();
        if (name is null) return;

        string? email = ReadRequired("Email: ");
        if (email is null) return;

        string? major = ReadRequired("Major: ");
        if (major is null) return;

        Student student = new(name, email, major);

        if (book.Add(student))
        {
            Console.WriteLine("Student added successfully.");
        }
        else
        {
            Console.WriteLine("Duplicate student. Not added.");
        }
    }

    private static void AddEmployee(ContactBook book)
    {
        FullName? name = ReadName();
        if (name is null) return;

        string? email = ReadRequired("Email: ");
        if (email is null) return;

        string? department = ReadRequired("Department: ");
        if (department is null) return;

        Employee employee = new(name, email, department);

        if (book.Add(employee))
        {
            Console.WriteLine("Employee added successfully.");
        }
        else
        {
            Console.WriteLine("Duplicate employee. Not added.");
        }
    }

    private static void ListContacts(ContactBook book)
    {
        IContact[] contacts = book.GetAll();

        if (contacts.Length == 0)
        {
            Console.WriteLine("No contacts found.");
            return;
        }

        for (int i = 0; i < contacts.Length; i++)
        {
            IContact contact = contacts[i];

            string details = contact switch
            {
                Student student => $"Student - {student.Name}, {student.Email}, Major: {student.Major}",
                Employee employee => $"Employee - {employee.Name}, {employee.Email}, Department: {employee.Department}",
                _ => $"{contact.Name}, {contact.Email}"
            };

            Console.WriteLine($"{i + 1}. {details}");
        }
    }

    private static void RemoveContact(ContactBook book)
    {
        IContact[] contacts = book.GetAll();

        if (contacts.Length == 0)
        {
            Console.WriteLine("No contacts to remove.");
            return;
        }

        ListContacts(book);
        Console.Write("Enter contact number to remove: ");

        if (!int.TryParse(Console.ReadLine(), out int number)
            || number < 1
            || number > contacts.Length)
        {
            Console.WriteLine("Invalid contact number.");
            return;
        }

        if (book.Remove(contacts[number - 1]))
        {
            Console.WriteLine("Contact removed successfully.");
        }
        else
        {
            Console.WriteLine("Contact not found.");
        }
    }
}