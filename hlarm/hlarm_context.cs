using hlarm.pseudocode.language;
using hlarm.pseudocode.parser;
using hlarm.pseudocode.pre_language;
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
        public source_file                              asl                     { get; set; }
        public Dictionary<string, instruction_section>  instruction_sections    { get; set; }

        public hlarm_context()
        {
            instruction_sections = new Dictionary<string, instruction_section>();
        }

        public void create_asl()
        {
            string dump = create_dump();

            pre_language_visitor visitor = new pre_language_visitor();

            pre_language_object pre_language_source = visitor.Visit(source_loader.get_parse_tree(dump));

            asl = new source_file(pre_language_source);
        }

        public void append_asl(string code)
        {
            pre_language_visitor visitor = new pre_language_visitor();

            pre_language_object pre_language_source = visitor.Visit(source_loader.get_parse_tree(code));

            asl.load_pre_language_object(pre_language_source);  
        }

        public void build_asl()
        {
            asl.build();
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

        public string create_dump()
        {
            StringBuilder library_code = new StringBuilder();
            StringBuilder instruction_code = new StringBuilder();

            foreach (var v in instruction_sections.Values)
            {
                if (v.type == instruction_section_type.pseudocode)
                {
                    library_code.Append(v.create_dump());   
                }
                else if (v.type == instruction_section_type.instruction)
                {
                    instruction_code.Append(v.create_dump());
                }
            }

            return library_code.ToString() + instruction_code.ToString();   
        }
    }
}
