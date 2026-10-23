namespace Contacts;

public static class Program
{
    public static void Main()
    {
        ContactBook contactBook = new();
        CLI cli = new(contactBook, Console.In, Console.Out);
        cli.Run();
    }
}
