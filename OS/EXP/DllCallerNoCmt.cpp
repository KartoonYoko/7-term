/************************************************************************
                          Лабораторная работа N2

                                Пример 2.

                               DLLCALLER.CPP

       Тестовая программа для вызова функции из DLL (явное связывание)

************************************************************************/
#include <windows.h>


typedef  LPCWSTR  (*pfn) (void);



int WINAPI WinMain (HINSTANCE, HINSTANCE, LPSTR, int)
{

   HMODULE hMod = LoadLibrary (L"testdll.dll");


   if (!hMod)
      return MessageBox (NULL, L"Ошибка загрузки testdll.dll!\n", L"ОШИБКА!", MB_OK | MB_ICONEXCLAMATION);


   pfn addr = (pfn)GetProcAddress (hMod, "Test");

   if (!addr)
      return MessageBox (NULL, L"Ошибка получения адреса функции!\n", L"ОШИБКА!", MB_OK | MB_ICONEXCLAMATION);

   LPCWSTR str = (*addr)();

   if (!str)
      return MessageBox (NULL, L"Получена пустая строка!\n", L"ОШИБКА!", MB_OK | MB_ICONEXCLAMATION);

   MessageBox (NULL, str, L"Вызов из DLL!", MB_OK | MB_ICONEXCLAMATION);

   FreeLibrary (hMod);

   return 0;    
}

