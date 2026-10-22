namespace Contacts;

public static class Program
{
    public static void Main() => Run();

    // Public entry point allows the CLI to be tested with an isolated ContactBook.
    public static void Run(ContactBook? contactBook = null)
    {
        var book = contactBook ?? new ContactBook();
        book.Load();

        while (true)
        {
            Console.WriteLine("\nContacts CLI");
            Console.WriteLine("1. Add a Student");
            Console.WriteLine("2. Add an Employee");
            Console.WriteLine("3. List Contacts");
            Console.WriteLine("4. Remove a Contact");
            Console.WriteLine("5. Edit a Contact");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");

            var choice = Console.ReadLine();

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
                    EditContact(book);
                    break;
                case "6":
                    book.Save();
                    return;
                case null:
                    Console.WriteLine(
                        "Input ended. Saving contacts and exiting.");
                    book.Save();
                    return;
                default:
                    Console.WriteLine(
                        "Invalid option. Please try again.");
                    break;
            }
        }
    }

    private static void AddStudent(ContactBook book)
    {
        var student = ReadStudent();

        if (book.Add(student))
            Console.WriteLine("Student added.");
        else
            Console.WriteLine("Duplicate student - not added.");
    }

    private static void AddEmployee(ContactBook book)
    {
        var employee = ReadEmployee();

        if (book.Add(employee))
            Console.WriteLine("Employee added.");
        else
            Console.WriteLine("Duplicate employee - not added.");
    }

    private static Student ReadStudent(int? preserveId = null)
    {
        var name = ReadName();

        Console.Write("Major: ");
        var major = Console.ReadLine() ?? string.Empty;

        Console.Write("Graduation year: ");
        var graduationYear = Console.ReadLine() ?? string.Empty;

        var emails = ReadEmails();
        var student = new Student(name, major, graduationYear, emails);

        return preserveId is int id
            ? student with { ID = id }
            : student;
    }

    private static Employee ReadEmployee(int? preserveId = null)
    {
        var name = ReadName();

        Console.Write("Department: ");
        var department = Console.ReadLine() ?? string.Empty;

        Console.Write("Company name: ");
        var companyName = Console.ReadLine() ?? string.Empty;

        var emails = ReadEmails();
        var employee = new Employee(name, department, companyName, emails);

        return preserveId is int id
            ? employee with { ID = id }
            : employee;
    }

    private static FullName ReadName()
    {
        Console.Write("First name: ");
        var firstName = Console.ReadLine() ?? string.Empty;

        Console.Write("Last name: ");
        var lastName = Console.ReadLine() ?? string.Empty;

        Console.Write("Middle name (optional): ");
        var middle = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(middle))
            middle = null;

        return new FullName(firstName, lastName, middle);
    }


    private static List<string> ReadEmails()
    {
        var emails = new List<string>();
    
        Console.WriteLine(
            "Enter email addresses (blank line to finish):");
    
        while (true)
        {
            Console.Write("Email: ");
            var input = Console.ReadLine();
    
            if (string.IsNullOrWhiteSpace(input))
            {
                break;
            }
    
            input = input.Trim();
    
            if (!input.Contains('@') || !input.Contains('.'))
            {
                Console.WriteLine(
                    "Invalid email. It must contain both @ and .");
                continue;
            }
    
            emails.Add(input);
        }
    
        return emails;
    }


    private static void ListContacts(ContactBook book)
    {
        DisplayContacts(book.GetAll());
    }

    private static void DisplayContacts(IContact[] contacts)
    {
        if (contacts.Length == 0)
        {
            Console.WriteLine("No contacts found.");
            return;
        }

        Console.WriteLine("\nContacts:");

        for (var index = 0; index < contacts.Length; index++)
        {
            Console.WriteLine(
                $"{index + 1}. {FormatContact(contacts[index])}");
        }
    }

    private static string FormatContact(IContact contact)
    {
        var emails = contact.Emails.Count == 0
            ? "(none)"
            : string.Join(", ", contact.Emails);

        return contact switch
        {
            Student student =>
                $"Student | ID: {student.ID} | Name: {student.name} | " +
                $"Major: {student.Major} | " +
                $"Graduation year: {student.GraduationYear} | " +
                $"Emails: {emails}",

            Employee employee =>
                $"Employee | ID: {employee.ID} | Name: {employee.name} | " +
                $"Department: {employee.Department} | " +
                $"Company: {employee.CompanyName} | Emails: {emails}",

            _ =>
                $"{contact.GetType().Name} | ID: {contact.ID} | " +
                $"Name: {contact.name} | Emails: {emails}"
        };
    }

    private static bool TrySelectContact(
        ContactBook book,
        string action,
        out IContact? selectedContact)
    {
        var contacts = book.GetAll();

        if (contacts.Length == 0)
        {
            Console.WriteLine("No contacts found.");
            selectedContact = null;
            return false;
        }

        DisplayContacts(contacts);
        Console.Write($"Enter the number of the contact to {action}: ");

        var input = Console.ReadLine();

        if (!int.TryParse(input, out var number)
            || number < 1
            || number > contacts.Length)
        {
            Console.WriteLine(
                "Invalid selection. No contact was changed.");
            selectedContact = null;
            return false;
        }

        selectedContact = contacts[number - 1];
        return true;
    }

    private static void RemoveContact(ContactBook book)
    {
        if (!TrySelectContact(book, "remove", out var contact)
            || contact is null)
        {
            return;
        }

        if (book.Remove(contact))
            Console.WriteLine("Contact removed.");
        else
            Console.WriteLine("Contact could not be removed.");
    }

    private static void EditContact(ContactBook book)
    {
        if (!TrySelectContact(book, "edit", out var existingContact)
            || existingContact is null)
        {
            return;
        }

        Console.WriteLine(
            $"Enter replacement details for " +
            $"{existingContact.GetType().Name}.");

        IContact updatedContact = existingContact switch
        {
            Student student => ReadStudent(student.ID),
            Employee employee => ReadEmployee(employee.ID),
            _ => existingContact
        };

        if (!book.Remove(existingContact))
        {
            Console.WriteLine("Contact could not be found.");
            return;
        }

        if (book.Add(updatedContact))
        {
            Console.WriteLine("Contact updated.");
            return;
        }

        // Keep the original if the edit would create a duplicate.
        book.Add(existingContact);

        Console.WriteLine(
            "Edit rejected because it would create a duplicate. " +
            "Original contact kept.");
    }
}
