#include <windows.h>


LPCWSTR GetSomeString()          // ������� GetSomeString () ������
{                               // ���������� ������ ������
    return L"Hello from DLL!\n";
}


LPCWSTR  __declspec (dllexport) Test(void)
{
    return GetSomeString();
}

BOOL WINAPI DllEntryPoint(HINSTANCE, DWORD, DWORD)
{
    return 1;    // DllEntryPoint ������ ���������� TRUE � ������ �� ������
}

