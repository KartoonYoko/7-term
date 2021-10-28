#include <windows.h>

//extern LPCWSTR Test(void);

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
    //LPCWSTR str = Test();        // Вызов функции из DLL
    //return MessageBox(NULL, str, L"Вызов из DLL!", MB_OK | MB_ICONEXCLAMATION);
    return MessageBox(NULL, L"asdasd", L"Вызов из DLL!", MB_OK | MB_ICONEXCLAMATION);
}

