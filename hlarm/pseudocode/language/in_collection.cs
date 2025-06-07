using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class in_collection : expression
    {
        public expression       test    { get; set;  }
        public List<expression> values  { get; set; }

        public in_collection()
        {
            values = new List<expression>();
        }
    }
}
