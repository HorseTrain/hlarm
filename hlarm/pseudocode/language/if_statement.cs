using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class if_statement : expression
    {
        public expression       condition           { get; set; }
        public language_object  pass                { get; set; }
        public language_object  fails               { get; set; }
        public bool             has_else_statement  => fails != null;
    }
}
