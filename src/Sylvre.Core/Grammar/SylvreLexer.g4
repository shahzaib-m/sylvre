/*
 * Sylvre lexer grammar
 * Shahzaib Mirani
 */

lexer grammar SylvreLexer;

DOUBLE_STRING       : '"'  ~('"' )* '"'  ;
SINGLE_STRING       : '\'' ~('\'')* '\'' ;
TEMPLATE_LITERAL    : '`'  ~('`' )* '`'  ;

HASH                : '#'   ;
LEFT_ANGLE_BRACKET  : '<'   ;
RIGHT_ANGLE_BRACKET : '>'   ;
COMMA               : ','   ;
FULLSTOP            : '.'   ;

OPEN_PARENTHESIS    : '(' ;
CLOSE_PARENTHESIS   : ')' ;

OPEN_SQUARE_BRACKET     : '[' ;
CLOSE_SQUARE_BRACKET    : ']' ;

PLUS        : '+'  ;
PLUSEQUALS  : '+=' ;
INCREMENT   : 'increment' ;


MINUS       : '-'  ;
MINUSQUALS  : '-=' ;
DECREMENT   : 'decrement' ;

MULTIPLY        : '*'  ;
MULTIPLYEQUALS  : '*=' ;

DIVIDE          : '/'  ;
DIVIDEDIVIDE    : '//' ;
DIVIDEEQUALS    : '/=' ;

EQUALSYMBOL     : '='  ;

USE     : 'USE'  ;
FILE    : 'FILE' ;

FUNCTION    : 'function' ;
PARAMS      : 'PARAMS'   ;

CREATE      : 'create'  ;
CALL        : 'call'    ;
EXIT        : 'exit' ;
WITH        : 'with' ;

IF          : 'if' ;
ELSEIF      : 'elseif' ;
ELSE        : 'else' ;
LOOPWHILE   : 'loopwhile' ;
LOOPFOR     : 'loopfor'  ;

AND         : 'AND' ;
OR          : 'OR' ;
NOT         : 'NOT'  ;
TRUE        : 'TRUE'  ;
FALSE       : 'FALSE' ;

GREATER_THAN    : 'GTHAN' ;
GREATER_EQUAL   : 'GEQUAL' ;
LESS_THAN       : 'LTHAN' ;
LESS_EQUAL      : 'LEQUAL' ;
EQUALS          : 'EQUALS' ;

IDENTIFIER : [A-Za-z] [A-Za-z0-9_]* ;

fragment Digit  : [0-9]  ;
NUMBER          : Digit+ ;
DECIMAL         : Digit+ '.' Digit+ ;


WS              : [ \t\r\n]+ -> channel(HIDDEN) ;
COMMENT         : '/*' .*? '*/' -> skip ;
LINE_COMMENT    : '//' ~[\r\n]* -> skip ;
NEWLINE         : '\r'? '\n' -> skip ;
