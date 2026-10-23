using System.Globalization;

namespace Contacts;

/// <summary>
/// The menu-driven user interface. It talks only to the reader and writer it is given,
/// so tests can drive it with strings instead of a real console.
/// </summary>
public sealed class ContactCli(ContactBook book, TextReader input, TextWriter output)
{
    public void Run()
    {
        while (true)
        {
            PrintMenu();
            string? choice = Prompt("Choose an option: ")?.Trim();

            if (choice is null or "0")
            {
                output.WriteLine("Goodbye.");
                return;
            }

            try
            {
                HandleChoice(choice);
            }
            catch (ArgumentException ex)
            {
                output.WriteLine($"Invalid input: {ex.Message}");
            }
        }
    }

    private void HandleChoice(string choice)
    {
        switch (choice)
        {
            case "1":
                AddStudent();
                break;
            case "2":
                AddEmployee();
                break;
            case "3":
                ListContacts();
                break;
            case "4":
                RemoveContact();
                break;
            default:
                output.WriteLine("Unknown option. Please choose 0-4.");
                break;
        }
    }

    private void PrintMenu()
    {
        output.WriteLine();
        output.WriteLine("1. Add student");
        output.WriteLine("2. Add employee");
        output.WriteLine("3. List contacts");
        output.WriteLine("4. Remove contact");
        output.WriteLine("0. Exit");
    }

    private void AddStudent()
    {
        if (ReadName() is not { } name
            || ReadRequired("Email: ") is not { } email
            || ReadRequired("School: ") is not { } school)
        {
            return;
        }

        AddContact(new Student(name, email, school));
    }

    private void AddEmployee()
    {
        if (ReadName() is not { } name
            || ReadRequired("Email: ") is not { } email
            || ReadRequired("Company: ") is not { } company
            || ReadRequired("Job title: ") is not { } title)
        {
            return;
        }

        AddContact(new Employee(name, email, company, title));
    }

    private void AddContact(IContact contact)
    {
        if (book.Add(contact))
        {
            output.WriteLine($"Added {contact.Describe()}");
        }
        else
        {
            output.WriteLine("That contact already exists. Nothing was added.");
        }
    }

    private void ListContacts()
    {
        IContact[] contacts = GetSortedContacts();

        if (contacts.Length == 0)
        {
            output.WriteLine("There are no contacts.");
            return;
        }

        PrintNumbered(contacts);
    }

    private void RemoveContact()
    {
        IContact[] contacts = GetSortedContacts();

        if (contacts.Length == 0)
        {
            output.WriteLine("There are no contacts to remove.");
            return;
        }

        PrintNumbered(contacts);

        string? text = Prompt("Number to remove (blank to cancel): ")?.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out int number)
            || number < 1
            || number > contacts.Length)
        {
            output.WriteLine("Please enter a number from the list.");
            return;
        }

        IContact selected = contacts[number - 1];
        book.Remove(selected);
        output.WriteLine($"Removed {selected.Describe()}");
    }

    // Sorted so the numbers shown to the user are stable between listing and removing.
    private IContact[] GetSortedContacts() =>
        book.GetAll()
            .OrderBy(c => c.Name.Last, StringComparer.OrdinalIgnoreCase)
            .ThenBy(c => c.Name.First, StringComparer.OrdinalIgnoreCase)
            .ThenBy(c => c.Describe(), StringComparer.Ordinal)
            .ToArray();

    private void PrintNumbered(IContact[] contacts)
    {
        for (int i = 0; i < contacts.Length; i++)
        {
            output.WriteLine($"{i + 1}. {contacts[i].Describe()}");
        }
    }

    private FullName? ReadName()
    {
        if (ReadRequired("First name: ") is not { } first)
        {
            return null;
        }

        string? middle = Prompt("Middle name (optional): ")?.Trim();

        if (ReadRequired("Last name: ") is not { } last)
        {
            return null;
        }

        return new FullName(first, last, middle);
    }

    /// <summary>Asks until a non-blank answer is given. Returns <c>null</c> if the input ends.</summary>
    private string? ReadRequired(string label)
    {
        while (true)
        {
            string? value = Prompt(label)?.Trim();

            if (value is null)
            {
                return null;
            }

            if (value.Length > 0)
            {
                return value;
            }

            output.WriteLine("A value is required.");
        }
    }

    private string? Prompt(string label)
    {
        output.Write(label);
        return input.ReadLine();
    }
}
