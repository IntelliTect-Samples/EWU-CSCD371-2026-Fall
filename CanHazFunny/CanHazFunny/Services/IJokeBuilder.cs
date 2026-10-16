using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CanHazFunny.Services;
public interface IJokeBuilder
{
    Task<string> GetJokeAsync();
}
