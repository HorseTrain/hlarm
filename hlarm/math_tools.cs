using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm
{
    public static class math_tools
    {
        public static int int_abs(int source)
        {
            if (source < 0)
                return -source;

            return source;
        }
    }
}
