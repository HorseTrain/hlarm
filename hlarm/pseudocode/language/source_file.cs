using Antlr4.Runtime.Tree;
using hlarm.pseudocode.pre_language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class source_file : scope
    {
        public List<instruction_declaration>    instructions        { get; set; }
        public List<function_declaration>       functions           { get; set; }
        public List<variable_declaration>       global_variables    { get; set; }

        void create_basic_type(string name)
        {
            basic_type result = new basic_type();

            result.name = name;

            insert_scoped_object(name, result);
        }

        void init_basic_types()
        {
            create_basic_type("boolean");
            create_basic_type("integer");
            create_basic_type("bit");
            create_basic_type("real");
            create_basic_type("void");
        }

        public source_file(pre_language_object source) : base (null)
        {
            instructions = new List<instruction_declaration>();
            functions = new List<function_declaration>();
            global_variables = new List<variable_declaration>();

            setup_source_file(this);

            init_basic_types();

            load_pre_language_object(source);
        }

        public void load_pre_language_object(pre_language_object source)
        {
            visit_source_file(source);
        }

        public void build()
        {
            foreach (function_declaration declaration in functions)
            {
                declaration.pre_build_function_data();
            }

            List<function_declaration> functions_temp = functions.ToList();

            foreach (function_declaration declaration in functions_temp)
            {
                declaration.evaluate_commands(declaration.data_source);
            }

            foreach (instruction_declaration declaration in instructions)
            {
                declaration.evaluate_commands(declaration.data_source);
            }
        }

        void visit_source_file(pre_language_object source)
        {
            debug_tools.assert(source.type == pre_language_type.source_file);

            foreach (pre_language_object top_object in (List<pre_language_object>)source.data)
            {
                visit_top(top_object);
            }
        }

        void visit_top(pre_language_object source)
        {
            switch (source.type)
            {
                case pre_language_type.instruction_declaration:
                    {
                        instruction_declaration instruction = visit_instruction_declaration(source);

                        instructions.Add(instruction);
                    }
                    ; break;

                case pre_language_type.function_declaration:
                    {
                        function_declaration function = visit_function_declaration(source);

                        functions.Add(function);
                    }
                    ; break;

                case pre_language_type.enumeration_declaration:
                    {
                        List<string> names = (List<string>)source.data;

                        int i = 0;

                        foreach (string name in names)
                        {
                            insert_scoped_object(name, new constant(i));

                            ++i;
                        }

                    }; break;
                default: throw new Exception();
            }
        }

        function_declaration visit_function_declaration(pre_language_object source)
        {
            List<object> data = (List<object>)source.data;

            pre_language_object name_source = (pre_language_object)data[1];
            function_declaration result = new function_declaration(this);

            result.name = name_from_collection(name_source);
            result.data_source = source;

            return result;
        }

        instruction_declaration visit_instruction_declaration(pre_language_object source)
        {
            instruction_declaration result = new instruction_declaration(this);

            List<object> data = source.data as List<object>;

            result.instruction_encoding = (int)(uint)big_integer_from_pre_language_object(data[0]);
            result.instruction_mask = (int)(uint)big_integer_from_pre_language_object(data[1]);

            List<pre_language_object> operands = (List<pre_language_object>)data[2];
            List<pre_language_object> helpers = (List<pre_language_object>)data[3];

            foreach (pre_language_object operand in operands)
            {
                List<object> operand_data = (List<object>)operand.data;

                string operand_name = (string)operand_data[0];
                int operand_offset = (int)big_integer_from_pre_language_object(operand_data[1]);
                int opernad_size = (int)big_integer_from_pre_language_object(operand_data[2]);

                result.create_operand(operand_name, operand_offset, opernad_size);  
            }

            foreach (pre_language_object helper in helpers)
            {
                //TODO:
            }

            result.data_source = (pre_language_object)data[4];

            return result;
        }

        public function_declaration create_empty_function(string function_name, int argument_count)
        {
            function_declaration result = new function_declaration(this);

            result.name = function_name;
            result.return_type = new dynamic_type();
            result.is_empty = true;

            for (int i = 0; i < argument_count; ++i)
            {
                variable_declaration parameter = new variable_declaration();

                string parameter_name = $"parameter_{i}";

                parameter.name = parameter_name;
                parameter.variable_type = new dynamic_type();
                parameter.is_function_parameter = true; 

                result.insert_scoped_object(parameter_name, parameter);
                result.parameters.Add(parameter);
            }

            insert_scoped_object(result.get_function_key(), result);

            functions.Add(result);

            return result;
        }

        public variable_declaration create_undefined_global_variable(string name)
        {
            variable_declaration result = new variable_declaration();

            result.name = name;
            result.variable_type = new dynamic_type();

            result.undefined_global = true;

            insert_scoped_object(name, result);

            global_variables.Add(result);

            return result;
        }

        BigInteger big_integer_from_pre_language_object(object source)
        {
            return big_integer_from_pre_language_object((pre_language_object)source);
        }

        public BigInteger big_integer_from_pre_language_object(pre_language_object source)
        {
            debug_tools.assert(source.type == pre_language_type.constant);

            return (BigInteger)source.data; 
        }

        public static void throw_error(IParseTree token, string message)
        {
            //TODO
        }
    }
}
