%option noyywrap yylineno
%{
#include <stdio.h>
#include <string.h>
#define ANSI_COLOR_RED     "\x1b[31m"
#define ANSI_COLOR_GREEN   "\x1b[32m"
#define ANSI_COLOR_YELLOW  "\x1b[33m"
#define ANSI_COLOR_BLUE    "\x1b[34m"
#define ANSI_COLOR_RESET   "\x1b[0m"
#define SIZEOP 5
#define SIZECONST 2
char* filename;
char* logic_operations[SIZEOP] = {"or", "xor", "and", "not", "="};
char* logic_constant[SIZECONST] = {"true", "false"};

int resWord(char* id);
int isLogicConst(char* str);
%}
letter [a-zA-Z]
digit [0-9]
delim [();]
ws [ \t\n]
%%
({letter}|{digit}|"=")({letter}|{digit}|"_")* {
        if(resWord(yytext))
        {
            printf("%s:%d LOGIC OPERATION %s\n", filename, yylineno, yytext);
        }else{
            if(isLogicConst(yytext)){
                printf("%s:%d LOGIC CONST %s\n", filename, yylineno, yytext);
            }
            else{
                printf("%s:%d IDENTIFIER %s\n", filename, yylineno, yytext);
            }
        }
    }
\"({letter}|(" ")+)*\" {
    printf("%s:%d STRING CONST %s\n", filename, yylineno, yytext);
}
":=" {
    printf("%s:%d ASSIGN %s\n", filename, yylineno, yytext);
}
{delim} {
    printf("%s:%d DELIMITER %s\n", filename, yylineno, yytext);
}
{ws}+ ;
. {
    printf(ANSI_COLOR_RED "%s:%d Unknown character '%s'\n" ANSI_COLOR_RESET, filename, yylineno, yytext );
}
%%
// 1 - если ключевое слово, иначе - 0
int resWord(char* id)
{
    int i;
    for(i = 0; i < SIZEOP; i++)
    {
        if(strcmp(id, logic_operations[i]) == 0)
        {
            return 1;
        }
    }
    return 0;
}

int isLogicConst(char* str){
    int i;
    for(i = 0; i < SIZECONST; i++)
    {
        if(strcmp(str, logic_constant[i]) == 0)
        {
            return 1;
        }
    }
    return 0;
}

int main(int argc, char** argv)
{
    if(argc < 2)
    {
        perror("Input file name is not specified");
        return 1;
    }
    yyin = fopen(argv[1], "r");
    if(yyin == NULL)
    {
        perror(argv[1]);
        return 1;
    }
    filename = strdup(argv[1]);
    yylineno = 1;
    yylex();
    return 0;
}
