using System;
using System.Collections.Generic;
using System.Text;

namespace ForceConnector.Core.Models;

public sealed record Location
{
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
}
