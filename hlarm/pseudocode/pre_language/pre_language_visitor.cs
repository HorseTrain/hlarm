using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static aslParser;

namespace hlarm.pseudocode.pre_language
{
    public class pre_language_visitor : aslBaseVisitor<pre_language_object>
    {
        const string empty = "¯\\_(ツ)_/¯";

        public override pre_language_object VisitSourceFile([NotNull] SourceFileContext context)
        {
            List<pre_language_object> result = new List<pre_language_object>();

            foreach (LineContext line in context.line())
            {
                pre_language_object to_add = Visit(line);

                if (to_add == null)
                {
                    Console.WriteLine(line.GetText());  

                    throw new Exception();
                }

                result.Add(to_add);
            }

            result = scoping_tools.group_pre_language_objects(result);

            return new pre_language_object(context, result, pre_language_type.source_file);
        }

        static pre_language_object create_empty_expression(IParseTree context)
        {
            return new pre_language_object(context, empty, pre_language_type.empty_expression);
        }

        public override pre_language_object VisitTuple([NotNull] TupleContext context)
        {
            List<pre_language_object> result = new List<pre_language_object>();

            foreach (var e in context.tupleSingle())
            {
                if (e.GetText() == "-")
                {
                    result.Add(create_empty_expression(e));

                    continue;
                }

                result.Add(Visit(e));
            }

            return new pre_language_object(context, result, pre_language_type.tuple);
        }

        public override pre_language_object VisitNormalExplicitFunctionDeclaration([NotNull] NormalExplicitFunctionDeclarationContext context)
        {
            List<object> result = new List<object>();

            result.Add(Visit(context.type()));
            result.Add(Visit(context.identifierPath()));
            result.Add(Visit(context.functionArguments()));

            return new pre_language_object(context, result, pre_language_type.function_declaration);
        }

        public override pre_language_object VisitFunctionArguments([NotNull] FunctionArgumentsContext context)
        {
            if (context.commaSeperatedExpressions() == null)
            {
                return create_default_arguments(context);
            }

            return Visit(context.commaSeperatedExpressions());
        }

        public override pre_language_object VisitLinedExpression([NotNull] LinedExpressionContext context)
        {
            pre_language_object result = Visit(context.expression());

            return new pre_language_object(context, result, pre_language_type.lined_expression);
        }

        public override pre_language_object VisitLValueSet([NotNull] LValueSetContext context)
        {
            pre_language_object l_value = Visit(context.expression(0));
            pre_language_object r_Value = Visit(context.expression(1));

            return new pre_language_object(context, new List<pre_language_object>([l_value, r_Value]), pre_language_type.l_value_set);
        }

        public override pre_language_object VisitReturnStatement([NotNull] ReturnStatementContext context)
        {
            if (context.expression() == null)
            {
                return new pre_language_object(context, empty, pre_language_type.return_statement);
            }

            pre_language_object to_return = Visit(context.expression());

            return new pre_language_object(context, to_return, pre_language_type.return_statement);
        }

        public override pre_language_object VisitIfStatement([NotNull] IfStatementContext context)
        {
            List<object> data = new List<object>([Visit(context.expression())]);

            return new pre_language_object(context, data, pre_language_type.if_statment);
        }

        public override pre_language_object VisitElseStatement([NotNull] ElseStatementContext context)
        {
            return new pre_language_object(context, new List<object>(), pre_language_type.else_statement);
        }

        public override pre_language_object VisitElseIfStatement([NotNull] ElseIfStatementContext context)
        {
            List<object> data = new List<object>([Visit(context.expression())]);

            return new pre_language_object(context, data, pre_language_type.else_if_statement);
        }

        public override pre_language_object VisitInstructionDeclaration([NotNull] InstructionDeclarationContext context)
        {
            List<object> instruction_data = new List<object>();

            instruction_data.Add(Visit(context.constant(0)));
            instruction_data.Add(Visit(context.constant(1)));

            List<pre_language_object> operand_data = new List<pre_language_object>();
            List<pre_language_object> helper_data = new List<pre_language_object>();

            instruction_data.Add(operand_data);
            instruction_data.Add(helper_data);

            foreach (var operand in context.operandData())
            {
                operand_data.Add(Visit(operand));
            }

            foreach (var helper in context.instructionHelperData())
            {
                operand_data.Add(Visit(helper));
            }

            return new pre_language_object(context, instruction_data, pre_language_type.instruction_declaration);
        }

        public override pre_language_object VisitVariableDeclaration([NotNull] VariableDeclarationContext context)
        {
            pre_variable_declaration_data data = new pre_variable_declaration_data();

            data.vairbale_type = Visit(context.type());

            foreach (var identifier in context.identifier())
            {
                data.variable_names.Add(identifier.GetText());
            }

            if (context.expression() != null)
            {
                data.default_value = Visit(context.expression());   
            }

            if (context.referenceTag() != null)
            {
                data.is_referable = true;
            }

            data.validate();

            return new pre_language_object(context, data, pre_language_type.variable_declaration);
        }

        public override pre_language_object VisitConcreteTypes([NotNull] ConcreteTypesContext context)
        {
            return new pre_language_object(context, context.GetText(), pre_language_type.concrete_type);
        }

        public override pre_language_object VisitDynamicTypes([NotNull] DynamicTypesContext context)
        {
            return new pre_language_object(context, Visit(context.expression()), pre_language_type.dynamic_type);
        }

        public override pre_language_object VisitConstantType([NotNull] ConstantTypeContext context)
        {
            return Visit(context.type());
        }

        public override pre_language_object VisitTupleType([NotNull] TupleTypeContext context)
        {
            List<pre_language_object> result = new List<pre_language_object>();

            foreach (var type in context.type())
            {
                result.Add(Visit(type));    
            }

            return new pre_language_object(context, result, pre_language_type.tuple_type);
        }

        public override pre_language_object VisitConstant([NotNull] ConstantContext context)
        {
            string constant_string = context.GetText().ToLower();

            if (constant_string.StartsWith("0x"))
            {
                constant_string = constant_string.Replace("_", "").Replace("0x", "");

                BigInteger result = string_tools.big_integer_from_hex_string(constant_string);  

                return new pre_language_object(context, result, pre_language_type.constant);
            }
            else if (constant_string.StartsWith("0b"))
            {
                constant_string = constant_string.Replace("_", "").Replace("0b", "");

                BigInteger result = string_tools.big_integer_from_binary_string(constant_string);

                return new pre_language_object(context, result, pre_language_type.constant);
            }
            else
            {
                return new pre_language_object(context, BigInteger.Parse(constant_string), pre_language_type.constant);
            }

            throw new Exception();
        }

        public override pre_language_object VisitTrueFalse([NotNull] TrueFalseContext context)
        {
            string constant_string = context.GetText();

            if (constant_string == "FALSE")
            {
                return new pre_language_object(context, new BigInteger(0), pre_language_type.constant);
            }
            else
            {
                return new pre_language_object(context, BigInteger.Parse(constant_string), pre_language_type.constant);
            }
        }

        public override pre_language_object VisitOperandData([NotNull] OperandDataContext context)
        {
            List<object> data = new List<object>();

            data.Add(context.identifier().GetText());
            data.Add(Visit(context.constant(0)));
            data.Add(Visit(context.constant(1)));

            return new pre_language_object(context, data);
        }

        public override pre_language_object VisitInstructionHelperData([NotNull] InstructionHelperDataContext context)
        {
            List<object> data = new List<object>();

            data.Add(context.GetChild(0).GetText());
            data.Add(Visit(context.constant(0)));
            data.Add(Visit(context.constant(1)));
            data.Add(Visit(context.constant(2)));

            return new pre_language_object(context, data);
        }

        public override pre_language_object VisitLine([NotNull] LineContext context)
        {
            return Visit(context.GetChild(0));
        }

        public pre_language_object visit_binary_expression(ParserRuleContext context)
        {
            if (context.ChildCount == 1)
            {
                return Visit(context.GetChild(0));  
            }

            pre_language_object working_result = null;

            List<object> temp_data = new List<object>();

            temp_data.Add(Visit(context.GetChild(0)));

            for (int i = 1; i < context.ChildCount; i += 2)
            {
                temp_data.Add(context.GetChild(i).GetText());
                temp_data.Add(Visit(context.GetChild(i + 1)));

                if (working_result == null)
                {
                    working_result = new pre_language_object(context, temp_data.ToList(), pre_language_type.binary_operation);
                }
                else
                {
                    List<object> to_add = [working_result, temp_data[0], temp_data[1]];

                    working_result = new pre_language_object(context, to_add, pre_language_type.binary_operation);
                }

                temp_data.Clear();
            }

            return working_result;
        }

        public override pre_language_object VisitParentheses([NotNull] ParenthesesContext context)
        {
            return new pre_language_object(context,  Visit(context.expression()), pre_language_type.parentheses);
        }

        public override pre_language_object VisitLogicalOrOperation([NotNull] LogicalOrOperationContext context)
        {
            return visit_binary_expression(context);
        }

        public override pre_language_object VisitLogicalAndOperation([NotNull] LogicalAndOperationContext context)
        {
            return visit_binary_expression(context);
        }

        public override pre_language_object VisitBitwiseOrOperation([NotNull] BitwiseOrOperationContext context)
        {
            return visit_binary_expression(context);
        }

        public override pre_language_object VisitBitwiseExclusiveOrOperation([NotNull] BitwiseExclusiveOrOperationContext context)
        {
            return visit_binary_expression(context);
        }

        public override pre_language_object VisitBitwiseAndOperation([NotNull] BitwiseAndOperationContext context)
        {
            return visit_binary_expression(context);
        }

        public override pre_language_object VisitEqualityOperations([NotNull] EqualityOperationsContext context)
        {
            return visit_binary_expression(context);
        }

        public override pre_language_object VisitComparisonOperations([NotNull] ComparisonOperationsContext context)
        {
            return visit_binary_expression(context);
        }

        public override pre_language_object VisitShiftingOperations([NotNull] ShiftingOperationsContext context)
        {
            return visit_binary_expression(context);
        }

        public override pre_language_object VisitConcatOperations([NotNull] ConcatOperationsContext context)
        {
            return visit_binary_expression(context);
        }

        public override pre_language_object VisitAdditionOperations([NotNull] AdditionOperationsContext context)
        {
            return visit_binary_expression(context);
        }

        public override pre_language_object VisitMultiplicationOperations([NotNull] MultiplicationOperationsContext context)
        {
            return visit_binary_expression(context);
        }

        public override pre_language_object VisitExponentialOperations([NotNull] ExponentialOperationsContext context)
        {
            return visit_binary_expression(context);
        }

        public override pre_language_object VisitUnaryOperations([NotNull] UnaryOperationsContext context)
        {
            if (context.ChildCount == 1)
            {
                return Visit(context.GetChild(0));
            }

            string unary_operation = context.GetChild(0).GetText();
            pre_language_object value = Visit(context.GetChild(1));

            List<object> data = [unary_operation, value];

            return new pre_language_object(context, data, pre_language_type.unary_operation);
        }

        static pre_language_object create_default_arguments(IParseTree token)
        {
            return new pre_language_object(token, new List<pre_language_object>(), pre_language_type.function_arguments);
        }

        public override pre_language_object VisitBitFeild([NotNull] BitFeildContext context)
        {
            List<object> data = new List<object>();

            foreach (AdditionOperationsContext e in context.additionOperations())
            {
                data.Add(Visit(e)); 
            }

            if (context.ChildCount > 1)
            {
                string second_part = context.GetChild(1).GetText();

                if (second_part == "+" || second_part == "-")
                {
                    throw new Exception();
                }
            }

            return new pre_language_object(context, data, pre_language_type.bit_field);
        }

        public override pre_language_object VisitFunctionScriptOperations([NotNull] FunctionScriptOperationsContext context)
        {
            if (context.ChildCount == 1)
            {
                return Visit(context.GetChild(0));
            }

            switch (context.functionScriptingSecond(0).children.Last())
            {
                case FunctionArgumentsContext fac:
                    {
                        debug_tools.assert(context.functionScriptingSecond().Count() == 1);

                        List<object> data = new List<object>();

                        pre_language_object arguments;

                        if (fac.commaSeperatedExpressions() != null)
                        {
                            arguments = Visit(fac.commaSeperatedExpressions());
                        }
                        else
                        {
                            arguments = create_default_arguments(fac);
                        }

                        data.Add(Visit(context.GetChild(0)));
                        data.Add(arguments);

                        return new pre_language_object(context, data, pre_language_type.function_call);
                    }

                case PartAccessorContext pac:
                    {
                        if (pac.GetChild(0) is BitAccessorContext bac)
                        {
                            debug_tools.assert(context.functionScriptingSecond().Count() == 1);

                            List<object> data = new List<object>();

                            data.Add(Visit(context.GetChild(0)));

                            foreach (BitFeildContext b in bac.bitFeild())
                            {
                                data.Add(Visit(b));
                            }

                            return new pre_language_object(context, data, pre_language_type.bit_field_accessed_value);
                        }
                    }
                    ; break;

            }

            throw new Exception();
        }

        public override pre_language_object VisitIdentifier([NotNull] IdentifierContext context)
        {
            return new pre_language_object(context, context.GetText(), pre_language_type.identifier);
        }

        public override pre_language_object VisitIdentifierPath([NotNull] IdentifierPathContext context)
        {
            List<string> path = new List<string>();

            foreach (var identifier in context.identifier())
            {
                path.Add(identifier.GetText());
            }

            return new pre_language_object(context, path, pre_language_type.identifier_collection);
        }

        public override pre_language_object VisitCommaSeperatedExpressions([NotNull] CommaSeperatedExpressionsContext context)
        {
            List<pre_language_object> result = new List<pre_language_object>();

            foreach (var s in context.commaSeperatedExpressionSingleton())
            {
                result.Add(Visit(s));   
            }

            return new pre_language_object(context, result, pre_language_type.comma_seperated_expression);
        }

        pre_language_object visit_ternary(ParserRuleContext context)
        {
            List<object> result = new List<object>();

            for (int i = 0; i < context.children.Count; ++i)
            {
                IParseTree working_context = context.GetChild(i);

                if (working_context is not ExpressionContext e)
                    continue;

                result.Add(Visit(e));
            }

            if (result.Count != 3)
            {
                throw new Exception();
            }

            return new pre_language_object(context, result, pre_language_type.ternary);
        }

        public override pre_language_object VisitSmallTernaryOperation([NotNull] SmallTernaryOperationContext context)
        {
            return visit_ternary(context);
        }

        public override pre_language_object VisitBigTernaryOperation([NotNull] BigTernaryOperationContext context)
        {
            return visit_ternary(context);
        }

        public override pre_language_object VisitBinaryEncodingPattern([NotNull] BinaryEncodingPatternContext context)
        {
            string working = context.GetText();

            working = working.Replace("\'", "");
            working = working.Replace(" ", "");

            return new pre_language_object(context, working, pre_language_type.binary_encoding_pattern);
        }
    }
}
