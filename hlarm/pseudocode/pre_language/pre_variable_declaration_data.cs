using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.pre_language
{
    public class pre_variable_declaration_data
    {
        public pre_language_object  vairbale_type   { get; set;  }
        public List<string>         variable_names  { get; set; }
        public bool                 is_referable    { get; set; }
        public pre_language_object  default_value   { get; set; }  

        public bool has_default_value
        {
            get => default_value != null;
        }

        public pre_variable_declaration_data()
        {
            variable_names = new List<string>();
        }

        public void validate()
        {
            if (vairbale_type == null)
            {
                throw new Exception();
            }
        }
    }
}
