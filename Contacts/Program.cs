namespace Contacts;

public static class Program
{
    public static void Main()
    {
        new ContactCli(new ContactBook(), Console.In, Console.Out).Run();
    }
}
