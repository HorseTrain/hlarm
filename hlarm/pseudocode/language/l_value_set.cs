using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class l_value_set : language_object
    {
        public expression l_value   { get; set; }
        public expression r_value   { get; set; } 

        public l_value_set(expression l_value, expression r_value)
        {
            this.l_value = l_value;
            this.r_value = r_value; 
        }
    }
}
