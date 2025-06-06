using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class binary_encoding_pattern : expression
    {
        public expression_type  type    { get; set; }
        public BigInteger       mask    { get; set; }
        public BigInteger       value   { get; set; }
        public int              length  { get; set; }

        public binary_encoding_pattern(string data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                int place = data.Length - i - 1;

                if (data[i] == '1')
                {
                    value |= ((BigInteger)1) << place;
                }
                
                if (data[i] != 'x')
                {
                    mask |= ((BigInteger)1) << place;   
                }
            }

            length = data.Length;
            type = new bits_type(new constant(data.Length));
        }

        public override expression_type get_expression_type()
        {
            return type;
        }

    }
}
