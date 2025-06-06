using hlarm.pseudocode.pre_language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class function_declaration : scope
    {
        public string                       name        { get; set; }
        public expression_type              return_type { get; set; }
        public List<variable_declaration>   parameters  { get; set; }
        public bool                         is_empty    { get; set; } 
        public pre_language_object          data_source { get; set; }

        public function_declaration(scope parent_scope) : base (parent_scope) 
        {
            parameters = new List<variable_declaration>();
        }

        public string get_function_key()
        {
            return $"{name} {parameters.Count}";
        }

        public void pre_build_function_data()
        {
            List<object> data = (List<object>)data_source.data;

            pre_language_object arguments = (pre_language_object)data[2];

            List<pre_language_object> argument_data = (List<pre_language_object>)arguments.data;

            Dictionary<string, List<expression>> new_names = new Dictionary<string, List<expression>>();

            foreach (pre_language_object argument in argument_data)
            {
                pre_variable_declaration_data declaration_data = (pre_variable_declaration_data)argument.data;

                List<expression> expressions_to_add_to = null;

                if (declaration_data.vairbale_type.type == pre_language_type.dynamic_type)
                {
                    pre_language_object size = (pre_language_object)declaration_data.vairbale_type.data;

                    if (size.type == pre_language_type.identifier_collection)
                    {
                        string new_name = name_from_collection(size);

                        if (!new_names.ContainsKey(new_name))
                        {
                            new_names.Add(new_name, new List<expression>());

                            create_variable_declaration(new_name, new dynamic_type(), null);
                        }

                        expressions_to_add_to = new_names[new_name];
                    }
                }

                variable_declaration declaration = (variable_declaration)visit(argument);

                if (expressions_to_add_to != null)
                {
                    expression new_size = new declaration_reference() { reference = declaration };

                    expressions_to_add_to.Add(new_size);
                }

                parameters.Add(declaration);
                declaration.is_function_parameter = true;
            }

            return_type = (expression_type)visit(data[0]);

            foreach (var d in new_names)
            {
                variable_declaration working_declaration = (variable_declaration)get_scoped_object(d.Key);

                working_declaration.default_value = new get_size() { values = d.Value };

                commands.Add(working_declaration);
            }

            thread_static_source_file_context.insert_scoped_object(get_function_key(), this);

            data_source = (pre_language_object)data[3];
        }

    }
}
