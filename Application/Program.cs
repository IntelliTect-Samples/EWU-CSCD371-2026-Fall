using Contacts;
using Persistence;

namespace Application;

public static class Program
{
    /// <summary>
    /// Runs the contact book command-line application.
    /// </summary>
    public static void Main()
    {
        string pathToSave = Path.Combine(
            Directory.GetCurrentDirectory(),
            "ContactBook.json");
        
        ContactBook contactBook = new();

        JsonDataService dataService = new(pathToSave);

        foreach (IContact savedContact in dataService.Load()) 
            contactBook.Add(savedContact);

        // Main Menu
        while (true)
        {
            Console.WriteLine("********** Main Menu **********");
            Console.WriteLine("1. Add a new contact.");
            Console.WriteLine("2. List all contacts.");
            Console.WriteLine("3. Remove a contact.");
            Console.WriteLine("4. Edit a contact.");
            Console.WriteLine("5. Exit the application.");
            
            Console.Write("Please select an option (1-5): ");
            string? input = Console.ReadLine();

            switch (input?.Trim())
            {
                case "1":
                    PromptCreateContact();
                    break;
                
                case "2":
                    if (HasContacts(true))
                        DisplayContacts();
                    WaitForUser();
                    break;
                
                case "3":
                    if (HasContacts(true))
                        PromptRemoveContact();
                    break;
                
                case "4":
                    if (HasContacts(true))
                        PromptEditContact();
                    break;
                
                case "5":
                    dataService.Save(contactBook.GetAll());
                    Console.WriteLine();
                    Console.WriteLine("Goodbye!");
                    return;
                
                default:
                    Console.WriteLine("Invalid input.");
                    break;
            }
        }

        bool HasContacts(bool notifyIfEmpty)
        {
            if (contactBook.GetAll().Length > 0)
                return true;

            if (!notifyIfEmpty) return false;
            
            Console.WriteLine();
            Console.WriteLine("No contacts available");
            Console.WriteLine();
            return false;
        }

        #region Contact Creation

        void PromptCreateContact()
        {
            Console.WriteLine();
            Console.WriteLine("********** Add Contact **********");
            while (true)
            {
                Console.WriteLine("1. Add a new Employee.");
                Console.WriteLine("2. Add a new Student.");
                Console.WriteLine();

                Console.Write("Please select a contact type to add (1-2): ");
                string? input = Console.ReadLine();
                
                switch (input?.Trim())
                {
                    case "1":
                        PromptCreateEmployee();
                        return;
                
                    case "2":
                        PromptCreateStudent();
                        return;
                
                    default:
                        Console.WriteLine("Invalid input.");
                        Console.WriteLine();
                        break;
                }
            }
        }

        void PromptCreateEmployee()
        {
            int nextNumber = contactBook.GetNextId(nameof(Employee));
            
            Employee employeeToAdd = new(
                PromptFullName(),
                PromptEmails("Please Provide an email for the contact")
            )
            {
                Id = new ContactId(Employee.Prefix, nextNumber)
            };
            
            while (contactBook.Contains(employeeToAdd))
            {
                Console.WriteLine("Contact already exists.");

                if (!PromptYesNo("Edit this contact?"))
                    return;
                
                employeeToAdd = (Employee)EditContactMenu(employeeToAdd);
            }
            AddContact(employeeToAdd);
        }
        
        void PromptCreateStudent()
        {
            int nextIdNumber = contactBook.GetNextId(nameof(Student));

            Student studentToAdd = new(
                PromptFullName(),
                PromptEmails("Please provide an email for the contact")
            )
            {
                Id = new ContactId(Student.Prefix, nextIdNumber)
            };

            // While loop for editing?
            while (contactBook.Contains(studentToAdd))
            {
                Console.WriteLine("Contact already exists.");
                
                if (!PromptYesNo("Edit this contact?"))
                    return;
                
                studentToAdd = (Student)EditContactMenu(studentToAdd);
            }
            AddContact(studentToAdd);
        }
        
        void AddContact(IContact contact)
        {
            contactBook.Add(contact);
            dataService.Save(contactBook.GetAll());
            
            Console.WriteLine("Contact was Added");
            Console.WriteLine();
        }

        #endregion

        #region Contact Display

        IContact PromptContactSelection()
        {
            IContact[] contacts = DisplayContacts();

            while (true)
            {
                Console.Write("Select Contact by number: ");
                string? input = Console.ReadLine();
                
                if (int.TryParse(input, out int selection) 
                    && selection >= 1 && selection <= contacts.Length)
                    return contacts[selection - 1];
                
                Console.WriteLine("Invalid selection.");
            }
        }
        
        IContact[] DisplayContacts()
        {
            IContact[] contacts = contactBook.GetAll();
            
            Console.WriteLine();
            Console.WriteLine("********** Contact List **********");

            for (int i = 0; i < contacts.Length; i++)
            {
                Console.WriteLine(
                    $"{i + 1}. {DisplayContactInfo(contacts[i])}");
            }
            
            Console.WriteLine("********** End Contact List **********");
            Console.WriteLine();
            
            return contacts;
        }
        
        string DisplayContactInfo(IContact contact)
        {
            return
                $"{contact.GetType().Name}: " +
                $"{contact.Id}, " +
                $"{DisplayFullName(contact.Name)} " +
                $"({DisplayEmails(contact.Emails)})";
        }
        
        string DisplayFullName(FullName name)
        {
            return string.IsNullOrWhiteSpace(name.MiddleName)
                ? $"{name.FirstName} {name.LastName}"
                : $"{name.FirstName} {name.MiddleName} {name.LastName}";
        }

        string DisplayEmails(ContactEmails emails)
        {
            return string.Join(", ", emails.Emails);
        }

        #endregion

        #region Contact Removal

        void PromptRemoveContact()
        {
            while (true)
            {
                IContact selectedContact = PromptContactSelection();

                Console.WriteLine();
                Console.WriteLine("You have selected to remove:");
                Console.WriteLine(DisplayContactInfo(selectedContact));
                Console.WriteLine();

                if (!PromptYesNo("Is this correct?"))
                {
                    Console.WriteLine("Removal aborted.");
                    Console.WriteLine();

                    if (!PromptYesNo("Reselect for removal?"))
                        return;

                    continue;
                }
                
                RemoveContact(selectedContact);

                // removed last contact must end loop
                if (!HasContacts(false))
                {
                    Console.WriteLine("Contact List is now Empty.");
                    Console.WriteLine();
                    WaitForUser();
                    return;
                }
                
                if (!PromptYesNo("Remove another Contact?"))
                    return;
            }
        }

        void RemoveContact(IContact contact)
        {
            contactBook.Remove(contact);
            dataService.Save(contactBook.GetAll());
            
            Console.WriteLine("Contact was Removed.");
            Console.WriteLine();
        }

        #endregion

        #region Contact Edits

        void PromptEditContact()
        {
            while (true)
            {
                IContact selectedContact = PromptContactSelection();
                
                IContact updatedContact = EditContactMenu(selectedContact);

                while (!updatedContact.Equals(selectedContact)
                       && contactBook.Contains(updatedContact))
                {
                    Console.WriteLine();
                    Console.WriteLine("A Contact with those details already exists.");
                    Console.WriteLine();

                    if (!PromptYesNo("Edit this contact?"))
                        return;
                    
                    updatedContact = EditContactMenu(updatedContact);
                }
                
                EditContact(selectedContact, updatedContact);
                
                if (!PromptYesNo("Edit another contact?"))
                    return;
            }
        }

        IContact EditContactMenu(IContact contactToEdit)
        {
            string firstName = contactToEdit.Name.FirstName;
            string lastName = contactToEdit.Name.LastName;
            string? middleName = contactToEdit.Name.MiddleName;
            ContactEmails emails = contactToEdit.Emails;
            ContactId id = contactToEdit.Id;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("********** Edit Contact **********");
                Console.WriteLine($"1. First Name : {firstName}");
                Console.WriteLine($"2. Last Name : {lastName}");

                Console.WriteLine($"3. Middle Name : " +
                                  $"{middleName ?? "(none)"}");

                Console.WriteLine($"4. Email(s) : {DisplayEmails(emails)}");
                Console.WriteLine("5. Save Changes");
                
                Console.Write("Select field to edit: ");
                string? input = Console.ReadLine();

                switch (input?.Trim())
                {
                    case "1":
                        firstName = PromptName("Update First Name to");
                        break;

                    case "2":
                        lastName = PromptName("Update Last Name to");
                        break;

                    case "3":
                        middleName = PromptOptionalName("Update Middle Name to");
                        break;

                    case "4":
                        emails = EditEmailsMenu(emails);
                        break;

                    case "5":
                        return RebuildContact();

                    default:
                        Console.WriteLine("Invalid selection.");
                        break;
                }
            }
            
            IContact RebuildContact()
            {
                FullName fullName = new(
                    firstName,
                    lastName,
                    middleName
                );

                return contactToEdit switch
                {
                    Student => new Student(fullName, emails) { Id = id },
                    Employee => new Employee(fullName, emails) { Id = id },
                    _ => throw new InvalidOperationException()
                };
            }
        }

        void EditContact(IContact contactToRemove, IContact contactToAdd)
        {
            contactBook.Remove(contactToRemove);
            contactBook.Add(contactToAdd);
            dataService.Save(contactBook.GetAll());
            
            Console.WriteLine("Contact was Edited.");
            Console.WriteLine();
        }

        #endregion

        #region Edit Emails

        ContactEmails EditEmailsMenu(ContactEmails emails)
        {
            List<string> editableEmails = emails.Emails.ToList();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("********** Edit Emails **********");
                Console.WriteLine("1. Add Email");
                Console.WriteLine("2. Remove Email");
                Console.WriteLine("3. Edit Email");
                Console.WriteLine("4. Return");
                Console.WriteLine();

                Console.Write("Select option: ");
                string? input = Console.ReadLine();

                switch (input?.Trim())
                {
                    case "1":
                        AddEmail(editableEmails);
                        break;
                    
                    case "2":
                        RemoveEmail(editableEmails);
                        break;
                    
                    case "3":
                        EditEmail(editableEmails);
                        break;
                    
                    case "4":
                        return new ContactEmails(editableEmails);
                    
                    default:
                        Console.WriteLine("Invalid selection.");
                        break;
                }
            }
        }

        void AddEmail(List<string> currentEmailList)
        {
            string email = PromptSingleEmail("Please Provide Email");

            if (currentEmailList.Contains(email))
            {
                Console.WriteLine("Email already exists.");
                return;
            }
            currentEmailList.Add(email);
        }

        void RemoveEmail(List<string> currentEmailList)
        {
            if (currentEmailList.Count <= 1)
            {
                Console.WriteLine("A Contact must maintain at least one email, " +
                        "try edit email if this email is no longer wanted");
                return;
            }
            int selectedIndex = PromptEmailSelection(currentEmailList);
            
            currentEmailList.RemoveAt(selectedIndex);
        }

        void EditEmail(List<string> currentEmailList)
        {
            int selectedIndex = PromptEmailSelection(currentEmailList);
            
            string updatedEmail = PromptSingleEmail("Update Email to");

            if (currentEmailList.Contains(updatedEmail))
            {
                Console.WriteLine("Email already exists.");
                return;
            }
            currentEmailList[selectedIndex] = updatedEmail;
        }

        int PromptEmailSelection(IReadOnlyList<string> emails)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("********** Email List **********");

                for (int i = 0; i < emails.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {emails[i]}");
                }
                
                Console.WriteLine();
                Console.Write("Select Email by Number: ");
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int selection)
                    && selection >= 1
                    && selection <= emails.Count)
                    
                    return selection - 1;
                
                Console.WriteLine("Invalid selection.");
            }
        }

        #endregion
        
        #region Prompt Helpers

        FullName PromptFullName()
        {
            return new FullName(
                PromptName("Please provide First Name for contact"),
                PromptName("Please provide Last Name for contact"),
                PromptOptionalName("Please provide Middle Name for contact")
            );
        }
        
        ContactEmails PromptEmails(string prompt)
        {
            List<string> emails = [];
            
            emails.Add(PromptSingleEmail(prompt));
            
            while (PromptYesNo("Add another email?"))
            {
                emails.Add(PromptSingleEmail("enter email here:"));
            }
            return new ContactEmails(emails);
        }

        string PromptSingleEmail(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                string? input = Console.ReadLine();
                
                if (ContactUtilities.ValidateEmail(input, out string error)
                    && input is not null)
                    return input;
                
                Console.WriteLine(error);
            }
        }

        string PromptName(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                string? input = Console.ReadLine();

                if (ContactUtilities.ValidateName(input, out string error)
                    && input is not null)
                    return input;
                
                Console.WriteLine(error);
            }
        }

        string? PromptOptionalName(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} (Optional): ");
                string? input = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(input))
                    return null;
                
                if (ContactUtilities.ValidateName(input, out string error))
                    return input;
                
                Console.WriteLine(error);
            }
        }

        bool PromptYesNo(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} (y/n): ");
                string? input = Console.ReadLine();
                
                if (string.Equals(input?.Trim(), "y",
                        StringComparison.OrdinalIgnoreCase))
                    return true;

                if (string.Equals(input?.Trim(), "n",
                        StringComparison.OrdinalIgnoreCase))
                    return false;

                Console.WriteLine("Please enter 'y' or 'n'");
            }
        }

        #endregion

        void WaitForUser()
        {
            Console.Write("Press enter to continue...");
            Console.ReadLine();
            Console.WriteLine();
        }
    }
}
