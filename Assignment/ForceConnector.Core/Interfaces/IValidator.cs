using System;
using System.Collections.Generic;
using System.Text;

namespace ForceConnector.Core.Interfaces;

/// <summary>
/// Defines validation logic for a type.
/// </summary>
/// <typeparam name="T">
/// Type being validated.
/// </typeparam>
public interface IValidator<T>
{
    /// <summary>
    /// Determines whether the value is valid.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <returns>
    /// True if valid; otherwise false.
    /// </returns>
    bool IsValid(T value);

    /// <summary>
    /// Returns validation errors for a value.
    /// </summary>
    /// <param name="value">
    /// Value to validate.
    /// </param>
    /// <returns>
    /// Collection of validation messages.
    /// </returns>
    IReadOnlyList<string> GetErrors(T value);
}
