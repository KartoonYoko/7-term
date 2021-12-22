#include <iostream>

int main()
{
    int qwe;        // ANNOUNCE has DATA_TYPE and IDEN
    qwe = 1;        // INIT has IDEN and ASSIGN and INT
    int a = 132;    // ANNOUNCE && INIT
    int c = 0;      // ANNOUNCE && INIT

    for (int i = 0; i < a; i++) {   // FOR (ANNOUNCE && INIT ; COMPARE ; INIT)
        c += i;                     // ANNOUNCE && INIT ;
    }
    
    std::cout << c << std::endl;
}
