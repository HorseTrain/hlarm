using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace debug_arm_interpreter
{
    public class test_context : interpreter
    {
        public ulong[]              gp_registers;
        public Vector128<ulong>[]   vector_registers;

        public test_context()
        {
            gp_registers = new ulong[32];
            vector_registers = new Vector128<ulong>[32];
        }

        protected override arm_number _R(arm_number parameter_0)
        {
            int index = (int)parameter_0.value;

            return new arm_number(gp_registers[index], 64);
        }

        protected override arm_number _R(arm_number parameter_0, arm_number parameter_1)
        {
            int index = (int)parameter_0.value;

            gp_registers[index] = (ulong)(parameter_1.value & ulong.MaxValue); 

            return null;
        }

        protected override arm_number SP(arm_number parameter_0)
        {
            ulong working_result = gp_registers[31];

            return new arm_number(working_result, (int)parameter_0.value);
        }

        protected override arm_number SP(arm_number parameter_0, arm_number parameter_1)
        {
            gp_registers[31] = (ulong)(parameter_1.value & ((1UL << (int)parameter_0.value) - 1));

            return null;
        }

        protected override arm_number EndOfDecode(arm_number parameter_0)
        {
            throw new Exception();

            return null;
        }

        protected override arm_number Replicate(arm_number parameter_0, arm_number parameter_1)
        {
            arm_number working_result = parameter_0;

            for (int i = 1; i < parameter_1.value; i++)
            {
                working_result = arm_number.concat(working_result, parameter_0);
            }

            return working_result;
        }
    }
}
