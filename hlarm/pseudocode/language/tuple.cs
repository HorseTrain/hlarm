using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class tuple : expression
    {
        public List<expression> data    { get; set; }

        public tuple() 
        {
            data = new List<expression>();
        }
    }
}
