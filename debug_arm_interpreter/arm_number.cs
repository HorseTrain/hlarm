using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace debug_arm_interpreter
{
    public class arm_number
    {
        public BigInteger   value   { get; set; }
        public int          size    { get; set; }

        public bool is_real()
        {
            return size == -2;
        }

        public BigInteger get_mask()
        {
            if (size == -1)
            {
                throw new Exception();
            }

            return ((BigInteger)1 << size) - 1;
        }

        public BigInteger get_masked_value()
        {
            return value & get_mask();
        }

        public arm_number()
        {
            size = -1;
            value = 0;
        }

        public arm_number(BigInteger value, int size = -1)
        {
            this.value = value;
            this.size = size;
        }

        static void assert_same_size(arm_number x, arm_number y)
        {
            if (x.size != y.size)
            {
                throw new Exception();
            }
        }

        public static implicit operator arm_number(arm_bit_field source)
        {
            return new arm_number(source.value, source.size);
        }

        public static arm_number operator ++(arm_number first)
        {
            return new arm_number(first.value + 1, first.size);
        }

        public static arm_number operator --(arm_number first)
        {
            return new arm_number(first.value - 1, first.size);
        }

        public static arm_number operator -(arm_number first)
        {
            return new arm_number(-first.value, first.size);
        }

        public static arm_number operator ~(arm_number first)
        {
            return new arm_number(~first.value & first.get_mask(), first.size);
        }

        //BINARY
        public static arm_number operator ==(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number((first.value == second.value) ? 1 : 0, 1);
        }

        public static arm_number operator !=(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number((first.value != second.value) ? 1 : 0, 1);
        }

        public static arm_number operator >(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number((first.value > second.value) ? 1 : 0, 1);
        }

        public static arm_number operator <(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number((first.value < second.value) ? 1 : 0, 1);
        }

        public static arm_number operator >=(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number((first.value >= second.value) ? 1 : 0, 1);
        }

        public static arm_number operator <=(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number((first.value <= second.value) ? 1 : 0, 1);
        }

        public static arm_number operator <<(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number(first.value << (int)second.value);
        }

        public static arm_number operator >>(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number(first.value >> (int)second.value);
        }

        public static arm_number operator +(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number(first.value + second.value);
        }

        public static arm_number operator -(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number(first.value - second.value);
        }

        public static arm_number operator *(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number(first.value * second.value);
        }

        public static arm_number operator /(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number(first.value / second.value);
        }

        public static arm_number operator &(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number(first.value & second.value);
        }

        public static arm_number operator |(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number(first.value | second.value);
        }

        public static arm_number operator ^(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number(first.value ^ second.value);
        }

        public static arm_number pow(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            return new arm_number(BigInteger.Pow(first.value, (int)second.value));
        }

        public static arm_number test_and(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            BigInteger _f = first.value != 0 ? 1 : 0;
            BigInteger _s = second.value != 0 ? 1 : 0;

            return new arm_number(_f & _s);
        }
        public static arm_number test_or(arm_number first, arm_number second)
        {
            assert_same_size(first, second);

            BigInteger _f = first.value != 0 ? 1 : 0;
            BigInteger _s = second.value != 0 ? 1 : 0;

            return new arm_number(_f | _s);
        }

        public static arm_number concat(arm_number first, arm_number second)
        {
            if (first.size == -1 || second.size == -1)
            {
                throw new Exception();
            }

            BigInteger first_n = first.get_masked_value();
            BigInteger second_n = second.get_masked_value();

            BigInteger result = (first_n << second.size) | second_n;

            return new arm_number(result, first.size + second.size);
        }

        //OTHERS
        public static implicit operator arm_number(BigInteger source)
        {
            return new arm_number(source);
        }

        public arm_number bits(int top, int bottom)
        {
            top++;

            int size = top - bottom;

            BigInteger mask = ((BigInteger)1 << size) - 1;

            return new arm_number((value >> bottom) & mask, size);
        }

        public arm_number bits(params arm_number[] parts)
        {
            if (parts.Count() % 2 != 0)
            {
                throw new Exception();
            }

            arm_number result = new arm_number(0, 0);

            for (int i = 0; i < parts.Length; i += 2)
            {
                result = concat(result, bits((int)parts[i].value, (int)parts[i + 1].value));
            }

            return result;
        }

        public static arm_number get_size(params arm_number[] values)
        {
            int working_size = values[0].size;

            for (int i = 1; i < values.Length; ++i)
            {
                if (values[i].size != working_size)
                {
                    throw new Exception();
                }
            }

            return new arm_number(working_size);
        }

        public static arm_number test_in(arm_number source, params arm_number[] collection)
        {
            for (int i = 0; i <collection.Length; ++i)
            {
                if (source.value == collection[i].value)
                {
                    return new arm_number(1, 1);
                }
            }

            return new arm_number(0, 1);
        }

        public static arm_number flip(arm_number source)
        {
            if (source.value == 0)
            {
                return new arm_number(1, 1);
            }

            return new arm_number(0, 0);
        }

        public static void set_bits(ref arm_number source, arm_number[] parts, arm_number new_value)
        {
            int top = (int)parts[0].value + 1;
            int bottom = (int)parts[1].value;

            int new_size = top - bottom;

            Debug.Assert(new_value.size == new_size);

            BigInteger mask = (((BigInteger)1 << new_size) - 1) << bottom;
            BigInteger inverse_mask = ~mask;

            source.value &= inverse_mask;
            source.value |= (new_value.value << bottom) & mask;

            if (parts.Length > 2)
            {
                throw new Exception();
            }
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
