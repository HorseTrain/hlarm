using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.pseudocode.pre_language
{
    public enum pre_language_type
    {
        undefined,
        instruction_declaration,
        function_declaration,
        variable_declaration,
        binary_operation,
        unary_operation,
        function_call,
        function_arguments,
        identifier,
        identifier_collection,
        comma_seperated_expression,
        source_file,
        ternary,
        binary_encoding_pattern,
        if_statment,
        else_statement,
        else_if_statement,
        l_value_set,
        return_statement,
        tuple,
        empty_expression,
        lined_expression,
        bit_field,
        bit_field_accessed_value,
        parentheses,
        concrete_type,
        dynamic_type,
        tuple_type,
        scope,
        constant
    }
}

