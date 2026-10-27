Permission permissions = Permission.Read | Permission.Write;
Console.WriteLine($"{permissions.HasFlag(Permission.Read)}, {permissions == Permission.Read}");

[Flags]
public enum Permission
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4
}
