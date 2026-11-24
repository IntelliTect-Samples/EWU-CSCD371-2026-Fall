using System;
using System.Collections.Generic;
using System.Text;

namespace ForceConnector.Core.Models;

public sealed record Rejection 
{
    public string EmailAddress { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}
