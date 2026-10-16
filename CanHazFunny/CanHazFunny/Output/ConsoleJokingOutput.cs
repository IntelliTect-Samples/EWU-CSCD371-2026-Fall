using System;
using System.Collections.Generic;
using System.Text;

namespace CanHazFunny.Output
{
    public sealed class ConsoleJokingOutput : IJokeOutputStringer
    {
        public void Write(string message) {
            Console.WriteLine(message);
        }
    }
}
