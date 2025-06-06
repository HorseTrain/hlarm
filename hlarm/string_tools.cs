using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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

        public static string tab_string(string source, int count = 1, bool new_lines = true)
        {
            string[] parts = source.Split('\n');

            StringBuilder result = new StringBuilder();

            for (int p = 0; p < parts.Length; ++p)
            {
                string part = parts[p];

                for (int i = 0; i < count; ++i)
                {
                    result.Append("    ");
                }

                if (new_lines)
                {
                    result.Append($"{part}\n");
                }
            }

            return result.ToString();
        }

        static int int_from_hex_char(char source)
        {
            if (source >= '0' && source <= '9')
            {
                return source - '0';
            }

            if (!(source >= 'a' && source <= 'f'))
            {
                throw new Exception();
            }

            return 10 + (source - 'a');
        }

        public static BigInteger big_integer_from_hex_string(string source)
        {
            BigInteger result = 0;

            source = source.ToLower();

            for (int i = 0; i < source.Length; ++i)
            {
                char working_char = source[source.Length - i - 1];

                result |= ((BigInteger)int_from_hex_char(working_char)) << (i * 4);
            }

            return result;
        }

        public static BigInteger big_integer_from_binary_string(string source)
        {
            BigInteger result = 0;

            for (int i = 0; i < source.Length; ++i)
            {
                char working_char = source[source.Length - i - 1];  

                if (working_char == 0)
                {
                    continue;
                }

                result |= (BigInteger)1 << i;
            }

            return result;
        }
    }
}
