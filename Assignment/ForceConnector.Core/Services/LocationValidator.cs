using ForceConnector.Core.Interfaces;
using ForceConnector.Core.Models;
using System.Text.RegularExpressions;

namespace ForceConnector.Core.Validation;

public sealed class LocationValidator : IValidator<Location>
{
    public bool IsValid(Location value)
    {
        return GetErrors(value).Count == 0;
    }

    public IReadOnlyList<string> GetErrors(Location value)
    {
        List<string> errors = [];

        if (string.IsNullOrWhiteSpace(value.City))
        {
            errors.Add("City is required.");
        }

        if (string.IsNullOrWhiteSpace(value.State))
        {
            errors.Add("State is required.");
        }
        else if (!Regex.IsMatch(value.State, @"^[A-Z]{2}$"))
        {
            errors.Add(
                "State must be a two-letter abbreviation.");
        }

        return errors;
    }
}