%{
#include "y.tab.h"
#include <stdio.h>
#include <string.h>
#define SIZE 2
char* keywords[SIZE] = {"for", "do"};
int resWord(char*);
%}

%option yylineno
%option noyywrap


letter [a-zA-Z]
digit [0-9]
ws [ \t\n\r]

%%

({letter}|"_")({letter}|{digit}|"_")* {
    //yylval.string = yytext;
    if(yytext == "for")
    {
        return FOR;
    }else{
        if (yytext == "do"){
            return DO;
        }
        else{
            return IDENTIFIER;
        }
    }  
}
[(] {
    return DELIML;
}
[)] {
    return DELIMR;
}
{digit}+ {
    yylval = atoi(yytext);
    return NUM;
}
{ws}+ { }
. { 
    printf("Unrecognized token %s\n", yytext);
    return *yytext; 
}
%%


// YYSTYPE yylval;

// int main(void)
// {
//     int token;
//     while ((token = yylex()) != 0)
//         printf("Token: %d '%s'\n", token, yytext);
//     return 0;
// }