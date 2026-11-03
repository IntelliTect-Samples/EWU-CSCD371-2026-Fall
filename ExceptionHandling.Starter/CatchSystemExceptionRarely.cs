namespace ExceptionHandling.Starter;

public static class CatchSystemExceptionRarely
{
    public static void Run()
    {
        string exportPath = Path.Combine(Path.GetTempPath(), "orders-export.csv");
        string[] orders = ["1001,Widget,3", "1002,Gadget,2", "1003,Gizmo,two"];

        // TODO: If the export fails for ANY reason, delete the half-written file,
        //       then let the exception continue up the call stack.
        ExportOrders(exportPath, orders);
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
