namespace Contacts;

public static class Program
{
    public static void Main()
    {
        ContactBook book = new();
        bool running = true;

        while(running)
        {
            Console.WriteLine("\n====== Contact Book ======");
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. Add Employee");
            Console.WriteLine("3. List Contacts");
            Console.WriteLine("4. Remove Contact");
            Console.WriteLine("5. Exit");

            Console.Write("Select an option: ");
            string? input = Console.ReadLine();

            switch (input)
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
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }   
        }
    }

    private static void AddStudent(ContactBook book)
    {
        Console.Write("Enter first name: ");
        string firstName = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter middle name(optional): ");
        string? middleName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(middleName))
            middleName = null;  

        Console.Write("Enter last name: ");
        string lastName = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter email: ");
        string email = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter major (optional): ");
        string? major = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(major))
            major = null;

        try
        {
            FullName name = new(firstName, middleName, lastName);
            Student student = new(name, email, major);
            if (book.Add(student))
                Console.WriteLine("Student added successfully.");
            else
                Console.WriteLine("Student already exists in the contact book.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error adding student: {ex.Message}");
        }
    }

    private static void AddEmployee(ContactBook book)
    {
        Console.Write("Enter first name: ");
        string firstName = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter middle name (optional): ");
        string? middleName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(middleName))
            middleName = null;

        Console.Write("Enter last name: ");
        string lastName = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter email: ");
        string email = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter job title (optional): ");
        string? jobTitle = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(jobTitle))
        {
            jobTitle = null;
        }

        try
        {
            FullName name = new(firstName, middleName, lastName);
            Employee employee = new(name, email, jobTitle);
            if (book.Add(employee))
                Console.WriteLine("Employee added successfully.");
            else
                Console.WriteLine("Employee already exists in the contact book.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error adding employee: {ex.Message}");
        }
    }

    private static void ListContacts(ContactBook book)
    {
        Console.WriteLine("\n--- Contact List ---");
        foreach (IContact contact in book.GetAll())
        {
            Console.WriteLine($"{contact.GetType().Name}: {contact.Name} <{contact.Email}>");
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

        for (int i = 0; i < contacts.Length; i++)
        {
        Console.WriteLine(
            $"{i + 1}. {contacts[i].GetType().Name}: " +
            $"{contacts[i].Name} <{contacts[i].Email}>");
        }

        Console.Write("Enter contact number to remove: ");

        if (!int.TryParse(Console.ReadLine(), out int choice)
            || choice < 1
            || choice > contacts.Length)
        {
        Console.WriteLine("Invalid contact number.");
        return;
        }

        if (book.Remove(contacts[choice - 1]))
        {
            Console.WriteLine("Contact removed successfully.");
        }
        else
        {
            Console.WriteLine("Contact not found.");
        }
    }
}
