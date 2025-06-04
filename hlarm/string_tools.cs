using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm
{
    public static class string_tools
    {
        public static string fix_string(string source)
        {
            for (char i = '\0'; i < ' '; ++i)
            {
                if (i == '\n')
                    continue;

                source = source.Replace($"{i}", "");
            }

            return source;
        }

        public static string create_repeating_line(string working, int count)
        {
            string result = "";

            for (int i = 0; i < count; ++i)
            {
                result += working;
            }

            return result;
        }

        public static string reverse_string(string source)
        {
            StringBuilder result = new StringBuilder();

            for (int i = source.Length - 1; i != -1; --i)
            {
                result.Append(source[i]);
            }

            return result.ToString();
        }

        public static string combine_strings(string[] source)
        {
            StringBuilder result = new StringBuilder();

            foreach (var s in source)
            {
                result.Append(s);
            }

            return result.ToString();
        }
    }
}
