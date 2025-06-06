using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class bit_fields_access : expression
    {
        public expression       base_expression { get; set; }
        public List<bit_range>  ranges          { get; set; }

        public bit_fields_access()
        {
            ranges = new List<bit_range>();
        }

        public override expression_type get_expression_type()
        {
            expression result = ranges[0].get_size();

            for (int i = 1; i < ranges.Count; ++i)
            {
                result = new binary_operation(result, "+", ranges[i].get_size());
            }

            return new bits_type(result);   
        }
    }
}
