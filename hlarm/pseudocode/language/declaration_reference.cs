using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class declaration_reference : expression
    {
        public string           reference_name  { get; set; }
        public language_object  reference       { get; set; }

        public override expression_type get_expression_type()
        {
            return new dynamic_type();
        }
    }
}
