using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class for_loop : scope
    {
        public bool                     goes_up             { get; set; }
        public language_object          start               { get; set; }
        public expression               end                 { get; set; } 
        public declaration_reference    start_reference     { get; set; }

        public for_loop(scope parent_scope) : base(parent_scope)
        {

        }
    }
}
