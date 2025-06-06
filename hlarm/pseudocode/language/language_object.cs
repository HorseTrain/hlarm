using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.language
{
    public class language_object
    {
        [ThreadStatic]
        protected static source_file thread_static_source_file_context;

        public static void setup_source_file(source_file working_source_file)
        {
            thread_static_source_file_context = working_source_file;
        }

        source_file     source_file_context     { get; set; }

        public language_object()
        {
            source_file_context = thread_static_source_file_context;
        }
    }
}
