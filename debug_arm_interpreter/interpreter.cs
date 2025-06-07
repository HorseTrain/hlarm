using System.Numerics;
using System.Diagnostics;

namespace debug_arm_interpreter
{
    public class interpreter
    {
        List<decoding_table> decoding_tables;
        
        public interpreter()
        {
            decoding_tables = [
                new decoding_table(285212672, 2139095040, execute_instruction_11000000_7f800000),
                new decoding_table(1920991232, 2139095040, execute_instruction_72800000_7f800000),
                new decoding_table(310378496, 2139095040, execute_instruction_12800000_7f800000),
                new decoding_table(1384120320, 2139095040, execute_instruction_52800000_7f800000),
                new decoding_table(1358954496, 2139095040, execute_instruction_51000000_7f800000)
        	];
        }
        static void execute_instruction_11000000_7f800000(interpreter interpreter_context, int instruction)
        {
        	arm_number sf = new arm_number((instruction >> 31) & 1, 1);
        	arm_number op = new arm_number((instruction >> 31) & 1, 1);
        	arm_number S = new arm_number((instruction >> 30) & 1, 1);
        	arm_number sh = new arm_number((instruction >> 22) & 1, 1);
        	arm_number imm12 = new arm_number((instruction >> 10) & 4095, 12);
        	arm_number Rn = new arm_number((instruction >> 5) & 31, 5);
        	arm_number Rd = new arm_number((instruction >> 0) & 31, 5);
        	interpreter_context.instruction_11000000_7f800000(sf,op,S,sh,imm12,Rn,Rd);
        }
        static void execute_instruction_72800000_7f800000(interpreter interpreter_context, int instruction)
        {
        	arm_number sf = new arm_number((instruction >> 31) & 1, 1);
        	arm_number opc = new arm_number((instruction >> 31) & 3, 2);
        	arm_number hw = new arm_number((instruction >> 21) & 3, 2);
        	arm_number imm16 = new arm_number((instruction >> 5) & 65535, 16);
        	arm_number Rd = new arm_number((instruction >> 0) & 31, 5);
        	interpreter_context.instruction_72800000_7f800000(sf,opc,hw,imm16,Rd);
        }
        static void execute_instruction_12800000_7f800000(interpreter interpreter_context, int instruction)
        {
        	arm_number sf = new arm_number((instruction >> 31) & 1, 1);
        	arm_number opc = new arm_number((instruction >> 31) & 3, 2);
        	arm_number hw = new arm_number((instruction >> 21) & 3, 2);
        	arm_number imm16 = new arm_number((instruction >> 5) & 65535, 16);
        	arm_number Rd = new arm_number((instruction >> 0) & 31, 5);
        	interpreter_context.instruction_12800000_7f800000(sf,opc,hw,imm16,Rd);
        }
        static void execute_instruction_52800000_7f800000(interpreter interpreter_context, int instruction)
        {
        	arm_number sf = new arm_number((instruction >> 31) & 1, 1);
        	arm_number opc = new arm_number((instruction >> 31) & 3, 2);
        	arm_number hw = new arm_number((instruction >> 21) & 3, 2);
        	arm_number imm16 = new arm_number((instruction >> 5) & 65535, 16);
        	arm_number Rd = new arm_number((instruction >> 0) & 31, 5);
        	interpreter_context.instruction_52800000_7f800000(sf,opc,hw,imm16,Rd);
        }
        static void execute_instruction_51000000_7f800000(interpreter interpreter_context, int instruction)
        {
        	arm_number sf = new arm_number((instruction >> 31) & 1, 1);
        	arm_number op = new arm_number((instruction >> 31) & 1, 1);
        	arm_number S = new arm_number((instruction >> 30) & 1, 1);
        	arm_number sh = new arm_number((instruction >> 22) & 1, 1);
        	arm_number imm12 = new arm_number((instruction >> 10) & 4095, 12);
        	arm_number Rn = new arm_number((instruction >> 5) & 31, 5);
        	arm_number Rd = new arm_number((instruction >> 0) & 31, 5);
        	interpreter_context.instruction_51000000_7f800000(sf,op,S,sh,imm12,Rn,Rd);
        }
        

        arm_number UInt(arm_number x)
        {
            arm_number N = arm_number.get_size(x);
            arm_number result = new arm_number(0);
            for (arm_number i = new arm_number(0); ((i != ((N - new arm_number(1)) + new arm_number(1)))).value != 0; i ++)
            {
                if ((x.bits(i, i) == new arm_bit_field(1, 1, 1)).value != 0)
                {
                    result = (result + arm_number.pow(new arm_number(2), i));
                }
            };
            return result;
        }
        arm_number Zeros(arm_number N)
        {
            return Replicate(new arm_bit_field(1, 0, 1),N);
        }
        arm_number SInt(arm_number x)
        {
            arm_number N = arm_number.get_size(x);
            arm_number result = new arm_number(0);
            for (arm_number i = new arm_number(0); ((i != ((N - new arm_number(1)) + new arm_number(1)))).value != 0; i ++)
            {
                if ((x.bits(i, i) == new arm_bit_field(1, 1, 1)).value != 0)
                {
                    result = (result + arm_number.pow(new arm_number(2), i));
                }
            };
            if ((x.bits((N - new arm_number(1)), (N - new arm_number(1))) == new arm_bit_field(1, 1, 1)).value != 0)
            {
                result = (result - arm_number.pow(new arm_number(2), N));
            }
            return result;
        }
        arm_number ZeroExtend(arm_number x,arm_number N)
        {
            arm_number M = arm_number.get_size(x);
            Debug.Assert((N >= M).value == 1);
            return arm_number.concat(Zeros((N - M)), x);
        }
        (arm_number,arm_number) AddWithCarry(arm_number x,arm_number y,arm_number carry_in)
        {
            arm_number N = arm_number.get_size(x);
            arm_number unsigned_sum = ((UInt(x) + UInt(y)) + UInt(carry_in));
            arm_number signed_sum = ((SInt(x) + SInt(y)) + UInt(carry_in));
            arm_number result = unsigned_sum.bits((N - new arm_number(1)), new arm_number(0));
            arm_number n = result.bits((N - new arm_number(1)), (N - new arm_number(1)));
            arm_number z = IsZero(result).value != 0 ? new arm_bit_field(1, 1, 1) : new arm_bit_field(1, 0, 1);
            arm_number c = (UInt(result) == unsigned_sum).value != 0 ? new arm_bit_field(1, 0, 1) : new arm_bit_field(1, 1, 1);
            arm_number v = (SInt(result) == signed_sum).value != 0 ? new arm_bit_field(1, 0, 1) : new arm_bit_field(1, 1, 1);
            return (result,arm_number.concat(arm_number.concat(arm_number.concat(n, z), c), v));
        }
        void X(arm_number n,arm_number width,arm_number value)
        {
            Debug.Assert(arm_number.test_and((n >= new arm_number(0)), (n <= new arm_number(31))).value == 1);
            Debug.Assert(arm_number.test_in(width,new arm_number(32),new arm_number(64)).value == 1);
            if ((n != new arm_number(31)).value != 0)
            {
                _R(n,ZeroExtend(value,new arm_number(64)));
            }
            return;
        }
        arm_number X(arm_number n,arm_number width)
        {
            Debug.Assert(arm_number.test_and((n >= new arm_number(0)), (n <= new arm_number(31))).value == 1);
            Debug.Assert(arm_number.test_in(width,new arm_number(8),new arm_number(16),new arm_number(32),new arm_number(64)).value == 1);
            arm_number rw = width;
            if ((n != new arm_number(31)).value != 0)
            {
                return _R(n).bits((rw - new arm_number(1)), new arm_number(0));
            }
            else
            {
                return Zeros(rw);
            }
        }
        arm_number IsZero(arm_number x)
        {
            arm_number N = arm_number.get_size(x);
            return (x == Zeros(N));
        }
        protected virtual arm_number Replicate(arm_number parameter_0,arm_number parameter_1) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_number _R(arm_number parameter_0,arm_number parameter_1) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_number _R(arm_number parameter_0) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_number SP(arm_number parameter_0) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_number SP(arm_number parameter_0,arm_number parameter_1) 
        { 
            throw new NotImplementedException(); 
        }
        protected virtual arm_number EndOfDecode(arm_number parameter_0) 
        { 
            throw new NotImplementedException(); 
        }
        void instruction_11000000_7f800000(arm_number sf,arm_number op,arm_number S,arm_number sh,arm_number imm12,arm_number Rn,arm_number Rd)
        {
            arm_number d = UInt(Rd);
            arm_number n = UInt(Rn);
            arm_number datasize = (new arm_number(32) << UInt(sf));
            arm_number imm = (sh == new arm_bit_field(1, 0, 1)).value != 0 ? arm_number.concat(Zeros(new arm_number(12)), imm12) : arm_number.concat(imm12, Zeros(new arm_number(12)));
            arm_number operand1 = (n == new arm_number(31)).value != 0 ? SP(datasize) : X(n,datasize);
            arm_number operand2 = ZeroExtend(imm,datasize);
            arm_number result = new arm_number();
            (result,_) = AddWithCarry(operand1,operand2,new arm_bit_field(1, 0, 1));
            if ((d == new arm_number(31)).value != 0)
            {
                SP(new arm_number(64),ZeroExtend(result,new arm_number(64)));
            }
            else
            {
                X(d,datasize,result);
            }
        }
        void instruction_72800000_7f800000(arm_number sf,arm_number opc,arm_number hw,arm_number imm16,arm_number Rd)
        {
            if (arm_number.test_and((sf == new arm_bit_field(1, 0, 1)), (hw.bits(new arm_number(1), new arm_number(1)) == new arm_bit_field(1, 1, 1))).value != 0)
            {
                EndOfDecode(new arm_number(0));
            }
            arm_number d = UInt(Rd);
            arm_number datasize = (new arm_number(32) << UInt(sf));
            arm_number imm = imm16;
            arm_number pos = (UInt(hw) << new arm_number(4));
            arm_number result = X(d,datasize);
            arm_number.set_bits(ref result, [(pos + new arm_number(15)), pos], imm);
            X(d,datasize,result);
        }
        void instruction_12800000_7f800000(arm_number sf,arm_number opc,arm_number hw,arm_number imm16,arm_number Rd)
        {
            if (arm_number.test_and((sf == new arm_bit_field(1, 0, 1)), (hw.bits(new arm_number(1), new arm_number(1)) == new arm_bit_field(1, 1, 1))).value != 0)
            {
                EndOfDecode(new arm_number(0));
            }
            arm_number d = UInt(Rd);
            arm_number datasize = (new arm_number(32) << UInt(sf));
            arm_number imm = imm16;
            arm_number pos = (UInt(hw) << new arm_number(4));
            arm_number result = Zeros(datasize);
            arm_number.set_bits(ref result, [(pos + new arm_number(15)), pos], imm);
            X(d,datasize,~((result)));
        }
        void instruction_52800000_7f800000(arm_number sf,arm_number opc,arm_number hw,arm_number imm16,arm_number Rd)
        {
            if (arm_number.test_and((sf == new arm_bit_field(1, 0, 1)), (hw.bits(new arm_number(1), new arm_number(1)) == new arm_bit_field(1, 1, 1))).value != 0)
            {
                EndOfDecode(new arm_number(0));
            }
            arm_number d = UInt(Rd);
            arm_number datasize = (new arm_number(32) << UInt(sf));
            arm_number imm = imm16;
            arm_number pos = (UInt(hw) << new arm_number(4));
            arm_number result = Zeros(datasize);
            arm_number.set_bits(ref result, [(pos + new arm_number(15)), pos], imm);
            X(d,datasize,result);
        }
        void instruction_51000000_7f800000(arm_number sf,arm_number op,arm_number S,arm_number sh,arm_number imm12,arm_number Rn,arm_number Rd)
        {
            arm_number d = UInt(Rd);
            arm_number n = UInt(Rn);
            arm_number datasize = (new arm_number(32) << UInt(sf));
            arm_number imm = (sh == new arm_bit_field(1, 0, 1)).value != 0 ? arm_number.concat(Zeros(new arm_number(12)), imm12) : arm_number.concat(imm12, Zeros(new arm_number(12)));
            arm_number operand1 = (n == new arm_number(31)).value != 0 ? SP(datasize) : X(n,datasize);
            arm_number operand2 = ZeroExtend(imm,datasize);
            arm_number result = new arm_number();
            (result,_) = AddWithCarry(operand1,~((operand2)),new arm_bit_field(1, 1, 1));
            if ((d == new arm_number(31)).value != 0)
            {
                SP(new arm_number(64),ZeroExtend(result,new arm_number(64)));
            }
            else
            {
                X(d,datasize,result);
            }
        }
        

        public void execute_instruction(int instruction)
        {
            for (int i = 0; i < decoding_tables.Count; ++i)
            {
                decoding_table test_table = decoding_tables[i];

                if (test_table.test(instruction))
                {
                    test_table.decode_and_execute_context(this, instruction);

                    return;
                }
            }

            throw new Exception();
        }

        public void execute_instruction_big(int instruction)
        {
            int result = 0;

            for (int i = 0; i < 4; ++i)
            {
                int part = (instruction >> (i * 8)) & 255;

                result |= (part << ((3 - i) * 8));
            }

            execute_instruction(result);
        }

        public void execute_instruction_big(uint instruction)
        {
            execute_instruction_big((int)instruction);
        }
    }
}
