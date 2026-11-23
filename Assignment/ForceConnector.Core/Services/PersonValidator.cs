using ForceConnector.Core.Interfaces;
using ForceConnector.Core.Models;
using System.Text.RegularExpressions;

namespace ForceConnector.Core.Validation;

public sealed class PersonValidator : IValidator<Person>
{
    public bool IsValid(Person value)
    {
        return GetErrors(value).Count == 0;
    }

    public IReadOnlyList<string> GetErrors(Person value)
    {
        List<string> errors = [];

        if (string.IsNullOrWhiteSpace(value.FirstName))
        {
            errors.Add("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(value.LastName))
        {
            errors.Add("Last name is required.");
        }

        if (string.IsNullOrWhiteSpace(value.EmailAddress))
        {
            errors.Add("Email address is required.");
        }
        else if (!value.EmailAddress.Contains('@'))
        {
            errors.Add("Email address is invalid.");
        }

        if (string.IsNullOrWhiteSpace(value.ZipCode))
        {
            errors.Add("ZIP code is required.");
        }
        else if (!Regex.IsMatch(value.ZipCode, @"^\d{5}$"))
        {
            errors.Add("ZIP code must be 5 digits.");
        }

        return errors;
    }
}
