using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static aslParser;

namespace hlarm.pseudocode.parser
{
    public static class source_loader
    {
        public class error : BaseErrorListener
        {
            public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                base.SyntaxError(output, recognizer, offendingSymbol, line, charPositionInLine, msg, e);

                throw new Exception();
            }
        }

        public static SourceFileContext get_parse_tree(string source)
        {
            ICharStream stream = CharStreams.fromString(source);
            ITokenSource lexer = new aslLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            aslParser parser = new aslParser(tokens);

            parser.AddErrorListener(new error());   

            SourceFileContext tree = parser.sourceFile();

            return tree;
        }
    }
}
