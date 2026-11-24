using System;
using System.Collections.Generic;
using System.Text;

using ForceConnector.Core.Models;

namespace ForceConnector.Core.Interfaces;

/// <summary>
/// Parses CSV content into application models.
/// </summary>
public interface ICsvParser
{
    /// <summary>
    /// Parses a people CSV document.
    /// </summary>
    /// <param name="csv">CSV text.</param>
    /// <returns>
    /// Parsed collection of people.
    /// </returns>
    IReadOnlyList<Person> ParsePeople(string csv);

    CsvParseResult ParsePeopleWithRejections(string csv);
}