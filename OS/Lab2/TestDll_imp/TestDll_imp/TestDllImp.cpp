#include <windows.h>

//extern LPCWSTR Test(void);

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
    //LPCWSTR str = Test();        // ����� ������� �� DLL
    //return MessageBox(NULL, str, L"����� �� DLL!", MB_OK | MB_ICONEXCLAMATION);
    return MessageBox(NULL, L"asdasd", L"����� �� DLL!", MB_OK | MB_ICONEXCLAMATION);
}

