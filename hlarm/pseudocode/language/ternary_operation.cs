using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class ternary_operation : expression
    {
        public expression condition { get; set; }
        public expression yes       { get; set; }
        public expression no        { get; set; }  
    }
}
