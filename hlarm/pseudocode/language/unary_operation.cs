using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class unary_operation : expression
    {
        public string       operation   { get; set; }
        public expression   value       { get; set; }
    }
}
