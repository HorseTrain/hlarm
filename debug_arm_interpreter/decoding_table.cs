using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace debug_arm_interpreter
{
    public delegate void decode_and_execute(interpreter interpreter_context, int instruction);

    public class decoding_table
    {
        public int                  instruction                 { get; set; }
        public int                  mask                        { get; set; }
        public decode_and_execute   decode_and_execute_context  { get; set; }

        public string instruction_string    => $"{instruction:b32}";
        public string mask_string           => $"{mask:b32}";

        public decoding_table(int instruction, int mask, decode_and_execute decode_and_execute_context)
        {
            this.instruction = instruction;
            this.mask = mask;
            this.decode_and_execute_context = decode_and_execute_context;
        }

        public bool test(int raw_instruction)
        {
            return (raw_instruction & mask) == instruction;
        }
    }
}
