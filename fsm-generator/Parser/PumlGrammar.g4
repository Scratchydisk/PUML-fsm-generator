/*
 * PUML STATE MODEL GRAMMAR
 *
 * ANTLR Grammar for parsing PUML formatted state diagrams.
 *
 * Scratchydisk 21/9/19
 * 
 * KNOWN ISSUES:
 *  - There must be a blank line after the @startuml statement
 *
 */

parser grammar PumlGrammar;

options { tokenVocab=PumlLexer; }
   
stateModel
    : startStatement line+ endStatement
    ;

// Diagram name is mandatory because it becomes the namespace
startStatement
    : STARTUML diagramName 
    ;

endStatement
    : ENDUML
    ;

diagramName
    : DIAGRAM_NAME 
    ;

line
    : stateTransition
    | stateInternalEvent
    | state
    | metaState
    | titleBlock
    | legendBlock
    | comment               // Passes comments to parser
    ;
    
// The newline is needed if there's no action and no whitespace after the toState
stateTransition
    : fromState transition toState ( | eventAction | NEWLINE )
    ;

fromState : state;
toState : state;

state
    : startState
    | (STATE | ) IDENTIFIER ( | FORK | JOIN )
    ;

titleBlock
    : TITLE textBlock ENDTITLE
    ;

legendBlock
    : LEGEND textBlock ENDLEGEND
    ;

textBlock
    : TEXTLINE*?
    ;

comment
    : COMMENT_LINE 
    ;
    
stateInternalEvent
    : state eventAction
    ;

eventAction
    : START_EVENT event END_EVENT action
    ;

metaState
    : state OPEN_CURLY line*? CLOSE_CURLY
    ;

startState
    : START
    ;

transition
    : TRANSITION
    ; 

// event and action are multiple words, not identifiers
event
    : EVENT_TIMEOUT timeoutGuard
    | EVENT_NAME ( | guard)
    ;
    
timeoutGuard
    : guard
    ;

guard
    : GUARD_START (TIMEOUT_MS | GUARD_CONTENT) GUARD_END
    ;

action
    : ACTION_TEXT
    |
    ;
    