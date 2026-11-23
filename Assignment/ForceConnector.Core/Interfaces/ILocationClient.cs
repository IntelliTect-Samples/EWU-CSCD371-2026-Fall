using System;
using System.Collections.Generic;
using System.Text;
using ForceConnector.Core.Models;
namespace ForceConnector.Core.Interfaces;

/// <summary>
/// Retrieves location information for ZIP codes.
/// </summary>
public interface ILocationClient
{
    /// <summary>
    /// Gets the city and state associated with a ZIP code.
    /// </summary>
    /// <param name="zipCode">ZIP code to search.</param>
    /// <param name="cancellationToken">
    /// Token used to cancel the request.
    /// </param>
    /// <returns>
    /// A valid location object.
    /// </returns>
    Task<Location> LookupAsync(
        string zipCode,
        CancellationToken cancellationToken);
}
