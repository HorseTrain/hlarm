using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class constant : expression
    {
        public expression_type  type    { get; set; }
        public BigInteger       value   { get; set; }

        public constant(BigInteger value, expression_type type = null)
        {
            if (type == null)
            {
                type = (expression_type)thread_static_source_file_context.get_scoped_object("integer");
            }

            this.value = value;
            this.type = type;
        }

        public override expression_type get_expression_type()
        {
            return type;
        }
    }
}
