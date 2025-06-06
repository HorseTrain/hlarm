using hlarm.pseudocode.pre_language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class scope : language_object
    {
        public scope                                parent_scope    { get; set; }
        public Dictionary<string, language_object>  defined_objects { get; set; }
        public List<language_object>                commands        { get; set; }

        public scope(scope parent_scope)
        {
            this.parent_scope = parent_scope;

            defined_objects = new Dictionary<string, language_object>();
            commands = new List<language_object>();
        }

        public language_object get_scoped_object(string path)
        {
            if (defined_objects.ContainsKey(path))
            {
                return defined_objects[path];
            }

            if (parent_scope == null)
            {
                return null;
            }

            return parent_scope.get_scoped_object(path);
        }

        public void insert_scoped_object(string name, language_object to_add)
        {
            defined_objects.Add(name, to_add);
        }

        public void evaluate_commands(pre_language_object scope_source)
        {
            debug_tools.assert(scope_source.type == pre_language_type.scope);

            pre_language_scope source_commands = (pre_language_scope)scope_source.data;

            foreach (pre_language_object command in source_commands.code)
            {
                language_object new_command = visit(command);

                if (new_command == null)
                    continue;

                commands.Add(new_command);  
            }
        }

        public language_object visit(object source)
        {
            return visit((pre_language_object)source);
        }

        public language_object visit(pre_language_object source)
        {
            switch (source.type)
            {
                case pre_language_type.variable_declaration: return visit_variable_declaration(source);
                case pre_language_type.function_call: return visit_function_call(source);
                case pre_language_type.identifier_collection: return visit_identifier_collection(source);
                case pre_language_type.binary_operation: return visit_binary_operation(source);
                case pre_language_type.constant: return visit_constant(source);
                case pre_language_type.ternary: return visit_ternary_expression(source);
                case pre_language_type.binary_encoding_pattern: return visit_binary_encoding_pattern(source);
                case pre_language_type.l_value_set: return visit_l_value_set(source);
                case pre_language_type.tuple: return visit_tuple(source);
                case pre_language_type.empty_expression: return new empty_expression();
                case pre_language_type.if_statment: return visit_if_statement(source);
                case pre_language_type.scope: return visit_scope(source);
                case pre_language_type.bit_field_accessed_value: return visit_bit_field_accessed_value(source);
                case pre_language_type.bit_field: return visit_bit_feild(source);
                case pre_language_type.lined_expression: return visit_lined_expression(source);
                case pre_language_type.unary_operation: return visit_unary_operation(source);
                case pre_language_type.parentheses: return visit_parentheses(source);
                case pre_language_type.tuple_type: return visit_expression_type(source);
                case pre_language_type.dynamic_type: return visit_expression_type(source);
                case pre_language_type.return_statement: return visit_return_statement(source);
                default: throw new Exception();
            }
        }

        scope visit_scope(pre_language_object source)
        {
            scope result = new scope(this);

            result.evaluate_commands(source);   

            return result;
        }

        return_statement visit_return_statement(pre_language_object source)
        {
            object test = source.data;

            return_statement to_return = new return_statement();

            if (test is not string)
            {
                to_return.value = (expression)visit(source.data);
            }

            return to_return;            
        }

        unary_operation visit_unary_operation(pre_language_object source)
        {
            unary_operation result = new unary_operation();

            List<object> data = (List<object>)source.data;

            result.operation = (string)data[0];
            result.value = (expression)visit(data[1]);

            return result;
        }

        parentheses visit_parentheses(pre_language_object source)
        {
            pre_language_object data = (pre_language_object)source.data;

            parentheses result = new parentheses();

            result.value = (expression)visit(data);

            return result;
        }

        language_object visit_lined_expression(pre_language_object source)
        {
            pre_language_object data = (pre_language_object)source.data;

            return visit(data);
        }

        bit_range visit_bit_feild(pre_language_object source)
        {
            List<object> data = (List<object>)source.data;

            expression first = (expression)visit(data[0]);
            expression second = (expression)visit(data[0]); //WE DO NOT WANT A COPY REFERENCE OF THE FIRST THING!

            if (data.Count == 2)
            {
                second = (expression)visit(data[1]);
            }

            return new bit_range(first, second);
        }

        bit_fields_access visit_bit_field_accessed_value(pre_language_object source)
        {
            List<object> data = (List<object>)source.data;

            bit_fields_access result = new bit_fields_access();

            result.base_expression = (expression)visit(data[0]);

            for (int i = 1; i < data.Count; ++i)
            {
                result.ranges.Add((bit_range)visit(data[i]));
            }

            var t = result.get_expression_type();

            return result;
        }

        scope convert_to_scope(language_object test)
        {
            if (test == null)
            {
                return null;
            }

            if (test is scope s)
            {
                return s;
            }

            scope working_scope = new scope(this);

            working_scope.commands.Add(test);

            return working_scope;
        }

        if_statement visit_if_statement(pre_language_object source)
        {
            if_statement result = new if_statement();

            List<object> data = (List<object>)source.data;

            result.condition = (expression)visit((pre_language_object)data[0]);
            result.pass = visit((pre_language_object)data[1]);

            if (data.Count == 3)
            {
                pre_language_object fail_statement = (pre_language_object)data[2];

                List<object> fail_data = (List<object>)fail_statement.data;

                if (fail_statement.type == pre_language_type.else_statement)
                {
                    result.fails = visit((pre_language_object)fail_data[0]);
                }
                else if (fail_statement.type == pre_language_type.else_if_statement)
                {
                    throw new Exception();
                }
            }

            result.pass = convert_to_scope(result.pass);
            result.fails = convert_to_scope(result.fails);

            return result;
        }

        tuple visit_tuple(pre_language_object source)
        {
            tuple result = new tuple();

            List<pre_language_object> data = (List<pre_language_object>)source.data;

            foreach (var d in data)
            {
                expression working_expression = (expression)visit(d);

                result.data.Add(working_expression);
            }

            return result;
        }

        language_object visit_l_value_set(pre_language_object source)
        {
            List<pre_language_object> data = (List<pre_language_object>)source.data;

            expression right = (expression)visit(data[1]);

            if (data[0].type == pre_language_type.function_call)
            {
                return visit_function_call(data[0], right); 
            }

            expression left = (expression)visit(data[0]);

            if (left is declaration_reference dr && dr.reference == null)
            {
                string name = data[0].raw_node.GetText();

                return create_variable_declaration(name, right.get_expression_type(), right);
            }

            return new l_value_set(left, right);
        }

        binary_encoding_pattern visit_binary_encoding_pattern(pre_language_object source)
        {
            debug_tools.assert(source.type == pre_language_type.binary_encoding_pattern);

            string data = (string)source.data;

            return new binary_encoding_pattern(data);
        }

        ternary_operation visit_ternary_expression(pre_language_object source)
        {
            debug_tools.assert(source.type == pre_language_type.ternary);

            ternary_operation result = new ternary_operation();

            List<object> data = (List<object>)source.data;

            result.condition = (expression)visit((pre_language_object)data[0]);
            result.yes = (expression)visit((pre_language_object)data[1]);
            result.no = (expression)visit((pre_language_object)data[2]);

            return result;
        }

        expression visit_binary_operation(pre_language_object source)
        {
            debug_tools.assert(source.type == pre_language_type.binary_operation);

            List<object> data = (List<object>)source.data;

            expression first = (expression)visit((pre_language_object)data[0]);
            expression second = (expression)visit((pre_language_object)data[2]);
            string operation = (string)data[1];

            return new binary_operation() { first = first, second = second, operation = operation };    
        }

        expression visit_constant(pre_language_object source)
        {
            debug_tools.assert(source.type == pre_language_type.constant);

            BigInteger raw_value = thread_static_source_file_context.big_integer_from_pre_language_object(source);

            return new constant(raw_value);
        }

        expression visit_identifier_collection(pre_language_object source)
        {
            debug_tools.assert(source.type == pre_language_type.identifier_collection);

            List<string> names = (List<string>)source.data;

            if (names.Count == 1)
            {
                declaration_reference result = new declaration_reference();
                result.reference = get_scoped_object(names[0]);
                result.reference_name = names[0];

                if (result.reference == null)
                {
                    result.reference = thread_static_source_file_context.create_undefined_global_variable(result.reference_name);
                }

                return result;
            }
            else
            {
                //TODO:
                struct_reference result = new struct_reference();

                result.name = name_from_collection(names);

                return result;
            }
        }

        static string name_from_collection(List<string> data)
        {
            string result = "";

            for (int i = 0; i < data.Count; ++i)
            {
                result += data[i];

                if (i != data.Count - 1)
                {
                    result += ".";
                }
            }

            return result;
        }


        public static string name_from_collection(pre_language_object collection)
        {
            debug_tools.assert(collection.type == pre_language_type.identifier_collection);

            List<string> data = (List<string>)collection.data;

            return name_from_collection(data);
        }

        function_call visit_function_call(pre_language_object source, expression extra_argument = null)
        {
            function_call result = new function_call();

            List<object> data = (List<object>)source.data;

            string function_name = name_from_collection((pre_language_object)data[0]);
            pre_language_object arguments = (pre_language_object)data[1];

            foreach (pre_language_object arg in (List<pre_language_object>)arguments.data)
            {
                result.arguments.Add((expression)visit(arg));
            }

            if (extra_argument != null)
            {
                result.arguments.Add(extra_argument);   
            }

            result.function_key = $"{function_name} {result.arguments.Count}";
            result.function_reference = get_scoped_object(result.function_key) as function_declaration;

            if (result.function_reference == null)
            {
                result.function_reference = thread_static_source_file_context.create_empty_function(function_name, result.arguments.Count);
            }

            return result;
        }

        public variable_declaration create_variable_declaration(string name, expression_type type, expression default_value)
        {
            variable_declaration result = new variable_declaration();

            result.name = name;
            result.variable_type = type;
            result.default_value = default_value;   

            insert_scoped_object(result.name, result);

            return result;
        }

        variable_declaration visit_variable_declaration(pre_language_object source)
        {
            pre_variable_declaration_data data = (pre_variable_declaration_data)source.data;

            if (data.variable_names.Count > 1)
            {
                throw new Exception();
            }

            string name = data.variable_names[0];
            expression_type variable_type = visit_expression_type(data.vairbale_type);
            expression default_value = null;

            if (data.default_value != null)
            {
                default_value = visit(data.default_value) as expression;
            }

            return create_variable_declaration(name, variable_type, default_value); 
        }

        expression_type visit_expression_type(pre_language_object source)
        {
            switch (source.type)
            {
                case pre_language_type.concrete_type:
                    {
                        string type_name = (string)source.data;

                        return (expression_type)get_scoped_object(type_name);
                    };

                case pre_language_type.dynamic_type:
                    {
                        expression length = (expression)visit((pre_language_object)source.data);

                        return new bits_type(length);   
                    };

                case pre_language_type.tuple_type:
                    {
                        tuple_type result = new tuple_type();

                        List<pre_language_object> data = (List<pre_language_object>)source.data;  

                        foreach (pre_language_object o in data)
                        {
                            result.types.Add((expression_type)visit(o));
                        }

                        return result;
                    }
                    ; 
                default: throw new Exception();
            }
        }
    }
}
