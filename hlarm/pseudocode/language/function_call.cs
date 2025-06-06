using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class function_call : expression
    {
        public string               function_key        { get; set; }
        public function_declaration function_reference  { get; set; }
        public List<expression>     arguments           { get; set; }

        public function_call()
        {
            arguments = new List<expression>();
        }

        public override expression_type get_expression_type()
        {
            return function_reference.return_type;
        }
    }
}
