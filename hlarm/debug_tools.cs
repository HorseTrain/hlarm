using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm
{
    public static class debug_tools
    {
        public static void assert(bool condition, string message = "")
        {
            if (!condition)
            {            
                throw new Exception(message);
            }
        }
    }
}
