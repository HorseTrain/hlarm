using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class constant_real : expression
    {
        public double   value   { get; set; }
        public constant_real(double value)
        {
            this.value = value; 
        }

        public override expression_type get_expression_type()
        {
            return (expression_type)thread_static_source_file_context.get_scoped_object("real");
        }
    }
}
