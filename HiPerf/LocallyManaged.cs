using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiPerf
{
    [AttributeUsage(AttributeTargets.Field)]
    public class LocallyManaged : System.Attribute
    {
    }
}
