using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xml;

namespace hlarm.specification
{
    public class instruction_encoding
    {
        public string                                   name                    { get; set; }   
        public string                                   encoding_string         { get; set; }
        public int                                      decoding_mask           { get; set; }
        public int                                      decoding_instruction    { get; set; }
        public string                                   decoding_pseudocode     { get; set; }
        public Dictionary<string, instruction_operand>  operands                { get; set; }
        public List<decoding_helper>                    helpers                 { get; set; }

        void load_encoding_data(ArmXmlFileInstructionsectionClassesIclassRegdiagram regdiagram)
        {
            if (regdiagram.Form != 32)
            {
                throw new Exception();
            }

            encoding_string = "";

            instruction_operand working_instruction_operand = null;

            foreach (ArmXmlFileInstructionsectionClassesIclassRegdiagramBox b in regdiagram.Box)
            {
                if (b.Name != null)
                {
                    working_instruction_operand = new instruction_operand();

                    working_instruction_operand.name = b.Name;
                    working_instruction_operand.offset = 32 - encoding_string.Length;

                    operands.Add(b.Name, working_instruction_operand);
                }

                foreach (ArmXmlFileInstructionsectionClassesIclassRegdiagramBoxC c in b.C)
                {
                    if (c.Colspan != 0)
                    {
                        if (b.C.Count != 1)
                        {
                            throw new Exception();
                        }

                        string c_value = c.Value;

                        if (c_value == null) c_value = "";
                        
                        //TODO: Are there more decoding conditions ?
                        if (c_value.StartsWith("!="))
                        {
                            string value = c_value.Split(' ')[1];

                            decoding_helper current_helper = new decoding_helper();

                            current_helper.type = encoding_helper_type.does_not_equal;
                            current_helper.encoding_offset = 32 - encoding_string.Length - value.Length;
                            current_helper.value = value;

                            helpers.Add(current_helper);
                        }
                        else if (c_value != "")
                        {
                            throw new Exception();
                        }

                        encoding_string += string_tools.create_repeating_line("-", c.Colspan);

                        working_instruction_operand.length = c.Colspan;
                        working_instruction_operand.offset -= c.Colspan;

                        working_instruction_operand = null;
                    }
                    else
                    {
                        string byte_info = c.Value;

                        byte_info = byte_info.Replace("(", "").Replace(")", "");

                        if (byte_info == "x")
                        {
                            encoding_string += "-";
                        }
                        else if (byte_info != "0" && byte_info != "1")
                        {
                            throw new Exception();
                        }
                        else
                        {
                            encoding_string += byte_info;
                        }
                    }
                }
            }

            for (int i = 0; i < 32; ++i)
            {
                int number_place = 31 - i;

                if (encoding_string[i] == '1')
                {
                    decoding_instruction |= 1 << number_place;
                }

                if (encoding_string[i] != '-')
                {
                    decoding_mask |= 1 << number_place; 
                }
            }

            if (encoding_string.Length != 32)
            {
                throw new Exception();
            }
        }

        public instruction_encoding(ArmXmlFileInstructionsectionClassesIclass i_class)
        {
            operands = new Dictionary<string, instruction_operand>();
            helpers = new List<decoding_helper>();

            name = i_class.Name;

            load_encoding_data(i_class.Regdiagram);

            if (i_class.PsSection != null)
            {
                decoding_pseudocode = i_class.PsSection.Ps.Pstext.Text[0];
            }
        }
    }
}
