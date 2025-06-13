#include <windows.h>
#include "MainForm.h"

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow) {
    MainForm form;
    
    if (!form.Initialize(hInstance)) {
        MessageBoxA(NULL, "Failed to initialize application", "Error", MB_OK | MB_ICONERROR);
        return 1;
    }
    
    form.Show();
    
    MSG msg;
    while (GetMessage(&msg, NULL, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }
    
    return (int)msg.wParam;
}