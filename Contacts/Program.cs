using System.Text.Json;

namespace Contacts;

public static class Program
{
    public static void Main()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "contacts.json");

        ContactBook book = Load(path, Console.Out);
        Run(book, Console.In, Console.Out);
        Save(book, path, Console.Out);
    }

    public static ContactBook Load(string path, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);

        try
        {
            return ContactStorage.Load(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or ArgumentException)
        {
            output.WriteLine($"Could not load contacts, starting empty: {ex.Message}");
            return new ContactBook();
        }
    }

    public static void Save(ContactBook book, string path, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);

        try
        {
            ContactStorage.Save(book, path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            output.WriteLine($"Could not save contacts: {ex.Message}");
        }
    }

    public static void Run(ContactBook book, TextReader input, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);

        while (true)
        {
            output.WriteLine("1) Add student");
            output.WriteLine("2) Add employee");
            output.WriteLine("3) List contacts");
            output.WriteLine("4) Remove contact");
            output.WriteLine("5) Edit contact");
            output.WriteLine("0) Quit");
            output.Write("Choose: ");

            string? choice = input.ReadLine();

            if (choice is null || choice == "0")
            {
                return;
            }

            switch (choice)
            {
                case "1":
                    AddStudent(book, input, output);
                    break;
                case "2":
                    AddEmployee(book, input, output);
                    break;
                case "3":
                    ListContacts(book, output);
                    break;
                case "4":
                    RemoveContact(book, input, output);
                    break;
                case "5":
                    EditContact(book, input, output);
                    break;
                default:
                    output.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    private static void AddStudent(ContactBook book, TextReader input, TextWriter output)
    {
        try
        {
            FullName name = ReadName(input, output);
            string[] emails = SplitEmails(Prompt("Emails (comma-separated): ", input, output));
            string major = Prompt("Major: ", input, output);
            string phone = Prompt("Phone (optional): ", input, output);

            ReportAdd(book.Add(new Student(name, emails, major, phone)), output);
        }
        catch (ArgumentException ex)
        {
            output.WriteLine($"Invalid input: {ex.Message}");
        }
    }

    private static void AddEmployee(ContactBook book, TextReader input, TextWriter output)
    {
        try
        {
            FullName name = ReadName(input, output);
            string[] emails = SplitEmails(Prompt("Emails (comma-separated): ", input, output));
            string jobTitle = Prompt("Job title: ", input, output);
            string phone = Prompt("Phone (optional): ", input, output);

            ReportAdd(book.Add(new Employee(name, emails, jobTitle, phone)), output);
        }
        catch (ArgumentException ex)
        {
            output.WriteLine($"Invalid input: {ex.Message}");
        }
    }

    private static void ListContacts(ContactBook book, TextWriter output)
    {
        IContact[] contacts = book.GetAll();

        if (contacts.Length == 0)
        {
            output.WriteLine("No contacts.");
            return;
        }

        for (int i = 0; i < contacts.Length; i++)
        {
            output.WriteLine($"{i + 1}) {contacts[i].Name} - {string.Join(", ", contacts[i].Emails)}");
        }
    }

    private static void RemoveContact(ContactBook book, TextReader input, TextWriter output)
    {
        IContact? contact = ChooseContact(book, "Number to remove: ", input, output);

        if (contact is null)
        {
            return;
        }

        output.WriteLine(book.Remove(contact) ? "Contact removed." : "Contact not found.");
    }

    private static void EditContact(ContactBook book, TextReader input, TextWriter output)
    {
        IContact? existing = ChooseContact(book, "Number to edit: ", input, output);

        if (existing is null)
        {
            return;
        }

        output.WriteLine("Press Enter to keep a value, or - to clear an optional one.");

        try
        {
            IContact updated = existing switch
            {
                Student student => new Student(
                    EditName(student.Name, input, output),
                    KeepEmails(student.Emails, input, output),
                    Keep("Major", student.Major, input, output),
                    KeepOptional("Phone", student.Phone, input, output)) { Id = student.Id },
                Employee employee => new Employee(
                    EditName(employee.Name, input, output),
                    KeepEmails(employee.Emails, input, output),
                    Keep("Job title", employee.JobTitle, input, output),
                    KeepOptional("Phone", employee.Phone, input, output)) { Id = employee.Id },
                _ => throw new InvalidOperationException($"Cannot edit a {existing.GetType().Name}."),
            };

            output.WriteLine(book.Replace(existing, updated) ? "Contact updated." : "That contact already exists.");
        }
        catch (ArgumentException ex)
        {
            output.WriteLine($"Invalid input: {ex.Message}");
        }
    }

    private static IContact? ChooseContact(ContactBook book, string label, TextReader input, TextWriter output)
    {
        IContact[] contacts = book.GetAll();

        if (contacts.Length == 0)
        {
            output.WriteLine("No contacts.");
            return null;
        }

        ListContacts(book, output);

        string number = Prompt(label, input, output);

        if (!int.TryParse(number, out int index) || index < 1 || index > contacts.Length)
        {
            output.WriteLine("Invalid number.");
            return null;
        }

        return contacts[index - 1];
    }

    private static void ReportAdd(bool added, TextWriter output)
    {
        output.WriteLine(added ? "Contact added." : "That contact already exists.");
    }

    private static string Prompt(string label, TextReader input, TextWriter output)
    {
        output.Write(label);
        return input.ReadLine() ?? "";
    }

    private static FullName ReadName(TextReader input, TextWriter output)
    {
        string first = Prompt("First name: ", input, output);
        string middle = Prompt("Middle name (optional): ", input, output);
        string last = Prompt("Last name: ", input, output);

        return new FullName(first, last, middle);
    }

    private static FullName EditName(FullName name, TextReader input, TextWriter output)
    {
        string first = Keep("First name", name.FirstName, input, output);
        string? middle = KeepOptional("Middle name", name.MiddleName, input, output);
        string last = Keep("Last name", name.LastName, input, output);

        return new FullName(first, last, middle);
    }

    private static string Keep(string label, string current, TextReader input, TextWriter output)
    {
        string value = Prompt($"{label} [{current}]: ", input, output);
        return string.IsNullOrWhiteSpace(value) ? current : value;
    }

    private static string[] KeepEmails(IReadOnlyList<string> current, TextReader input, TextWriter output)
    {
        string value = Keep("Emails (comma-separated)", string.Join(", ", current), input, output);
        return SplitEmails(value);
    }

    private static string[] SplitEmails(string value) =>
        value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static string? KeepOptional(string label, string? current, TextReader input, TextWriter output)
    {
        string value = Prompt($"{label} [{current}]: ", input, output);

        if (value.Trim() == "-")
        {
            return null;
        }

        return string.IsNullOrWhiteSpace(value) ? current : value;
    }
}
