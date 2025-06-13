#include "MainForm.h"
#include "UpdateHandler.h"
#include "Version.h"
#include "Constants.h"
#include <commctrl.h>

MainForm::MainForm() : hwnd(nullptr), updateHandler(nullptr), isUpdating(false) {}

MainForm::~MainForm() {
    delete updateHandler;
}

bool MainForm::Initialize(HINSTANCE hInstance) {
    // Initialize common controls
    INITCOMMONCONTROLSEX icex;
    icex.dwSize = sizeof(INITCOMMONCONTROLSEX);
    icex.dwICC = ICC_PROGRESS_CLASS;
    InitCommonControlsEx(&icex);
    
    // Register window class
    WNDCLASSEX wc = {};
    wc.cbSize = sizeof(WNDCLASSEX);
    wc.lpfnWndProc = WindowProc;
    wc.hInstance = hInstance;
    wc.lpszClassName = L"UpdaterMainWindow";
    wc.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
    wc.hCursor = LoadCursor(NULL, IDC_ARROW);
    wc.hIcon = LoadIcon(NULL, IDI_APPLICATION);
    
    if (!RegisterClassEx(&wc)) return false;
    
    // Create main window
    hwnd = CreateWindowEx(
        0,
        L"UpdaterMainWindow",
        L"Auto-Updater Application",
        WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT, CW_USEDEFAULT, 500, 400,
        NULL, NULL, hInstance, this
    );
    
    if (!hwnd) return false;
    
    // Create controls
    lblCurrentVersion = CreateWindow(L"STATIC", L"Current Version: 1.0.0.0",
        WS_VISIBLE | WS_CHILD, 20, 20, 300, 25, hwnd, (HMENU)ID_LABEL_CURRENT_VERSION, hInstance, NULL);
    
    lblLatestVersion = CreateWindow(L"STATIC", L"Latest Version: Checking...",
        WS_VISIBLE | WS_CHILD, 20, 50, 300, 25, hwnd, (HMENU)ID_LABEL_LATEST_VERSION, hInstance, NULL);
    
    lblStatus = CreateWindow(L"STATIC", L"Checking for updates...",
        WS_VISIBLE | WS_CHILD, 20, 80, 400, 25, hwnd, (HMENU)ID_LABEL_STATUS, hInstance, NULL);
    
    btnUpdateNow = CreateWindow(L"BUTTON", L"Update Now",
        WS_VISIBLE | WS_CHILD | BS_PUSHBUTTON, 20, 120, 100, 30, 
        hwnd, (HMENU)ID_BUTTON_UPDATE_NOW, hInstance, NULL);
    
    // Progress panel (initially hidden)
    progressBar = CreateWindow(PROGRESS_CLASS, NULL,
        WS_CHILD | PBS_SMOOTH, 20, 170, 400, 25, 
        hwnd, (HMENU)ID_PROGRESS_BAR, hInstance, NULL);
    
    lblPercentage = CreateWindow(L"STATIC", L"0%",
        WS_CHILD, 430, 170, 50, 25, hwnd, (HMENU)ID_LABEL_PERCENTAGE, hInstance, NULL);
    
    btnUpdateCancel = CreateWindow(L"BUTTON", L"Cancel",
        WS_CHILD | BS_PUSHBUTTON, 20, 210, 100, 30, 
        hwnd, (HMENU)ID_BUTTON_UPDATE_CANCEL, hInstance, NULL);
    
    // Set progress bar range
    SendMessage(progressBar, PBM_SETRANGE, 0, MAKELPARAM(0, 100));
    
    // Initialize update handler
    updateHandler = new UpdateHandler(this);
    
    // Set timer for periodic checks
    SetTimer(hwnd, ID_TIMER_CHECK, 300000, NULL); // Check every 5 minutes
    
    return true;
}

void MainForm::Show() {
    ShowWindow(hwnd, SW_SHOW);
    UpdateWindow(hwnd);
    
    // Initial update check
    updateHandler->InitializeWithUpdateCheck();
}

void MainForm::UpdateVersionInfo(const std::string& latestVersion) {
    std::string currentText = "Current Version: " + VersionManager::getCurrentVersion();
    std::string latestText = "Latest Version: " + latestVersion;
    
    SetWindowTextA(lblCurrentVersion, currentText.c_str());
    SetWindowTextA(lblLatestVersion, latestText.c_str());
}

void MainForm::BeginPatchUpdate(const std::string& url) {
    pendingUpdateUrl = url;
    
    // Hide update button, show progress controls
    ShowWindow(btnUpdateNow, SW_HIDE);
    ShowWindow(progressBar, SW_SHOW);
    ShowWindow(lblPercentage, SW_SHOW);
    ShowWindow(btnUpdateCancel, SW_SHOW);
    
    UpdateStatus("Auto-Updating...");
    updateHandler->DownloadAndUpdate(url);
}

void MainForm::PromptMajorUpdate(const std::string& url, const std::string& latestVersion) {
    std::string message = "A major update (v" + latestVersion + ") is available.\n"
                         "Current version: " + VersionManager::getCurrentVersion() + "\n\n"
                         "Update now?";
    
    int result = MessageBoxA(hwnd, message.c_str(), "Major Update Available", MB_YESNO | MB_ICONINFORMATION);
    
    if (result == IDYES) {
        BeginPatchUpdate(url);
    } else {
        UpdateStatus("Update postponed by user.");
    }
}

void MainForm::UpdateStatus(const std::string& message) {
    SetWindowTextA(lblStatus, message.c_str());
}

void MainForm::SetProgress(int percentage) {
    SendMessage(progressBar, PBM_SETPOS, percentage, 0);
    std::string percentText = std::to_string(percentage) + "%";
    SetWindowTextA(lblPercentage, percentText.c_str());
}

// Window Procedure
LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam) {
    MainForm* form = nullptr;
    
    if (uMsg == WM_NCCREATE) {
        CREATESTRUCT* cs = (CREATESTRUCT*)lParam;
        form = (MainForm*)cs->lpCreateParams;
        SetWindowLongPtr(hwnd, GWLP_USERDATA, (LONG_PTR)form);
    } else {
        form = (MainForm*)GetWindowLongPtr(hwnd, GWLP_USERDATA);
    }
    
    if (form) {
        switch (uMsg) {
        case WM_COMMAND:
            switch (LOWORD(wParam)) {
            case ID_BUTTON_UPDATE_NOW:
                if (!form->pendingUpdateUrl.empty()) {
                    form->BeginPatchUpdate(form->pendingUpdateUrl);
                }
                break;
            case ID_BUTTON_UPDATE_CANCEL:
                if (MessageBoxA(hwnd, "Cancel update?", "Confirmation", MB_YESNO | MB_ICONQUESTION) == IDYES) {
                    ShowWindow(form->progressBar, SW_HIDE);
                    ShowWindow(form->lblPercentage, SW_HIDE);
                    ShowWindow(form->btnUpdateCancel, SW_HIDE);
                    ShowWindow(form->btnUpdateNow, SW_SHOW);
                }
                break;
            }
            break;
            
        case WM_TIMER:
            if (wParam == ID_TIMER_CHECK) {
                form->updateHandler->CheckForUpdates();
            }
            break;
            
        case WM_CLOSE:
            if (form->IsUpdating()) {
                MessageBoxA(hwnd, "Please wait until update completes", "Update in Progress", MB_OK | MB_ICONWARNING);
                return 0;
            }
            DestroyWindow(hwnd);
            break;
            
        case WM_DESTROY:
            KillTimer(hwnd, ID_TIMER_CHECK);
            PostQuitMessage(0);
            break;
        }
    }
    
    return DefWindowProc(hwnd, uMsg, wParam, lParam);
}