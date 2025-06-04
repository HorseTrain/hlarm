using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.specification
{
    public class instruction_operand
    {
        public string   name    { get; set; }
        public int      offset  { get; set; }
        public int      length  { get; set; }

        public int      top_bit => offset + length;
    }
}
