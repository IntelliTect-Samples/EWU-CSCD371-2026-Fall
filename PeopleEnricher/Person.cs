namespace PeopleEnricher;

public record Person
{
    public Person(
        string firstName,
        string lastName,
        string emailAddress,
        string zipCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(zipCode);

        if (zipCode.Length != 5 ||
            !zipCode.All(char.IsDigit))
        {
            throw new ArgumentException(
                "Zip Code must be exactly 5 digits.",
                nameof(zipCode));
        }

        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        ZipCode = zipCode;
    }
    
    public string FirstName { get; }
    public string LastName { get; }
    public string EmailAddress { get; }
    public string ZipCode { get; }
}