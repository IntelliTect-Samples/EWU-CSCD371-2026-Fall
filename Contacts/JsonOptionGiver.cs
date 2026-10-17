using System.Text.Json;

namespace Contacts;

public static class JsonOptionGiver
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        WriteIndented = true
    };
}
