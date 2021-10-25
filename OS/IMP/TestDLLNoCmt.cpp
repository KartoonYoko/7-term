/************************************************************************
                          Лабораторная работа N2

                                Пример 1.

                               TESTDLL.CPP

    Пример библиотеки DLL, экспортирующей функцию  при неявном связывании

************************************************************************/
#include <windows.h>


LPCWSTR GetSomeString ()
{                       
   return L"Hello from DLL!\n";
}


LPCWSTR  __declspec (dllexport) Test (void)
{
   return GetSomeString ();
}


BOOL WINAPI DllEntryPoint (HINSTANCE, DWORD, DWORD)
{
    return 1;    
}

