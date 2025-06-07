using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class while_loop : language_object
    {
        public expression       condition   { get; set; }
        public language_object  code        { get; set; }
    }
}
