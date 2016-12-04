using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGL.Core.Infrastructure {
    [AttributeUsage(AttributeTargets.Method)]
    internal class DirectStateAccessAttribute : Attribute {
    }
}
