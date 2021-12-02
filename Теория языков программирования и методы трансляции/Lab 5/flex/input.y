%{
    #include <stdio.h>
    int yylex(void);
    void yyerror(char *s) {
      fprintf (stderr, "%s\n", s);
    }
%}

%token NUM
%token FOR
%token DO
%token IDENTIFIER
%token DELIML
%token DELIMR


%%

PROGRAM: FOR DELIML IDENTIFIER DELIMR DO {  }
        |
        IDENTIFIER {};
%%

int main()
{
   yyparse();
   return 0;
}