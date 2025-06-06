using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class instruction_operand
    {
        public string   operand_name    { get; set; }
        public int      operand_offset  { get; set; }
        public int      operand_length  { get; set; }
    }
}
