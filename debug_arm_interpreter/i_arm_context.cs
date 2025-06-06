using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace debug_arm_interpreter
{
    public unsafe interface i_arm_context
    {
        public void* get_physical_memory(ulong virtual_address);
        public void set_x(int index, ulong value);
        public ulong get_x(int index);
    }
}
