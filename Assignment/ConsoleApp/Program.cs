using Assignment;
using System.Text.RegularExpressions;

Console.WriteLine("Sup");

PingProcess pingProcess = new PingProcess();

string[] hostNames = new string[] { "localhost", "localhost", "localhost", "localhost" };
PingResult result = await pingProcess.RunAsync(hostNames);

string[]? lines = result.StdOutput?.Split(Environment.NewLine);
int count = 0;
foreach(string s in lines!)
{
    if (s.Contains("Pinging"))//Returns 8 times??
        count++;
}

Console.WriteLine(count);