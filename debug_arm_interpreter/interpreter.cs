using System.Numerics;

namespace debug_arm_interpreter
{
    public class arm_integer
    {
        public BigInteger   value   { get; set; }
        public int          size    { get; set; }

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

        public arm_integer(BigInteger value, int size = -1)
        {
            this.value = value;
            this.size = size;
        }

        public static implicit operator arm_integer(arm_bit_field source)
        {
            return new arm_integer(source.value, source.size);  
        }

        public static arm_integer operator == (arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value == (int)second.value ? 1 : 0);
        }

        public static arm_integer operator !=(arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value == (int)second.value ? 0 : 1);
        }

        public static implicit operator arm_integer(BigInteger source)
        {
            return new arm_integer(source);
        }

        public static arm_integer operator << (arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value << (int)second.value);
        } 

        public static arm_integer operator >>(arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value << (int)second.value);
        }

        public static arm_integer operator +(arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value + (int)second.value);
        }

        public static arm_integer operator -(arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value + (int)second.value);
        }

        public static arm_integer operator *(arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value * (int)second.value);
        }

        public static arm_integer operator /(arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value / (int)second.value);
        }

        public static arm_integer operator & (arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value & (int)second.value);
        }

        public static arm_integer operator | (arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value | (int)second.value);
        }

        public static arm_integer operator ^ (arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value ^ (int)second.value);
        }

        public static arm_integer test_and(arm_integer first, arm_integer second)
        {
            BigInteger _f = first.value != 0 ? 1 : 0;
            BigInteger _s = second.value != 0 ? 1 : 0;

            return new arm_integer(_f & _s);
        }
        public static arm_integer test_or(arm_integer first, arm_integer second)
        {
            BigInteger _f = first.value != 0 ? 1 : 0;
            BigInteger _s = second.value != 0 ? 1 : 0;

            return new arm_integer(_f | _s);
        }

        public static arm_integer concat(arm_integer first, arm_integer second)
        {
            if (first.size == -1 || second.size == -1)
            {
                throw new Exception();
            }

            BigInteger first_n = first.get_masked_value();
            BigInteger second_n = second.get_masked_value();

            BigInteger result = (first_n << second.size) | second_n;

            return new arm_integer(result, first.size + second.size);
        }

        public arm_integer bits(int top, int bottom)
        {
            top++;

            int size = top - bottom;

            BigInteger mask = ((BigInteger)1 << size) - 1;

            return new arm_integer((value >> bottom) & mask, size);
        }

        public arm_integer bits(params arm_integer[] parts)
        {
            if (parts.Count() % 2 != 0)
            {
                throw new Exception();
            }

            arm_integer result = new arm_integer(0, 0);

            for (int i = 0; i < parts.Length; i += 2)
            {
                result = concat(result, bits((int)parts[i].value, (int)parts[i + 1].value));
            }

            return result;
        }

        public static arm_integer get_size(params arm_integer[] values)
        {
            int working_size = values[0].size;

            for (int i = 1; i < values.Length; ++i)
            {
                if (values[i].size != working_size)
                {
                    throw new Exception();
                }
            }

            return new arm_integer(working_size);
        }

        public static void set_bits(ref arm_integer source, arm_integer[] parts, arm_integer new_value)
        {

        }
    }

    public class arm_bit_field
    {
        public BigInteger   mask    { get; set; }
        public BigInteger   value   { get; set; }
        public int          size    { get; set; }  

        public arm_bit_field(BigInteger mask, BigInteger value, int size)
        {
            this.mask = mask;
            this.value = value;
            this.size = size;
        }

        public static arm_integer operator == (arm_integer first,  arm_bit_field second)
        {
           return new arm_integer((first.value & second.mask) == second.value ? 1 : 0);  
        }

        public static arm_integer operator !=(arm_integer first, arm_bit_field second)
        {
            return new arm_integer((first.value & second.mask) == second.value ? 0 : 1);
        }

        public static arm_integer operator ==(arm_bit_field second, arm_integer first)
        {
            return new arm_integer((first.value & second.mask) == second.value ? 1 : 0);
        }

        public static arm_integer operator !=(arm_bit_field second, arm_integer first)
        {
            return new arm_integer((first.value & second.mask) == second.value ? 0 : 1);
        }
    }

    public class interpreter
    {
        arm_integer Decode_UNDEF { get; set; }
        (arm_integer,arm_integer) AddWithCarry(arm_integer x,arm_integer y,arm_integer carry_in)
        {
            arm_integer N = arm_integer.get_size(x,y);
            arm_integer unsigned_sum = ((UInt(x) + UInt(y)) + UInt(carry_in));
            arm_integer signed_sum = ((SInt(x) + SInt(y)) + UInt(carry_in));
            arm_integer result = unsigned_sum.bits((N - new arm_integer(1)), new arm_integer(0));
            arm_integer n = result.bits((N - new arm_integer(1)), (N - new arm_integer(1)));
            arm_integer z = IsZero(result).value != 0 ? new arm_bit_field(1, 1, 1) : new arm_bit_field(1, 0, 1);
            arm_integer c = (UInt(result) == unsigned_sum).value != 0 ? new arm_bit_field(1, 0, 1) : new arm_bit_field(1, 1, 1);
            arm_integer v = (SInt(result) == signed_sum).value != 0 ? new arm_bit_field(1, 0, 1) : new arm_bit_field(1, 1, 1);
            return (result,arm_integer.concat(arm_integer.concat(arm_integer.concat(n, z), c), v));
        }
        protected virtual arm_integer UInt(arm_integer parameter_0) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_integer SInt(arm_integer parameter_0) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_integer IsZero(arm_integer parameter_0) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_integer Zeros(arm_integer parameter_0) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_integer SP(arm_integer parameter_0) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_integer X(arm_integer parameter_0,arm_integer parameter_1) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_integer ZeroExtend(arm_integer parameter_0,arm_integer parameter_1) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_integer SP(arm_integer parameter_0,arm_integer parameter_1) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_integer X(arm_integer parameter_0,arm_integer parameter_1,arm_integer parameter_2) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_integer EndOfDecode(arm_integer parameter_0) 
        { 
            throw new NotImplementedException(); 
        }
        void instruction_11000000_7f800000(arm_integer sf,arm_integer op,arm_integer S,arm_integer sh,arm_integer imm12,arm_integer Rn,arm_integer Rd)
        {
            arm_integer d = UInt(Rd);
            arm_integer n = UInt(Rn);
            arm_integer datasize = (new arm_integer(32) << UInt(sf));
            arm_integer imm = (sh == new arm_bit_field(1, 0, 1)).value != 0 ? arm_integer.concat(Zeros(new arm_integer(12)), imm12) : arm_integer.concat(imm12, Zeros(new arm_integer(12)));
            arm_integer operand1 = (n == new arm_integer(31)).value != 0 ? SP(datasize) : X(n,datasize);
            arm_integer operand2 = ZeroExtend(imm,datasize);
            arm_integer result;
            (result,_) = AddWithCarry(operand1,operand2,new arm_bit_field(1, 0, 1));
            if ((d == new arm_integer(31)).value != 0)
            {
                SP(new arm_integer(64),ZeroExtend(result,new arm_integer(64)));
            }
            else
            {
                X(d,datasize,result);
            }
        }
        void instruction_72800000_7f800000(arm_integer sf,arm_integer opc,arm_integer hw,arm_integer imm16,arm_integer Rd)
        {
            if (arm_integer.test_and((sf == new arm_bit_field(1, 0, 1)), (hw.bits(new arm_integer(1), new arm_integer(1)) == new arm_bit_field(1, 1, 1))).value != 0)
            {
                EndOfDecode(Decode_UNDEF);
            }
            arm_integer d = UInt(Rd);
            arm_integer datasize = (new arm_integer(32) << UInt(sf));
            arm_integer imm = imm16;
            arm_integer pos = (UInt(hw) << new arm_integer(4));
            arm_integer result = X(d,datasize);
            arm_integer.set_bits(ref result, [(pos + new arm_integer(15)), pos], imm);
            X(d,datasize,result);
        }
        

    }
}
