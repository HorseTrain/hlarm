using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class when_statement : language_object
    {
        public List<expression> passes  { get; set; }
        public language_object  code    { get; set; }

        public when_statement()
        {
            passes = new List<expression>();
        }
    }
}
