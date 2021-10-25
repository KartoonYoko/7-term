/************************************************************************
                          ������������ ������ N2

                                ������ 2.

                               TESTDLL.CPP

     ������ ���������� DLL, �������������� ������� ��� ����� ����������

************************************************************************/
#include <windows.h>

/* ������� GetSomeString() �������� ���������� �������� DLL � �� ��������������.
   ����� �������, ������ ���������� �� ����� ������� ������� � �� ����. ���
   �������, �������� DLL �������� ��������� ����������  ���������� �������,
   ����������� �������� ������, � ����� ��������� ��������������  �������,
   ������� ��������� ���� ���������� ����� DLL � ������������. ��� ����
   �������������� ������� ����� �������� ���������� ������� DLL ��� ��������
   � ������ �������.
*/

LPCWSTR GetSomeString ()				        // ������� GetSomeString () ������	
{								// ���������� ������ ������       
   return (LPCWSTR)(L"Hello from DLL!\n");
}


/* ������� Test() �������� ��������������, �.�. �������� ������ �����������.
   ��� ����� ��� ������ ���� ������� �� ����������� �������������, �������
   ������������ ������������ ������� �����������. � ������ ������ -
   __declspec (dllexport). ��� ������������� ����� ������������ ������� �����
   �������� � ������� �������� DLL � ������ �������� ������� �����������.
   �������� ������� � ���������� ��������� ������ ��������������� �� ��������
   � ���� DLL, �� ����������� ������������ __declspec (dllexport).

   ������������ ������ ���������� �������� ��, ��� � ���������� ���������
   ������ ���� �������� ��������� ��� ������� � DLL. ����������� �++ ����������
   �.�. ���������� ���� (name mangling), ��������������� ��� ���������
   ������������� �������. ��� ���������� ���� ���������� ��� ������� ����������
   �� ����� � ������ ���������, ��������� � ���� ������������ �����������
   ������� ����������. ����� �������, ������� ������� ������� �� DLL �� �����
   ����� ��������� (��� �������, ���������� ���������� ����������� � ���������
   ��� ������� ����� ����������). ���� �� �������� ������ ������ �������� - 
   ���������� ���������� ����. ��� ����������� �������������� ������������ 
   ������������ EXTERN "C". ��� ���� ��������� ��� ������� ����� ��������� 
   � ������ � ������ ���������.
*/

extern "C" LPCWSTR  __declspec (dllexport) Test (void)
{
   return GetSomeString ();
}

// ������� DllEntryPoint �������� ����������� ������ ����� ��� DLL, �.�.
// �������� WinMain ��� ���������� (��� ��� ����� ���������� DllMain). 
// ��� �������, ��� ����������� ���������� �����  - � �������, ����� 
// ��������� ��������� �����-�� ����������������� �������� ��� �������� DLL,
// ���������  DllEntryPoint ����������� ������������� ��� �������� DLL.

BOOL WINAPI DllEntryPoint (HINSTANCE, DWORD, DWORD)
{
    return 1;				// DllEntryPoint ������ ���������� TRUE � ������ �� ������
}

