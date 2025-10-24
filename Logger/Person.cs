using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger;
public abstract record Person : Entity
{
    public override string Name
    {
        get
        {
            return string.IsNullOrEmpty(FullName.MiddleName)
                ? $"{FullName.FirstName} {FullName.LastName}"
                : $"{FullName.FirstName} {FullName.MiddleName} {FullName.LastName}";
        }
    }

    public FullName FullName { get; init; }

}