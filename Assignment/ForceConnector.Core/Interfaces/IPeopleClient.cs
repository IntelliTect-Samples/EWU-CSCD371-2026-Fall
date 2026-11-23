using System;
using System.Collections.Generic;
using System.Text;

namespace ForceConnector.Core.Interfaces;

/// <summary>
/// Retrieves the source CSV of people from the remote endpoint.
/// </summary>
public interface IPeopleClient
{
    /// <summary>
    /// Downloads the people CSV for a specific user.
    /// </summary>
    /// <param name="user">User identifier used by the endpoint.</param>
    /// <param name="cancellationToken">
    /// Token used to cancel the request.
    /// </param>
    /// <returns>
    /// The raw CSV returned by the API.
    /// </returns>
    Task<string> DownloadPeopleCsvAsync(
        string user,
        CancellationToken cancellationToken);
}
