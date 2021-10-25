/************************************************************************
                          ������������ ������ N2

                                ������ 2.

                               DLLCALLER.CPP

       �������� ��������� ��� ������ ������� �� DLL (����� ����������)

************************************************************************/
#include <windows.h>


typedef  LPCWSTR  (*pfn) (void);



int WINAPI WinMain (HINSTANCE, HINSTANCE, LPSTR, int)
{

   HMODULE hMod = LoadLibrary (L"testdll.dll");


   if (!hMod)
      return MessageBox (NULL, L"������ �������� testdll.dll!\n", L"������!", MB_OK | MB_ICONEXCLAMATION);


   pfn addr = (pfn)GetProcAddress (hMod, "Test");

   if (!addr)
      return MessageBox (NULL, L"������ ��������� ������ �������!\n", L"������!", MB_OK | MB_ICONEXCLAMATION);

   LPCWSTR str = (*addr)();

   if (!str)
      return MessageBox (NULL, L"�������� ������ ������!\n", L"������!", MB_OK | MB_ICONEXCLAMATION);

   MessageBox (NULL, str, L"����� �� DLL!", MB_OK | MB_ICONEXCLAMATION);

   FreeLibrary (hMod);

   return 0;    
}

