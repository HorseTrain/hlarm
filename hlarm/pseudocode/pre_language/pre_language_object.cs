using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.pre_language
{
    public class pre_language_object 
    {
        public pre_language_type type   { get; set; }
        public IParseTree raw_node      { get; set; }

        public string raw_node_string => raw_node.GetText();

        public int tab
        {
            get
            {
                int test = column;

                if ((test % 4) != 0)
                {
                    return -1;
                }

                int working_result = test / 4;

                return working_result;
            }
        }

        public int line     => ((ParserRuleContext)raw_node).Start.Line;
        public int column   => ((ParserRuleContext)raw_node).Start.Column;

        public object data      { get; set; }

        public pre_language_object(IParseTree raw_node,object data = null, pre_language_type type = pre_language_type.undefined)
        {
            this.raw_node = raw_node;
            this.data = data;
            this.type = type;

            validate();
        }

        void throw_error()
        {
            Console.WriteLine(raw_node.GetText());

            throw new Exception();
        }

        void validate()
        {
            if (data is IList lo)
            {
                foreach (object sub_data in lo)
                {
                    if (sub_data != null)
                    {
                        continue;
                    }

                    throw_error();
                }
            }
            else if (data == null)
            {
                throw_error();
            }
        }
    }
}
