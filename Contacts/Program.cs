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
            output.WriteLine("5. Edit contact");
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
                case "5":
                    EditContact(input, output, contactBook, storagePath);
                    break;
                default:
                    output.WriteLine("Invalid option. Please choose 0-5.");
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
            || !TryReadOptional(input, output, "Emails (comma-separated, optional): ", out string? emails)
            || !TryReadOptional(input, output, "Phone number (optional): ", out string? phoneNumber)
            || !TryReadOptional(input, output, "Address (optional): ", out string? address)
            || !TryReadOptional(input, output, "School name (optional): ", out string? schoolName)
            || !TryReadOptional(input, output, "Major (optional): ", out string? major))
        {
            return;
        }

        try
        {
            AddContact(output, contactBook, new Student(name, SplitEmails(emails), phoneNumber, address, schoolName, major),
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
            || !TryReadOptional(input, output, "Emails (comma-separated, optional): ", out string? emails)
            || !TryReadOptional(input, output, "Phone number (optional): ", out string? phoneNumber)
            || !TryReadOptional(input, output, "Address (optional): ", out string? address)
            || !TryReadOptional(input, output, "Employer (optional): ", out string? employer)
            || !TryReadOptional(input, output, "Job title (optional): ", out string? jobTitle))
        {
            return;
        }

        try
        {
            AddContact(output, contactBook, new Employee(name, SplitEmails(emails), phoneNumber, address, employer, jobTitle),
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

    private static string[] SplitEmails(string? emails) =>
        emails?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];

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

    private static void EditContact(
        TextReader input,
        TextWriter output,
        ContactBook contactBook,
        string storagePath)
    {
        IContact[] contacts = contactBook.GetAll();
        if (contacts.Length == 0)
        {
            output.WriteLine("No contacts to edit.");
            return;
        }

        ListContacts(output, contactBook);
        output.Write("Enter the number of the contact to edit: ");
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

        IContact original = contacts[contactNumber - 1];
        output.WriteLine("Press Enter to keep a value, or enter - to clear an optional field.");

        if (!TryReadEdit(input, output, "First name", original.Name.FirstName, out string? firstName, allowClear: false)
            || firstName is null
            || !TryReadEdit(input, output, "Last name", original.Name.LastName, out string? lastName, allowClear: false)
            || lastName is null
            || !TryReadEdit(input, output, "Middle name", original.Name.MiddleName, out string? middleName)
            || !TryReadEdit(input, output, "Emails (comma-separated)", string.Join(", ", original.Emails), out string? emails)
            || !TryReadEdit(input, output, "Phone number", original.PhoneNumber, out string? phoneNumber)
            || !TryReadEdit(input, output, "Address", original.Address, out string? address))
        {
            return;
        }

        try
        {
            FullName name = new(firstName, lastName, middleName);
            Contact updated;
            switch (original)
            {
                case Student student:
                    if (!TryReadEdit(input, output, "School name", student.SchoolName, out string? schoolName)
                        || !TryReadEdit(input, output, "Major", student.Major, out string? major))
                    {
                        return;
                    }

                    updated = new Student(name, SplitEmails(emails), phoneNumber, address, schoolName, major);
                    break;
                case Employee employee:
                    if (!TryReadEdit(input, output, "Employer", employee.Employer, out string? employer)
                        || !TryReadEdit(input, output, "Job title", employee.JobTitle, out string? jobTitle))
                    {
                        return;
                    }

                    updated = new Employee(name, SplitEmails(emails), phoneNumber, address, employer, jobTitle);
                    break;
                default:
                    output.WriteLine("This contact type cannot be edited.");
                    return;
            }

            updated.Id = original.Id;
            contactBook.Remove(original);
            if (!contactBook.Add(updated))
            {
                contactBook.Add(original);
                output.WriteLine("A contact with the same information already exists.");
                return;
            }

            if (!TrySaveContacts(output, contactBook, storagePath))
            {
                contactBook.Remove(updated);
                contactBook.Add(original);
                return;
            }

            output.WriteLine("Contact updated.");
        }
        catch (ArgumentException exception)
        {
            output.WriteLine($"Could not edit contact: {exception.Message}");
        }
    }

    private static bool TryReadEdit(
        TextReader input,
        TextWriter output,
        string label,
        string? current,
        out string? value,
        bool allowClear = true)
    {
        output.Write($"{label} [{current}]: ");
        string? line = input.ReadLine();
        value = string.IsNullOrWhiteSpace(line) ? current : line;
        if (allowClear && line?.Trim() == "-")
        {
            value = null;
        }

        return line is not null;
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
