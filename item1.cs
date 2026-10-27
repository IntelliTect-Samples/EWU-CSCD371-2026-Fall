Console.WriteLine($"{(int)Status.Pending}, {(int)Status.Active}, {(int)Status.Complete}");

public enum Status
{
    Pending,
    Active = 3,
    Complete
}
