grammar asl;

options
{
    language=CSharp;
}

constantTest
    : constant* EOF
    ;

expressionTest
    : expression* EOF
    ;

lineTest
    : line* EOF
    ;

sourceFile
    : line* EOF
    ;

//Lines
lValueSet
    : 'constant'? expression '=' expression
    ;

variableDeclaration
    : type '&'? identifier (',' identifier)* ('=' expression)?
    ;

line
    : ( 
        lValueSet | 
        variableDeclaration |
        returnStatement |
        assertStatement | 
        enumerationDeclaration |
        arrayDeclaration |
        hardStatements
        ) ';'
    | controlFlowStatements
    | explicitFunctionDeclaration
    | typeDeclaration
    | expression ';'?
    ;

assertStatement
    : 'assert' expression
    ;

controlFlowStatements
    : ifStatement
    | elseStatement
    | elseIfStatement
    | whileStatement
    | caseStatement
    | whenStatement
    | forLoop
    ;

hardStatements
    : 'IMPLEMENTATION_DEFINED'
    | 'UNDEFINED'
    ;

enumerationDeclaration
    : 'enumeration' identifier '{' identifier (',' identifier)* ','? '}'
    ;

typeMember
    : variableDeclaration
    | typeArrayDeclaration
    ;

typeArrayDeclaration
    : 'array' numberRange 'of' type identifier
    ;

typeDeclaration
    : newTypeDeclaration
    | oldTypeRedeclaration
    | emptyTypeDeclaration
    ;

emptyTypeDeclaration
    : 'type' identifier ';'
    ;

newTypeDeclaration
    : 'type' identifier 'is' '(' typeMember (',' typeMember)* ','? ')'
    ;

oldTypeRedeclaration
    : 'type' identifier '=' type ';'
    ;

forLoop
    : 'for' lValueSet ('to' | 'downto') expression 
    ;

caseStatement
    : 'case' expression 'of'
    ;

whenStatement
    : 'when' expression (',' expression)*
    ;

ifStatement
    : 'if' expression 'then'
    ;

elseStatement
    : 'else'
    ;

elseIfStatement
    : 'elsif' expression 'then'
    ;

returnStatement
    : 'return' expression?
    ;

whileStatement
    : 'while' expression 'do'
    ;

explicitFunctionDeclaration
    : normalExplicitFunctionDeclaration
    | setExplicitFunctionDeclaration
    ;

setExplicitFunctionDeclaration
    : functionScriptOperations '=' variableDeclaration ';'?
    ;

normalExplicitFunctionDeclaration
    : type functionScriptOperations ';'?
    ;

arrayDeclaration
    : 'array' type identifier numberRange
    | 'array' numberRange 'of' type identifier
    ;

//Expression
commaSeperatedExpressionSingleton
    : expression
    | variableDeclaration
    ;

commaSeperatedExpressions
    : commaSeperatedExpressionSingleton (',' commaSeperatedExpressionSingleton)*
    ;

functionArguments
    : '(' commaSeperatedExpressions? ')'
    | '[' commaSeperatedExpressions? ']'
    ;

parentheses
    : '(' expression ')'
    ;

edgeCases
    : type 
        (
            'IMPLEMENTATION_DEFINED' |
            'UNKNOWN'
        )
        STRING?
    ;

bitAccessor
    : '<' bitFeild (',' bitFeild)* '>'
    ;

bitFeild
    : additionOperations (('+' | '-')? ':' additionOperations)?
    ;

structAccessor
    : '.' '<' identifier (',' identifier)* '>'
    ;

partAccessor
    : bitAccessor
    | structAccessor
    ;

baseExpression
    : constant
    | identifier
    | parentheses
    | tuple
    | edgeCases
    ;

numberRange
    : '[' expression '..' expression ']'
    | '{' expression '..' expression '}'
    ;

functionScriptOperations
    : baseExpression (functionArguments | ('.' functionScriptOperations) | ('IN' collection) | partAccessor)*
    ;

unaryOperations
    : ('!' | '-' | '&' | 'NOT')* functionScriptOperations
    ;

exponentialOperations
    : unaryOperations ('^' unaryOperations)*
    ;

multiplicationOperations
    : exponentialOperations (('*' | '/' | 'DIV' | 'MOD') exponentialOperations)*
    ;

additionOperations
    : multiplicationOperations (('+' | '-') multiplicationOperations)*
    ;

concatOperations
    : additionOperations (':' additionOperations)*
    ;

shiftingOperations
    : concatOperations (('<<' | '>>') concatOperations)*
    ;

comparisonOperations
    : shiftingOperations (('<' | '<=' | '>' | '>=') shiftingOperations)*
    ;

equalityOperations
    : comparisonOperations (('!=' | '==') comparisonOperations)*
    ;

bitwiseAndOperation
    : equalityOperations ('AND' equalityOperations)*
    ; 

bitwiseExclusiveOrOperation
    : bitwiseAndOperation ('EOR' bitwiseAndOperation)*
    ; 

bitwiseOrOperation
    : bitwiseExclusiveOrOperation ('OR' bitwiseExclusiveOrOperation)*
    ; 

logicalAndOperation
    : bitwiseOrOperation ('&&' bitwiseOrOperation)*
    ; 

logicalOrOperation
    : logicalAndOperation ('||' logicalAndOperation)*
    ;

smallTernaryOperation
    : 'if' expression 'then' expression 'else' expression
    ;

bigTernaryOperation
    : 'if' expression 'then' expression ('elsif' expression 'then' expression)* 'else' expression
    ;

teranryOperation
    : bigTernaryOperation
    | smallTernaryOperation
    ;

expression
    : logicalOrOperation
    | teranryOperation
    ;

tupleSingle
    : expression
    | '-'
    ;

tuple
    : '(' tupleSingle (',' tupleSingle)+ ')'
    ;

collection
    : '{' expression (',' expression)* '}'
    | numberRange
    ;

//IDENTIFIER
identifier
    : IDENTIFIER
    ;

IDENTIFIER
    : [a-zA-Z_][a-zA-Z0-9_]*
    ;

//TYPES
type
    : concreteTypes
    | dynamicTypes
    | identifier
    | constantType
    | tupleType
    ;

tupleType
    : '(' type (',' type)+ ')'
    ;

constantType
    : 'constant' type?
    ;

concreteTypes
    : 'boolean'
    | 'integer'
    | 'bit'
    | 'real'
    ;

dynamicTypes
    : 'bits' '(' expression ')'
    ;

//CONSTANTS
constant
    : DECIMAL_NUMBER
    | HEX_NUMBER
    | BINARY_NUMBER
    | TRUE_FALSE
    | BITS
    ;

DECIMAL_NUMBER
    : [0-9]+ ('.' [0-9]+)?
    ;

HEX_NUMBER
    : '0x' [0-9a-fA-F_]+
    ;

BINARY_NUMBER
    : '0b' [0-1]+
    ;

TRUE_FALSE
    : 'TRUE'
    | 'FALSE'
    ;

STRING
    :  '"' ( ~[\\"] )* '"'
    ;

BITS
    : '\'' [0-1x ]+ '\''
    ;

WHITE_SPACE
    : [ \n\t\r] -> skip
    ;

LINE_COMMENT
    : '//' ~[\r\n]* -> skip
    ;

BLOB_COMMENT
    : '/*' .*? '*/' -> skip
    ;