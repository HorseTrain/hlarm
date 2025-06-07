using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace debug_arm_interpreter
{
    public class arm_bit_field
    {
        public BigInteger   mask    { get; set; }
        public BigInteger   value   { get; set; }
        public int          size    { get; set; }

        public arm_bit_field(BigInteger mask, BigInteger value, int size)
        {
            this.mask = mask;
            this.value = value & (((BigInteger)1 << size) - 1);
            this.size = size;
        }

        public static arm_number operator ==(arm_number first, arm_bit_field second)
        {
            return new arm_number((first.value & second.mask) == second.value ? 1 : 0);
        }

        public static arm_number operator !=(arm_number first, arm_bit_field second)
        {
            return new arm_number((first.value & second.mask) == second.value ? 0 : 1);
        }

        public static arm_number operator ==(arm_bit_field second, arm_number first)
        {
            return new arm_number((first.value & second.mask) == second.value ? 1 : 0);
        }

        public static arm_number operator !=(arm_bit_field second, arm_number first)
        {
            return new arm_number((first.value & second.mask) == second.value ? 0 : 1);
        }

        public static arm_bit_field operator ~(arm_bit_field test)
        {
            return new arm_bit_field(test.mask, ~test.value, test.size);
        }
    }
}
