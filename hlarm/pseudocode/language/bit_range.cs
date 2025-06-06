using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class bit_range : language_object
    {
        public expression top_bit       { get; set; }
        public expression bottom_bit    { get; set; }  

        public bit_range(expression top_bit, expression bottom_bit)
        {
            this.top_bit = top_bit;
            this.bottom_bit = bottom_bit;
        }

        public expression get_size()
        {
            expression top_bit_plus_one = new binary_operation(top_bit, "+", new constant(1));

            return new binary_operation(top_bit_plus_one, "-", bottom_bit);
        }
    }
}
