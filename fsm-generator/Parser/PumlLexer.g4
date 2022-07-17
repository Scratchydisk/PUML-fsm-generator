/*
 * PUML STATE MODEL LEXER
 *
 * ANTLR Lexer for parsing PUML formatted state diagrams.
 *
 * Scratchydisk 21/9/19
 * 
 */

lexer grammar PumlLexer;

fragment TRANSITION_DIRECTION
    : [lrud]
    ;

START
    : '[*]'
    ;

STARTUML
    : '@startuml' -> pushMode(START_STATEMENT)
    ;

ENDUML
    : '@enduml'
    ;

FORK
    : '<<fork>>'
    ;

JOIN
    : '<<join>>'
    ;

LEGEND 
    : 'legend' NEWLINE -> pushMode(INSIDE_TEXTBLOCK)
    ;

STATE
    : 'state'
    ;

TITLE 
    : 'title' ->pushMode(INSIDE_TEXTBLOCK)
    ;

fragment CONTENT_BLOCK
    : ANY*?
    ;

TRANSITION
    : '-->'
    | '-' TRANSITION_DIRECTION '->'
    ;

// Reads comment line, except trailing \r\n
COMMENT_LINE
	: '\'' ~( '\r' | '\n' )* -> pushMode(END_COMMENT_LINE)
	;

START_EVENT
    : ':' -> pushMode(PARSING_EVENT)
    ;

OPEN_CURLY
    : '{'
    ;

CLOSE_CURLY
    : '}'
    ;
	
QUOTE
	: '"'
	;

DOUBLE_QUOTE
	: '""'
	;

SLASH
    : '/'
    ;


SQUARE_OPEN
    : '['
    ;

SQUARE_CLOSE
    : ']'
    ;

INTEGER
    : ('0' .. '9')+
    ;


IDENTIFIER
   : ('a' .. 'z' | 'A' .. 'Z') ('a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '_' | '-' )*
   ;

TEXT
    // Note "\" escaped and '-' has to be last if not part of a range
    : ( 'a'..'z' | 'A'..'Z' | '0'..'9' | [_./,!"£$%^&*()+=-?#~@;:\\|\t-] )+
    ;


NEWLINE             
    : ('\r'? '\n' | '\r') 
    ;

WS
    : ([ \t] | NEWLINE )+ -> skip
	;

// If this shows up then there's an error!
ANY
    : .
    ;

//---------------------------------------------------------
mode START_STATEMENT;

// IDENTIFIER because this becomes the namespace name
DIAGRAM_NAME
    : IDENTIFIER+ ( ' ' | IDENTIFIER )* -> popMode
    ;

// END_DIAGRAM_NAME
//     : NEWLINE -> skip, popMode
//     ;

WS_S
	: [ \r\n\t]+? -> skip
	;

// If this shows up then there's an error!
ANY_S : .;


//---------------------------------------------------------
mode INSIDE_TEXTBLOCK;

ENDTITLE 
    : 'end title' -> popMode
    ;

ENDLEGEND 
    : 'end legend' -> popMode
    ;

// Empty line detection isn't 100% but it's enough
// to pass blank lines in the PUML diagram as blank
// lines to the parser.
// It does the following:
// * 1 blank line -> 1 blank line
// * > 1 blank line -> (n-1)/2 blank lines passed
TEXTLINE
    : ~[\r\n]+
    | NEWLINE NEWLINE NEWLINE  // Empty line
    ;

WS_TB
    : [\r\n]+? -> skip
    ;

// If this shows up then there's an error!
ANY_TB : .;

//---------------------------------------------------------
mode END_COMMENT_LINE;

// Need to consume newline at end of line comment
END_COMMENT
    : [ \r\n\t]+ -> skip, popMode
    ;

//---------------------------------------------------------
mode PARSING_EVENT;

GUARD_START
    : SQUARE_OPEN -> pushMode(PARSING_GUARD)
    ;
    
END_EVENT
    : SLASH -> pushMode(PARSING_ACTION)
    ;

EVENT_TIMEOUT
    : 'Timeout'
    ;

EVENT_NAME
    : IDENTIFIER 
    ;

END_PE
    : NEWLINE+ -> skip, popMode
    ;

WS_PE
	: [ \t]+ -> skip
	;

//---------------------------------------------------------
 mode PARSING_GUARD;

GUARD_END
    : SQUARE_CLOSE -> popMode
    ;

TIMEOUT_MS
    : INTEGER+
    ;

GUARD_CONTENT
    : TEXT+?
    ;

//---------------------------------------------------------
mode PARSING_ACTION;

ACTION_TEXT
    : IDENTIFIER+ ( ' ' | TEXT )* -> popMode
    ;
    
SPACE_PA 
    : [ ]+ -> skip
    ;

// This terminates action and event name parsing
WS_PA : ([\t] | NEWLINE)+ -> skip, popMode, popMode;

// If this shows up then there's an error!
ANY_PA : .;

