#include <windows.h>


LPCWSTR GetSomeString()          // Функция GetSomeString () просто
{                               // возвращает строку текста
    return L"Hello from DLL!\n";
}


LPCWSTR  __declspec (dllexport) Test(void)
{
    return GetSomeString();
}

BOOL WINAPI DllEntryPoint(HINSTANCE, DWORD, DWORD)
{
    return 1;    // DllEntryPoint просто возвращает TRUE и ничего не делает
}

