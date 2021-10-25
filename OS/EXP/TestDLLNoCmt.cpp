/************************************************************************
                          Лабораторная работа N2

                                Пример 2.

                               TESTDLL.CPP

     Пример библиотеки DLL, экспортирующей функцию при явном связывании

************************************************************************/
#include <windows.h>


LPCWSTR GetSomeString ()       
{                            
   return (LPCWSTR)(L"Hello from DLL!\n");
}


extern "C" LPCWSTR  __declspec (dllexport) Test (void)
{
   return GetSomeString ();
}


BOOL WINAPI DllEntryPoint (HINSTANCE, DWORD, DWORD)
{
    return 1;    
}

