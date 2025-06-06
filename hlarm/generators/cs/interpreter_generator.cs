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
        hlarm_context   context     { get; set; }

        const string base_string = @"using System.Numerics;

namespace debug_arm_interpreter
{
    public class arm_integer
    {
        public BigInteger   value   { get; set; }
        public int          size    { get; set; }

        public BigInteger get_mask()
        {
            if (size == -1)
            {
                throw new Exception(); 
            }

            return ((BigInteger)1 << size) - 1;
        }

        public BigInteger get_masked_value()
        {
            return value & get_mask();  
        }

        public arm_integer(BigInteger value, int size = -1)
        {
            this.value = value;
            this.size = size;
        }

        public static implicit operator arm_integer(arm_bit_field source)
        {
            return new arm_integer(source.value, source.size);  
        }

        public static arm_integer operator == (arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value == (int)second.value ? 1 : 0);
        }

        public static arm_integer operator !=(arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value == (int)second.value ? 0 : 1);
        }

        public static implicit operator arm_integer(BigInteger source)
        {
            return new arm_integer(source);
        }

        public static arm_integer operator << (arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value << (int)second.value);
        } 

        public static arm_integer operator >>(arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value << (int)second.value);
        }

        public static arm_integer operator +(arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value + (int)second.value);
        }

        public static arm_integer operator -(arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value + (int)second.value);
        }

        public static arm_integer operator *(arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value * (int)second.value);
        }

        public static arm_integer operator /(arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value / (int)second.value);
        }

        public static arm_integer operator & (arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value & (int)second.value);
        }

        public static arm_integer operator | (arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value | (int)second.value);
        }

        public static arm_integer operator ^ (arm_integer first, arm_integer second)
        {
            return new arm_integer(first.value ^ (int)second.value);
        }

        public static arm_integer test_and(arm_integer first, arm_integer second)
        {
            BigInteger _f = first.value != 0 ? 1 : 0;
            BigInteger _s = second.value != 0 ? 1 : 0;

            return new arm_integer(_f & _s);
        }
        public static arm_integer test_or(arm_integer first, arm_integer second)
        {
            BigInteger _f = first.value != 0 ? 1 : 0;
            BigInteger _s = second.value != 0 ? 1 : 0;

            return new arm_integer(_f | _s);
        }

        public static arm_integer concat(arm_integer first, arm_integer second)
        {
            if (first.size == -1 || second.size == -1)
            {
                throw new Exception();
            }

            BigInteger first_n = first.get_masked_value();
            BigInteger second_n = second.get_masked_value();

            BigInteger result = (first_n << second.size) | second_n;

            return new arm_integer(result, first.size + second.size);
        }

        public arm_integer bits(int top, int bottom)
        {
            top++;

            int size = top - bottom;

            BigInteger mask = ((BigInteger)1 << size) - 1;

            return new arm_integer((value >> bottom) & mask, size);
        }

        public arm_integer bits(params arm_integer[] parts)
        {
            if (parts.Count() % 2 != 0)
            {
                throw new Exception();
            }

            arm_integer result = new arm_integer(0, 0);

            for (int i = 0; i < parts.Length; i += 2)
            {
                result = concat(result, bits((int)parts[i].value, (int)parts[i + 1].value));
            }

            return result;
        }

        public static arm_integer get_size(params arm_integer[] values)
        {
            int working_size = values[0].size;

            for (int i = 1; i < values.Length; ++i)
            {
                if (values[i].size != working_size)
                {
                    throw new Exception();
                }
            }

            return new arm_integer(working_size);
        }

        public static void set_bits(ref arm_integer source, arm_integer[] parts, arm_integer new_value)
        {

        }
    }

    public class arm_bit_field
    {
        public BigInteger   mask    { get; set; }
        public BigInteger   value   { get; set; }
        public int          size    { get; set; }  

        public arm_bit_field(BigInteger mask, BigInteger value, int size)
        {
            this.mask = mask;
            this.value = value;
            this.size = size;
        }

        public static arm_integer operator == (arm_integer first,  arm_bit_field second)
        {
           return new arm_integer((first.value & second.mask) == second.value ? 1 : 0);  
        }

        public static arm_integer operator !=(arm_integer first, arm_bit_field second)
        {
            return new arm_integer((first.value & second.mask) == second.value ? 0 : 1);
        }

        public static arm_integer operator ==(arm_bit_field second, arm_integer first)
        {
            return new arm_integer((first.value & second.mask) == second.value ? 1 : 0);
        }

        public static arm_integer operator !=(arm_bit_field second, arm_integer first)
        {
            return new arm_integer((first.value & second.mask) == second.value ? 0 : 1);
        }
    }

    public class interpreter
    {
%IMPLEMENTATION%
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
                case scope sc: return visit_scope(sc);
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
                default: throw new Exception();
            }
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
            string result = "arm_integer.get_size(";

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

                return $"arm_integer.set_bits(ref {visit(bfs.base_expression)}, [{fields}], {visit(lvs.r_value)})";
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
                case ":": return $"arm_integer.concat({first}, {second})";
                case "&&": return $"arm_integer.test_and({first}, {second})";
                case "||": return $"arm_integer.test_or({first}, {second})";
            }

            return $"({first} {operation} {second})";
        }

        string visit_function_call(function_call function_call_context)
        {
            string result = function_call_context.function_reference.name + "(";

            foreach (var argument in function_call_context.arguments)
            {
                result += visit(argument);

                if (argument != function_call_context.arguments.Last())
                {
                    result += ",";
                }
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
                    }; break;
            }

            return default_type;
        }

        string default_type => "arm_integer";

        string visit_variable_declaration(variable_declaration source)
        {
            string type = visit_type(source.variable_type);
            string name = source.name;

            string working_result = $"{type} {name}";

            if (source.default_value != null)
            {
                working_result += $" = {visit(source.default_value)}";
            }

            if (source.undefined_global)
            {
                working_result = $"{working_result} {{ get; set; }}";
            }

            return working_result;
        }

        string generate_instruction(instruction_declaration source)
        {
            string result = $"void instruction_{source.instruction_encoding:x}_{source.instruction_mask:x}(";

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

            working_result = working_result.Replace("%IMPLEMENTATION%", string_tools.tab_string(implementation.ToString(), 2));

            return working_result;
        }
    }
}
