using hlarm.pseudocode.language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace hlarm.generators.cs
{
    public class interpreter_generator
    {
        hlarm_context context { get; set; }

        const string base_string = @"using System.Numerics;
using System.Diagnostics;

namespace debug_arm_interpreter
{
    public class interpreter
    {
%DECODING%
%IMPLEMENTATION%
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
";

        public interpreter_generator(hlarm_context context)
        {
            this.context = context;
        }

        string visit(language_object source)
        {
            switch (source)
            {
                case variable_declaration vd: return visit_variable_declaration(vd);
                case function_call fc: return visit_function_call(fc);
                case declaration_reference dr: return visit_declaration_reference(dr);
                case binary_operation bo: return visit_binary_operation(bo);
                case constant c: return visit_constant(c);
                case ternary_operation to: return visit_ternary_operation(to);
                case binary_encoding_pattern bep: return visit_binary_encoding_pattern(bep);
                case l_value_set lvs: return visit_l_value_set(lvs);
                case tuple t: return visit_tuple(t);
                case empty_expression: return visit_empty_expression();
                case if_statement ifs: return visit_if_statement(ifs);
                case bit_fields_access bfs: return visit_bit_feild(bfs);
                case bit_range br: return visit_bit_range(br);
                case return_statement rt: return visit_return_statement(rt);
                case bits_type bt: return visit_type(bt);
                case get_size gs: return visit_get_size(gs);
                case assert_statement ass: return visit_assert_statement(ass);
                case basic_type bt: return visit_type(bt);
                case parentheses pt: return visit_parenthesis(pt);
                case unary_operation uo: return visit_unary_operation(uo);
                case while_loop wl: return visit_while_loop(wl);
                case in_collection ic: return visit_in_collection(ic);
                case for_loop fl: return visit_for_loop(fl);
                case scope sc: return visit_scope(sc);
                case case_statement cs: return visit_case_statement(cs);
                case constant_real cr: return visit_constant_real(cr);
                default: throw new Exception();
            }
        }

        string visit_case_statement(case_statement source)
        {
            string source_test = visit(source.test);

            string final = "";

            foreach (var ws in source.statements)
            {
                string this_test = "";

                foreach (expression e in ws.passes)
                {
                    this_test += to_bool($"{visit(e)} == {source_test}");

                    if (e != ws.passes.Last())
                    {
                        this_test += "||";
                    }
                }

                if (ws != source.statements.First())
                {
                    final += "else ";
                }

                final += $"if ({this_test})\n{visit(ws.code)}\n";
            }

            return final;
        }

        string visit_constant_real(constant_real source)
        {
            return $"new {default_type}((double){source.value})";
        }

        string visit_in_collection(in_collection source)
        {
            string working_result = $"{default_type}.test_in({visit(source.test)}";

            foreach (var v in source.values)
            {
                working_result += "," + visit(v);
            }

            working_result += ")";

            return working_result;
        }

        string to_bool(string source)
        {
            return $"({source}).value != 0";
        }

        string visit_while_loop(while_loop source)
        {
            string working_result = $"while ({visit(source.condition)}.value != 0)\n";

            working_result += visit(source.code);

            return working_result;
        }

        string visit_for_loop(for_loop source)
        {
            string condition = to_bool( $"({visit(source.start_reference)} != {visit(source.end)})");

            string working_result = $"for ({visit(source.start)}; {condition}; {visit(source.start_reference)} {(source.goes_up ? ("++") : ("--"))})\n";

            working_result += visit_scope(source);

            return working_result;
        }

        string visit_assert_statement(assert_statement source)
        {
            return $"Debug.Assert({visit(source.value)}.value == 1)";
        }

        string visit_unary_operation(unary_operation op)
        {
            string operation = op.operation;

            switch (operation)
            {
                case "!":
                    {
                        return $"{default_type}.flip({visit(op.value)})";
                    }

                case "NOT":
                    {
                        operation = "~";
                    }; break;
            }

            return $"{operation}({visit(op.value)})";
        }

        string visit_parenthesis(parentheses source)
        {
            return $"({visit(source.value)})";
        }

        string visit_return_statement(return_statement source)
        {
            if (source.value == null)
            {
                return "return";
            }

            return $"return {visit(source.value)}";
        }

        string visit_get_size(get_size source)
        {
            string result = $"{default_type}.get_size(";

            foreach (var size in source.values)
            {
                result += visit(size);

                if (size != source.values.Last())
                {
                    result += ",";
                }
            }

            result += ")";

            return result;
        }

        string visit_declaration_reference(declaration_reference dr)
        {
            switch (dr.reference)
            {
                case variable_declaration vd: return vd.name;
                case constant c: return visit(c);
                default: throw new Exception();
            }
        }

        string visit_bit_range(bit_range source)
        {
            return $"{visit(source.top_bit)}, {visit(source.bottom_bit)}";
        }

        string visit_bit_feild(bit_fields_access source)
        {
            string first_part = visit(source.base_expression) + ".bits(";

            for (int i = 0; i < source.ranges.Count; ++i)
            {
                first_part += visit(source.ranges[i]);

                if (i != source.ranges.Count - 1)
                {
                    first_part += ",";
                }
            }

            return first_part + ")";
        }

        string visit_if_statement(if_statement if_statmenet_context)
        {
            string result = $"if ({visit(if_statmenet_context.condition)}.value != 0)";

            result += "\n";

            result += visit(if_statmenet_context.pass);

            if (if_statmenet_context.has_else_statement)
            {
                result += "\nelse\n";

                result += visit(if_statmenet_context.fails);
            }

            return result;
        }

        string visit_l_value_set(l_value_set lvs)
        {
            if (lvs.l_value is bit_fields_access bfs)
            {
                List<bit_range> ranges = bfs.ranges;

                string fields = "";

                for (int i = 0; i < ranges.Count; ++i)
                {
                    fields += visit(ranges[i]);

                    if (i != ranges.Count - 1)
                    {
                        fields += ",";
                    }
                }

                return $"{default_type}.set_bits(ref {visit(bfs.base_expression)}, [{fields}], {visit(lvs.r_value)})";
            }

            return $"{visit(lvs.l_value)} = {visit(lvs.r_value)}";
        }

        string visit_empty_expression()
        {
            return "_";
        }

        string visit_tuple(tuple t)
        {
            string result = "(";

            foreach (expression e in t.data)
            {
                result += visit(e);

                if (e != t.data.Last())
                {
                    result += ",";
                }
            }

            result += ")";

            return result;
        }

        string visit_binary_encoding_pattern(binary_encoding_pattern bep)
        {
            return $"new arm_bit_field({bep.mask}, {bep.value}, {bep.length})";
        }

        string visit_ternary_operation(ternary_operation to)
        {
            return $"{visit(to.condition)}.value != 0 ? {visit(to.yes)} : {visit(to.no)}";
        }

        string visit_constant(constant c)
        {
            return $"new {default_type}({c.value})";
        }

        string visit_binary_operation(binary_operation bo)
        {
            string first = visit(bo.first);
            string second = visit(bo.second);

            string operation = bo.operation;

            switch (operation)
            {
                case ":": return $"{default_type}.concat({first}, {second})";
                case "&&": return $"{default_type}.test_and({first}, {second})";
                case "||": return $"{default_type}.test_or({first}, {second})";
                case "^": return $"{default_type}.pow({first}, {second})";
                case "EOR": operation = "^"; break;
                case "DIV": operation = "/"; break;
                case "AND": operation = "&"; break;
                case "OR": operation = "|"; break;
            }

            return $"({first} {operation} {second})";
        }

        string visit_function_call(function_call function_call_context)
        {
            string result = function_call_context.function_reference.name + "(";

            int arg_index = 0;

            foreach (var argument in function_call_context.arguments)
            {
                if (function_call_context.function_reference.parameters[arg_index].is_referable)
                {
                    result += "ref ";
                }

                result += visit(argument);

                if (argument != function_call_context.arguments.Last())
                {
                    result += ",";
                }

                arg_index++;
            }

            result += ")";

            return result;
        }

        string visit_scope(scope sc)
        {
            string result = "{\n";

            foreach (language_object command in sc.commands)
            {
                string to_add = visit(command);

                switch (command)
                {
                    case if_statement:
                        {

                            break;
                        }
                    default: to_add += ";"; break;
                }

                result += string_tools.tab_string(to_add);
            }

            return result + "}";
        }

        string visit_type(expression_type type)
        {
            switch (type)
            {
                case tuple_type tt:
                    {
                        string result = "(";

                        foreach (expression_type type_temp in tt.types)
                        {
                            result += visit(type_temp);

                            if (type_temp != tt.types.Last())
                            {
                                result += ",";
                            }
                        }

                        result += ")";

                        return result;
                    }
                    ;

                case basic_type bt:
                    {
                        if (bt.name == "void")
                            return "void";
                    }
                    ; break;
            }

            return default_type;
        }

        string default_type => "arm_number";

        string visit_variable_declaration(variable_declaration source)
        {
            string type = visit_type(source.variable_type);
            string name = source.name;

            string working_result = $"{type} {name}";

            if (source.is_referable)
            {
                if (!source.is_function_parameter)
                {
                    throw new Exception();
                }

                working_result = $"ref {working_result}";
            }

            if (source.default_value != null)
            {
                working_result += $" = {visit(source.default_value)}";
            }
            else if (!source.is_function_parameter && !source.undefined_global)
            {
                working_result += $" = new {default_type}()";
            }

            if (source.undefined_global)
            {
                working_result = $"{working_result} {{ get; set; }}";
            }

            return working_result;
        }

        string generate_instruction(instruction_declaration source)
        {
            string result = $"void {instruction_name(source)}(";

            foreach (instruction_operand operand in source.operands)
            {
                result += $"{default_type} {operand.operand_name}";

                if (operand != source.operands.Last())
                {
                    result += ",";
                }
            }

            result += ")\n";

            result += visit(source);

            return result;
        }

        string generate_function(function_declaration source)
        {
            string result = $"{visit_type(source.return_type)} {source.name}(";

            foreach (var parameter in source.parameters)
            {
                result += visit(parameter);

                if (parameter != source.parameters.Last())
                {
                    result += ",";
                }
            }

            result += ")";

            if (source.is_empty)
            {
                return @$"protected virtual {result} 
{{ 
    throw new NotImplementedException(); 
}}";
            }

            return result + $"\n{visit(source)}";
        }

        string instruction_name(instruction_declaration declaration)
        {
            return $"instruction_{declaration.instruction_encoding:x}_{declaration.instruction_mask:x}";    
        }

        string generate_decoding()
        {
            string working_result = @"List<decoding_table> decoding_tables;

public interpreter()
{
    decoding_tables = [
";

            foreach (var instruction in context.asl.instructions)
            {
                string test_result = $"new decoding_table({instruction.instruction_encoding}, {instruction.instruction_mask}, execute_{instruction_name(instruction)})";

                if (instruction != context.asl.instructions.Last())
                {
                    test_result += ",";
                }

                test_result = string_tools.tab_string(test_result, 2);

                working_result += test_result;
            }

            working_result += "\t];\n";

            working_result += "}\n";

            foreach (var instruction in context.asl.instructions)
            {
                working_result += $"static void execute_{instruction_name(instruction)}(interpreter interpreter_context, int instruction)\n{{\n";

                foreach (var instruction_operand in instruction.operands)
                {
                    working_result += $"\t{default_type} {instruction_operand.operand_name} = new {default_type}((instruction >> {instruction_operand.operand_offset}) & {(1 << instruction_operand.operand_length) - 1}, {instruction_operand.operand_length});\n";
                }

                working_result += $"\tinterpreter_context.{instruction_name(instruction)}(";

                foreach (var instruction_operand in instruction.operands)
                {
                    working_result += instruction_operand.operand_name;

                    if (instruction_operand != instruction.operands.Last())
                    {
                        working_result += ",";
                    }
                }

                working_result += ");\n}\n";
            }

            return working_result;
        }

        public string generate_interpreter()
        {
            string working_result = base_string;

            StringBuilder implementation = new StringBuilder();

            foreach (var variable in context.asl.global_variables)
            {
                implementation.Append(visit(variable));
                implementation.Append("\n");
            }

            foreach (var function in context.asl.functions)
            {
                implementation.Append(generate_function(function));
                implementation.Append("\n");
            }

            foreach (var instruction in context.asl.instructions)
            {
                implementation.Append(generate_instruction(instruction));
                implementation.Append("\n");
            }

            working_result = working_result.Replace("%DECODING%", string_tools.tab_string(generate_decoding(), 2));
            working_result = working_result.Replace("%IMPLEMENTATION%", string_tools.tab_string(implementation.ToString(), 2));

            return working_result;
        }
    }
}
