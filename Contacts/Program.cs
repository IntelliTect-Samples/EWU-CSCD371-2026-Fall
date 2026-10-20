using System.Text.Json;

namespace Contacts;

public static class Program
{
    public static void Main()
    {
        Run(Console.In, Console.Out);
    }

    public static void Run(TextReader input, TextWriter output)
    {
        Run(input, output, "contacts.json");
    }

    public static void Run(TextReader input, TextWriter output, string storagePath)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentException.ThrowIfNullOrWhiteSpace(storagePath);

        ContactBook contactBook;
        try
        {
            contactBook = ContactBook.LoadFromFile(storagePath);
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or JsonException
            or NotSupportedException
            or ArgumentException)
        {
            output.WriteLine($"Could not load contacts: {exception.Message}");
            return;
        }

        while (true)
        {
            output.WriteLine();
            output.WriteLine("Contacts");
            output.WriteLine("1. Add student");
            output.WriteLine("2. Add employee");
            output.WriteLine("3. List contacts");
            output.WriteLine("4. Remove contact");
            output.WriteLine("0. Exit");
            output.Write("Select an option: ");

            string? choice = input.ReadLine();
            if (choice is null || choice == "0")
            {
                output.WriteLine("Goodbye.");
                return;
            }

            switch (choice)
            {
                case "1":
                    AddStudent(input, output, contactBook, storagePath);
                    break;
                case "2":
                    AddEmployee(input, output, contactBook, storagePath);
                    break;
                case "3":
                    ListContacts(output, contactBook);
                    break;
                case "4":
                    RemoveContact(input, output, contactBook, storagePath);
                    break;
                default:
                    output.WriteLine("Invalid option. Please choose 0-4.");
                    break;
            }
        }
    }

    private static void AddStudent(
        TextReader input,
        TextWriter output,
        ContactBook contactBook,
        string storagePath)
    {
        if (!TryReadName(input, output, out FullName? name)
            || name is null
            || !TryReadOptional(input, output, "Email (optional): ", out string? email)
            || !TryReadOptional(input, output, "Phone number (optional): ", out string? phoneNumber)
            || !TryReadOptional(input, output, "Address (optional): ", out string? address)
            || !TryReadOptional(input, output, "School name (optional): ", out string? schoolName)
            || !TryReadOptional(input, output, "Major (optional): ", out string? major))
        {
            return;
        }

        try
        {
            AddContact(output, contactBook, new Student(name, email, phoneNumber, address, schoolName, major),
                storagePath);
        }
        catch (ArgumentException exception)
        {
            output.WriteLine($"Could not add student: {exception.Message}");
        }
    }

    private static void AddEmployee(
        TextReader input,
        TextWriter output,
        ContactBook contactBook,
        string storagePath)
    {
        if (!TryReadName(input, output, out FullName? name)
            || name is null
            || !TryReadOptional(input, output, "Email (optional): ", out string? email)
            || !TryReadOptional(input, output, "Phone number (optional): ", out string? phoneNumber)
            || !TryReadOptional(input, output, "Address (optional): ", out string? address)
            || !TryReadOptional(input, output, "Employer (optional): ", out string? employer)
            || !TryReadOptional(input, output, "Job title (optional): ", out string? jobTitle))
        {
            return;
        }

        try
        {
            AddContact(output, contactBook, new Employee(name, email, phoneNumber, address, employer, jobTitle),
                storagePath);
        }
        catch (ArgumentException exception)
        {
            output.WriteLine($"Could not add employee: {exception.Message}");
        }
    }

    private static bool TryReadName(TextReader input, TextWriter output, out FullName? name)
    {
        name = null;
        string? firstName = ReadRequired(input, output, "First name: ");
        if (firstName is null)
        {
            return false;
        }

        string? lastName = ReadRequired(input, output, "Last name: ");
        if (lastName is null)
        {
            return false;
        }

        if (!TryReadOptional(input, output, "Middle name (optional): ", out string? middleName))
        {
            return false;
        }

        name = new FullName(firstName, lastName, middleName);
        return true;
    }

    private static string? ReadRequired(TextReader input, TextWriter output, string prompt)
    {
        while (true)
        {
            output.Write(prompt);
            string? value = input.ReadLine();
            if (value is null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            output.WriteLine("This field is required.");
        }
    }

    private static bool TryReadOptional(
        TextReader input,
        TextWriter output,
        string prompt,
        out string? value)
    {
        output.Write(prompt);
        string? line = input.ReadLine();
        value = string.IsNullOrWhiteSpace(line) ? null : line;
        return line is not null;
    }

    private static void AddContact(
        TextWriter output,
        ContactBook contactBook,
        IContact contact,
        string storagePath)
    {
        if (!contactBook.Add(contact))
        {
            output.WriteLine("A contact with the same information already exists.");
            return;
        }

        if (!TrySaveContacts(output, contactBook, storagePath))
        {
            contactBook.Remove(contact);
            return;
        }

        output.WriteLine("Contact added.");
    }

    private static void ListContacts(TextWriter output, ContactBook contactBook)
    {
        IContact[] contacts = contactBook.GetAll();
        if (contacts.Length == 0)
        {
            output.WriteLine("No contacts found.");
            return;
        }

        for (int index = 0; index < contacts.Length; index++)
        {
            string contactType = contacts[index] switch
            {
                Student => "Student",
                Employee => "Employee",
                _ => "Contact"
            };

            output.WriteLine($"{index + 1}. {contactType}");
            output.Write(contacts[index].ToString());
        }
    }

    private static void RemoveContact(
        TextReader input,
        TextWriter output,
        ContactBook contactBook,
        string storagePath)
    {
        IContact[] contacts = contactBook.GetAll();
        if (contacts.Length == 0)
        {
            output.WriteLine("No contacts to remove.");
            return;
        }

        ListContacts(output, contactBook);
        output.Write("Enter the number of the contact to remove: ");
        string? selection = input.ReadLine();
        if (selection is null)
        {
            return;
        }

        if (!int.TryParse(selection, out int contactNumber)
            || contactNumber < 1
            || contactNumber > contacts.Length)
        {
            output.WriteLine("Invalid contact number.");
            return;
        }

        if (contactBook.Remove(contacts[contactNumber - 1]))
        {
            if (!TrySaveContacts(output, contactBook, storagePath))
            {
                contactBook.Add(contacts[contactNumber - 1]);
                return;
            }

            output.WriteLine("Contact removed.");
        }
    }

    private static bool TrySaveContacts(TextWriter output, ContactBook contactBook, string storagePath)
    {
        try
        {
            contactBook.SaveToFile(storagePath);
            return true;
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or JsonException
            or NotSupportedException
            or ArgumentException)
        {
            output.WriteLine($"Could not save contacts: {exception.Message}");
            return false;
        }
    }
}
