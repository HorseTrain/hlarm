using hlarm.pseudocode.parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xml;

namespace hlarm.specification
{
    public class instruction_section
    {
        public string                       id          { get; set; }
        public string                       title       { get; set; }
        public instruction_section_type     type        { get; set; }  
        public List<instruction_encoding>   encodings   { get; set; }
        public string                       pseudocode  { get; set; }    

        public instruction_section(ArmXmlFileInstructionsection source)
        {
            encodings = new List<instruction_encoding>();

            parse_from_xml(source);
        }

        void parse_from_xml(ArmXmlFileInstructionsection source)
        {
            id = source.Id;
            title = source.Title;

            switch (source.Type)
            {
                case "alias":       type = instruction_section_type.alias;          break;
                case "instruction": type = instruction_section_type.instruction;    break;
                case "pseudocode":  type = instruction_section_type.pseudocode;     break;
                default: throw new Exception();
            }

            foreach (ArmXmlFileInstructionsectionClasses classes_singular in source.Classes)
            {
                foreach (ArmXmlFileInstructionsectionClassesIclass i_class in classes_singular.Iclass)
                {
                    instruction_encoding working_instruction_encoding = new instruction_encoding(i_class);

                    encodings.Add(working_instruction_encoding);    
                }
            }

            if (type != instruction_section_type.alias)
            {
                foreach (var section in source.PsSection)
                {
                    foreach (var ps in section.Ps)
                    {
                        pseudocode += ps.Pstext.Text[0] + "\n";
                    }
                }
            }

            if (pseudocode == null)
                return;

            source_loader.get_parse_tree(pseudocode);
        }
    }
}
