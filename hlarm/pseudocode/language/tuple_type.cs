using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class tuple_type : expression_type
    {
        public List<expression_type> types  { get; set; }

        public tuple_type()
        {
            types = new List<expression_type>();    
        }
    }
}
