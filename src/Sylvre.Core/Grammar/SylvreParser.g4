/*
 * Sylvre parser grammar
 * Shahzaib Mirani
 */

parser grammar SylvreParser;
options { tokenVocab=SylvreLexer; }

program     : block+ EOF
            ;

seperator   : HASH
            ;

block       : use_file_statement
            | function_block
            | if_block elseif_block* else_block?
            | loopwhile_block
            | loopfor_block
            | statement_block
            ;

nestable_block  : if_block elseif_block* else_block?
                | loopwhile_block
                | loopfor_block
                | statement_block
                ;

use_file_statement  : USE FILE ( SINGLE_STRING | DOUBLE_STRING ) seperator
                    ;

statement_block : statement seperator;

function_block  : FUNCTION variable_reference (PARAMS parameters)?
                    LEFT_ANGLE_BRACKET nestable_block* RIGHT_ANGLE_BRACKET
                ;

if_block        : IF OPEN_PARENTHESIS conditional_expression CLOSE_PARENTHESIS
                    LEFT_ANGLE_BRACKET nestable_block* RIGHT_ANGLE_BRACKET
                ;
elseif_block    : ELSEIF OPEN_PARENTHESIS conditional_expression CLOSE_PARENTHESIS
                    LEFT_ANGLE_BRACKET nestable_block* RIGHT_ANGLE_BRACKET
                ;
else_block      : ELSE 
                    LEFT_ANGLE_BRACKET nestable_block* RIGHT_ANGLE_BRACKET
                ;

loopwhile_block : LOOPWHILE OPEN_PARENTHESIS
                  conditional_expression
                  CLOSE_PARENTHESIS
                    LEFT_ANGLE_BRACKET nestable_block* RIGHT_ANGLE_BRACKET
                ;
loopfor_block   : LOOPFOR OPEN_PARENTHESIS 
                  ( declaration | assignment )    seperator
                  conditional_expression          seperator
                  ( assignment | expression )     seperator?
                  CLOSE_PARENTHESIS
                    LEFT_ANGLE_BRACKET nestable_block* RIGHT_ANGLE_BRACKET 
                ;

conditional_expression  : OPEN_PARENTHESIS conditional_expression CLOSE_PARENTHESIS                             //# parentheses_expression
                        | NOT conditional_expression                                                    //# not_expression
                        | left=conditional_expression comparison_operator right=conditional_expression  //# comparator_expression
                        | left=conditional_expression logical_operator    right=conditional_expression  //# binary_expression
                        | expression                                                                    //# evaluation_expression
                        ;

comparison_operator : GREATER_THAN | GREATER_EQUAL | LESS_THAN | LESS_EQUAL | EQUALS
                    ;
logical_operator    : AND | OR
                    ;       

bool    : TRUE | FALSE
        ;                     

parameters  : variable_reference (COMMA variable_reference)*
            ;
arguments   : conditional_expression (COMMA conditional_expression)*
            ;

statement   : declaration
            | assignment
            | function_call
            | function_return
            | unary_increment
            | unary_decrement
            ;

declaration : CREATE variable_reference assignment_operator conditional_expression
            | CREATE variable_reference assignment_operator array_assignment
            ;  
assignment  : variable_complex_reference_left assignment_operator conditional_expression
            | variable_complex_reference_left assignment_operator array_assignment
            ;       

assignment_operator : EQUALSYMBOL
                    | PLUSEQUALS
                    | MINUSQUALS
                    | MULTIPLYEQUALS
                    | DIVIDEEQUALS
                    ;

array_assignment    : OPEN_SQUARE_BRACKET array_elements? CLOSE_SQUARE_BRACKET
                    ;
array_elements      : expression (COMMA expression)*
                    ;

function_call           : CALL variable_complex_reference OPEN_PARENTHESIS arguments? CLOSE_PARENTHESIS
                        ;
function_return         : EXIT function_return_value?
                        ;
function_return_value   : WITH conditional_expression
                        ;

expression  : term arithmetic_operator expression | term
            ;
term        : factor arithmetic_operator term | factor 
            ;
factor      : OPEN_PARENTHESIS expression CLOSE_PARENTHESIS | (MINUS)? NUMBER | MINUS? DECIMAL 
                | string | variable_complex_reference | function_call | bool | unary_increment | unary_decrement
            ;

arithmetic_operator : PLUS | MINUS | MULTIPLY | DIVIDE
                    ;

string  : DOUBLE_STRING
        | SINGLE_STRING
        | TEMPLATE_LITERAL
        ; 

variable_reference               : IDENTIFIER
                                 ;
variable_complex_reference      : variable_reference variable_suffix*
                                ;
variable_complex_reference_left : variable_reference variable_suffix_left*
                                ;

variable_suffix         : member_reference | index_reference
                        ;
variable_suffix_left    : member_reference_left | index_reference
                        ;
member_reference        : FULLSTOP ( variable_reference | function_call )
                        ;
member_reference_left   : FULLSTOP variable_reference
                        ;
index_reference         : OPEN_SQUARE_BRACKET expression CLOSE_SQUARE_BRACKET
                        ;


unary_increment : unary_prefix_increment | unary_suffix_increment
                ;
unary_decrement : unary_prefix_decrement | unary_suffix_decrement
                ;        

unary_prefix_increment : INCREMENT variable_complex_reference
                       ;
unary_prefix_decrement : DECREMENT variable_complex_reference
                       ;
unary_suffix_increment : variable_complex_reference INCREMENT
                       ;
unary_suffix_decrement : variable_complex_reference DECREMENT
                       ;