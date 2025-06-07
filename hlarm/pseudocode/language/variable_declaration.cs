using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class variable_declaration : language_object
    {
        public expression_type  variable_type           { get; set; } 
        public string           name                    { get; set; }
        public expression       default_value           { get; set; }

        public bool             is_function_parameter   { get; set; }
        public bool             undefined_global        { get; set; }

        public bool             is_referable           { get; set; } 
    }
}
