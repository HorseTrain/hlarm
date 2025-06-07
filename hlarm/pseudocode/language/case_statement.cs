using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class case_statement : language_object
    {
        public expression           test        { get; set; }
        public List<when_statement> statements  { get; set; }

        public case_statement() 
        {
            statements = new List<when_statement>();
        }
    }
}
