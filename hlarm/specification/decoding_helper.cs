using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.specification
{
    public class decoding_helper
    {
        public encoding_helper_type type            { get; set; }
        public int                  encoding_offset { get; set; }
        public string               value           { get; set; }
        public int                  length          => value.Length;
    }
}
