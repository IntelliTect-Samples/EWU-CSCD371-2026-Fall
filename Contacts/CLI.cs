using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Contacts;
public class CLI
{

    private readonly ContactBook _contactBook;
    private readonly TextReader _in;
    private readonly TextWriter _out;

    public CLI(ContactBook contactbook, TextReader? @in = null, TextWriter? @out = null)
    {
        _contactBook = contactbook;
        _in = @in ?? Console.In;
        _out = @out ?? Console.Out;
    }

    internal static string? FormatContact(IContact contact) => contact switch
    {
        Employee e => $"{e.Name,20} | {e.Email,20} | {e.Company,30} | {e.JobTitle,20} | {e.Phone??""}",
        Student s => $"{s.Name,20} | {s.Email,20} | {s.School,30} | {s.Major,20} | {s.Phone??""}",
        _ => null
    };

    public void List()
    {
        _out.WriteLine("Contacts");
        _out.WriteLine("---------------------------------------------------------------------------------------------");
        foreach (IContact? contact in _contactBook.GetAll().OrderBy(c => c.Name.lastName))
        {
            string? contactString = FormatContact(contact);
            if (contactString != null)
            {
                _out.WriteLine(contactString);
            }
        }
    }

    public FullName EnterName()
    {
        string firstName = PromptUser("Please enter your contact's first name"
            , "Error! Your contact's first name must have at least 1 character!" +
              "Please try again...")!.Trim(); // reprompts until not null

        string? middleName = PromptUser("Please enter your contact's middle name (optional)>");

        string lastName = PromptUser("Please enter your contact's last name "
            , "Error! Your contact's last name must have at least 1 character!\n" +
              "Please try again...")!.Trim();

        FullName fullName = new(firstName, lastName, middleName);

        return fullName;
    }

    public Employee EnterEmployeeContact()
    {
        FullName name = EnterName();

        string email = PromptUser("Please enter your contact's email\n> " 
            , "Error! Your contact's email must have at least 1 character!\n" +
              "Please try again...")!;

        string company = PromptUser("Please enter your contact's company\n> "
            , "Error! Your contact's company must have at least 1 character!\n" +
              "Please try again...")!;


        string jobTitle = PromptUser("Please enter your contact's job title\n> "
            , "Error! Your contact's job title must have at least 1 character!\n" +
              "Please try again...")!;

        string? phone = PromptUser("Please enter your contact's phone number(optional) ");

        return new(name, email, company, jobTitle, phone);
    }

    public Student EnterStudentContact()
    {
        FullName name = EnterName();

        string email = PromptUser("Please enter your contact's email"
            , "Error! Your contact's email must have at least 1 character!\n" +
              "Please try again...")!;

        string school = PromptUser("Please enter your contact's school "
            , "Error! Your contact's school must have at least 1 character!\n" +
              "Please try again...")!;

        string major = PromptUser("Please enter your contact's major"
            , "Error! Your contact's major must have at least 1 character!\n" +
              "Please try again...")!;

        string? phone = PromptUser("Please enter your contact's phone number(optional)");

        return new(name, email, school, major, phone);
    }
    public void EnterRemoveFromList()
    {
        var contactList = _contactBook.GetAll().OrderBy(c => c.Name).ToList();

        if (contactList.Count == 0)
        {
            _out.WriteLine("Contact list is empty...");
            return;
        }

        for (int i = 0; i < contactList.Count; i++)
        {
            _out.WriteLine($"{i} -> {FormatContact(contactList[i])}");
        }

        _out.WriteLine("Enter the corresponding number to remove... ");
        IContact contact;

        bool isValid = int.TryParse(_in.ReadLine(), out int id) && id >= 0 && id < contactList.Count;

        if (isValid)
        {
            contact = contactList[id];
            _contactBook.Remove(contact);
            _out.WriteLine("removed");
        }

    }

    public string? PromptUser(string prompt, string? reprompt = null)
    {
        _out.WriteLine(prompt);

        string? result = _in.ReadLine();
        if (reprompt is null)
        {
            return result;
        }

        while (string.IsNullOrWhiteSpace(result))
        {
            _out.WriteLine(reprompt);
            result = _in.ReadLine();
        }

        return result;
    }

    public  IContact EnterContact()
    {
        _out.WriteLine("Enter Contact Type (Student or Employee)");
        string? type = _in.ReadLine();

        while (type is null || (type.Trim() != "Student" && type.Trim() != "Employee"))
        {
            _out.WriteLine("Invalid input! Try again...");
            type = _in.ReadLine();
        }

        type = type.Trim();

        return type switch
        {
            "Student" => EnterStudentContact(),
            "Employee" => EnterEmployeeContact(),
            _ => EnterEmployeeContact()
        };
    }

    public void Run()
    {
        bool running = true;
        while(running)
        {
            string textOut =
            "-----------------------------------\n" +
            "Welcome To Your Contact Book!\n" +
            "-----------------------------------\n" +
            "Enter 'List' to view your contacts\n" +
            "Enter 'Add' to add a student contact\n" +
            "Enter 'Remove' to remove a contact\n" +
            "Enter 'Exit' to exit the program";
            _out.WriteLine(textOut);
            string? textIn = _in.ReadLine();

            switch (textIn)
            {
                case "List":
                    List();
                    break;
                case "Add":
                    if (!_contactBook.Add(EnterContact()))
                    {
                        _out.WriteLine("Contact already exists in the contact book");
                    }
                    break;
                case "Remove":
                    EnterRemoveFromList();
                    break;
                case "Exit":
                    running = false;
                    break;
            }
        }
    }



}
