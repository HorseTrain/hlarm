using hlarm.pseudocode.pre_language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class instruction_declaration : scope
    {
        public int                                  instruction_mask        { get; set; }
        public int                                  instruction_encoding    { get; set; }
        public List<instruction_operand>            operands                { get; set; }
        public List<instruction_encoding_helper>    encoding_helpers        { get; set; }

        public pre_language_object                  data_source             { get; set; }

        public instruction_declaration(scope parent_scope) : base(parent_scope)
        {
            operands = new List<instruction_operand>();
            encoding_helpers = new List<instruction_encoding_helper>();    
        }

        public void create_operand(string operand_name, int operand_offset, int operand_length)
        {
            instruction_operand result_operand = new instruction_operand();

            result_operand.operand_name = operand_name;
            result_operand.operand_offset = operand_offset; 
            result_operand.operand_length = operand_length;

            variable_declaration declaration = new variable_declaration();

            declaration.name = operand_name;
            declaration.default_value = null;
            declaration.variable_type = new bits_type(new constant(operand_length));

            insert_scoped_object(operand_name, declaration);

            operands.Add(result_operand);
        }
    }
}
