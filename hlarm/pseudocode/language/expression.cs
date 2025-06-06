using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class expression : language_object
    {
        public virtual expression_type get_expression_type()
        {
            throw new Exception(this.GetType().ToString());
        }
    }
}
