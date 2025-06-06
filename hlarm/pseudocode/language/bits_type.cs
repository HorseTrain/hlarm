using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    internal class bits_type : expression_type
    {
        public expression size  { get; set; }

        public bits_type(expression size)
        {
            this.size = size;
        }
    }
}
