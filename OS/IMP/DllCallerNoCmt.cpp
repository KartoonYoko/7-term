/************************************************************************
                          Лабораторная работа N2

                                Пример 1.

                               DLLCALLER.CPP

       Тестовая программа для вызова функции из DLL (неявное связывание)

************************************************************************/
#include <windows.h>

extern LPCWSTR Test (void);

int WINAPI WinMain (HINSTANCE, HINSTANCE, LPSTR, int)
{
    LPCWSTR str = Test ();        
    return MessageBox (NULL, str, L"Вызов из DLL!", MB_OK | MB_ICONEXCLAMATION);
}

