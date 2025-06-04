using hlarm.specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xml;

namespace hlarm
{
    public class hlarm_context
    {
        public Dictionary<string, instruction_section> instruction_sections     { get; set; }

        public hlarm_context()
        {
            instruction_sections = new Dictionary<string, instruction_section>();
        }

        public void append_data_to_context(ArmXmlFile xml_file)
        {
            if (xml_file.Instructionsection.Count == 1)
            {
                instruction_section working_section = new instruction_section(xml_file.Instructionsection[0]);

                instruction_sections.Add(working_section.id, working_section);  
            }
            else
            {
                Console.WriteLine("Unknown XML File");
            }
        }
    }
}
