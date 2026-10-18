namespace Contacts;

public static class Program
{
    public static void Main()
    {
        Run(new ContactBook(), Console.In, Console.Out);
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
            string email = Prompt("Email: ", input, output);
            string major = Prompt("Major: ", input, output);
            string phone = Prompt("Phone (optional): ", input, output);

            ReportAdd(book.Add(new Student(name, email, major, phone)), output);
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
            string email = Prompt("Email: ", input, output);
            string jobTitle = Prompt("Job title: ", input, output);
            string phone = Prompt("Phone (optional): ", input, output);

            ReportAdd(book.Add(new Employee(name, email, jobTitle, phone)), output);
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
            output.WriteLine($"{i + 1}) {contacts[i].Name} - {contacts[i].Email}");
        }
    }

    private static void RemoveContact(ContactBook book, TextReader input, TextWriter output)
    {
        IContact[] contacts = book.GetAll();

        if (contacts.Length == 0)
        {
            output.WriteLine("No contacts.");
            return;
        }

        ListContacts(book, output);

        string number = Prompt("Number to remove: ", input, output);

        if (!int.TryParse(number, out int index) || index < 1 || index > contacts.Length)
        {
            output.WriteLine("Invalid number.");
            return;
        }

        output.WriteLine(book.Remove(contacts[index - 1]) ? "Contact removed." : "Contact not found.");
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
}
