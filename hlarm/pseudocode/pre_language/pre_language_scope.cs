using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.pre_language
{
    public class pre_language_scope
    {
        public int tab                          { get; set; }
        public List<pre_language_object> code   { get; set; }

        public pre_language_scope(int tab, List<pre_language_object> code)
        {
            this.tab = tab;
            this.code = code;
        }
    }
}
