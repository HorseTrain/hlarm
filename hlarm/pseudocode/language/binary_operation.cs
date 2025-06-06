using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class binary_operation : expression
    {
        public expression   first       { get; set; }
        public expression   second      { get; set; }  
        public string       operation   { get; set; }

        public binary_operation(expression first = null, string operation = "", expression second = null)
        {
            this.first = first;
            this.operation = operation;
            this.second = second;
        }
    }
}
