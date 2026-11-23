using System;
using System.Collections.Generic;
using System.Text;

using ForceConnector.Core.Models;

namespace ForceConnector.Core.Interfaces;

/// <summary>
/// Coordinates downloading, validating,
/// enriching, and reporting on people data.
/// </summary>
public interface IPeopleEnricher
{
    /// <summary>
    /// Performs the complete enrichment process.
    /// </summary>
    /// <param name="user">
    /// User identifier used for the source file.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <returns>
    /// Enrichment results including accepted
    /// people and rejection information.
    /// </returns>
    Task<EnrichmentResult> EnrichAsync(
        string user,
        CancellationToken cancellationToken);
}