using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class instruction_encoding_helper
    {
        public int                              offset  { get; set; }
        public int                              length  { get; set; }
        public int                              value   { get; set; }
        public instruction_encoding_helper_type type    { get; set; }
    }
}
