using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace debug_arm_interpreter
{
    static class program
    {
        public static void Main(string[] args)
        {
            test_context test = new test_context();

            //sub w0, w0, 100
            test.execute_instruction_big(0x00900151);

            //add w1, w0, 300
            test.execute_instruction_big(0x01B00411);

            //movk x3, 20, lsl 48
            test.execute_instruction_big(0x8302E0F2);

            //movn w3, 200, lsl 16
            test.execute_instruction_big(0x0319A012);

            //movz x4, 500
            test.execute_instruction_big(0x843E80D2);

            for (int i = 0; i < 32; ++i)
            {
                Console.WriteLine($"reg {i:d3} {test.gp_registers[i]:x16}");    
            }
        }
    }
}
