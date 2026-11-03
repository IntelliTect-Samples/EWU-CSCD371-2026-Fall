namespace ExceptionHandling.Reference;

public static class CatchSystemExceptionRarely
{
    public static void Run()
    {
        string exportPath = Path.Combine(Path.GetTempPath(), "orders-export.csv");
        string[] orders = ["1001,Widget,3", "1002,Gadget,2", "1003,Gizmo,two"];

        try
        {
            ExportOrders(exportPath, orders);
        }
        catch
        {
            // We don't know - or need to know - WHAT failed. Whatever it was,
            // a half-written export must not be left behind. Clean up, then rethrow.
            File.Delete(exportPath);
            Console.WriteLine($"  Partial export removed? {!File.Exists(exportPath)}");
            throw;
        }

        // The other legitimate catch (Exception) lives at the very top of the call stack:
        // see DemoRunner.RunDemo in ExceptionHandling.Shared.
    }

    private static void ExportOrders(string path, string[] orders)
    {
        using StreamWriter writer = new(path);
        foreach (string order in orders)
        {
            string[] fields = order.Split(',');
            int quantity = int.Parse(fields[2]);
            writer.WriteLine($"{fields[0]},{fields[1]},{quantity}");
            Console.WriteLine($"  Exported order {fields[0]}");
        }
    }
}
