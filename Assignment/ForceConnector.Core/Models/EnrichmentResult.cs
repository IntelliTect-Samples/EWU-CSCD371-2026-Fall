using System;
using System.Collections.Generic;
using System.Text;

namespace ForceConnector.Core.Models;

public sealed record EnrichmentResult
{
    public IReadOnlyList<EnrichedPerson> People { get; init; }
    = [];
    public IReadOnlyList<Rejection> Rejections { get; init; }
    = [];
}
