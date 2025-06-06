using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.pre_language
{
    public static class scoping_tools
    {
        public static int get_inner_scope_from(ref List<pre_language_object> result, List<pre_language_object> source, int start, int source_tab)
        {
            int last_line = -1;
            int test_tab = source_tab;

            for (int i = start; i < source.Count; i++)
            {
                pre_language_object sub_tab_working_object = source[i];

                if (last_line != sub_tab_working_object.line)
                {
                    last_line = sub_tab_working_object.line;
                    test_tab = sub_tab_working_object.tab;
                }

                if (test_tab < source_tab)
                {
                    return i - 1;
                }
                else
                {
                    result.Add(sub_tab_working_object);
                }
            }

            return source.Count - 1;
        }

        public static List<pre_language_object> group_pre_language_objects(List<pre_language_object> source, int source_tab = 0)
        {
            List<pre_language_object> result = new List<pre_language_object>();

            int last_line = -1;
            int test_tab = -1;

            for (int i = 0; i < source.Count; ++i)
            {
                pre_language_object working_object = source[i];

                if (last_line != working_object.line)
                {
                    last_line = working_object.line;    
                    test_tab = working_object.tab;
                }

                if (test_tab == -1)
                {
                    throw new Exception();
                }
                else if (test_tab == source_tab)
                {
                    result.Add(working_object);
                }
                else if (test_tab > source_tab)
                {
                    List<pre_language_object> working_scope = new List<pre_language_object>();

                    i = get_inner_scope_from(ref working_scope, source, i, source_tab + 1);

                    IParseTree first_token = null;

                    if (working_scope.Count >= 1)
                    {
                        first_token = working_scope[0].raw_node;
                    }

                    working_scope = group_pre_language_objects(working_scope, source_tab + 1);

                    result.Add(new pre_language_object(first_token, new pre_language_scope(source_tab + 1, working_scope), pre_language_type.scope));
                }
                else
                {
                    throw new Exception();
                }
            }

            result = group_scoped_objects(result, source_tab);

            return result;
        }

        static List<pre_language_object> group_if_else_if_statements(List<pre_language_object> source)
        {
            while (true)
            {
                List<pre_language_object> result_temp = new List<pre_language_object>();
                HashSet<pre_language_object> to_ignore = new HashSet<pre_language_object>();

                for (int i = source.Count- 1; i != -1; --i)
                {
                    pre_language_object working_language_object = source[i];

                    if (i + 1 >= source.Count)
                    {
                        continue;
                    }

                    pre_language_object next_working_language_object = source[i + 1];

                    if (!(working_language_object.type == pre_language_type.if_statment &&
                        (
                            next_working_language_object.type == pre_language_type.else_if_statement ||
                            next_working_language_object.type == pre_language_type.else_statement
                        ))

                        )
                    {
                        continue;
                    }

                    ((List<object>)working_language_object.data).Add(next_working_language_object);

                    to_ignore.Add(next_working_language_object);
                }

                foreach (var s in source)
                {
                    if (to_ignore.Contains(s))
                        continue;

                    result_temp.Add(s);  
                }

                source = result_temp;

                if (to_ignore.Count == 0)
                {
                    break;
                }
            }

            return source;
        }

        static List<pre_language_object> group_scoped_objects(List<pre_language_object> source, int source_tab)
        {
            List<pre_language_object> result = new List<pre_language_object>();

            for (int i = 0; i < source.Count; ++i)
            {
                pre_language_object working_language_object = source[i];    

                switch (working_language_object.type)
                {
                    case pre_language_type.instruction_declaration:
                    case pre_language_type.if_statment:
                    case pre_language_type.else_if_statement:
                    case pre_language_type.else_statement:
                    case pre_language_type.function_declaration:
                        {
                            List<object> to_modify = (List<object>)working_language_object.data;

                            to_modify.Add(source[i + 1]);

                            working_language_object.data = to_modify;    

                            ++i;

                            result.Add(working_language_object);
                        }; break;

                    default: result.Add(working_language_object); break;
                }
            }

            result = group_if_else_if_statements(result);   

            return result;
        }
    }
}
