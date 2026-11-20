namespace Assignment8.ResilientImporter;


/// <summary>
/// An enum to Import data using one of two methods. Useful for data parsing without the throws crashing everything
/// </summary>
public enum ImportMode
{
    FailFast,
    SkipInvalid
}
