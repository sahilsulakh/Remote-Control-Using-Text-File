#pragma once
#include <windows.h>
#include <string>

class UpdateHandler;

class MainForm {
private:
    HWND hwnd;
    HWND btnUpdateNow, btnUpdateCancel, progressBar;
    HWND lblStatus, lblCurrentVersion, lblLatestVersion, lblPercentage;
    UpdateHandler* updateHandler;
    bool isUpdating;
    
public:
    std::string pendingUpdateUrl;
    
    MainForm();
    ~MainForm();
    
    bool Initialize(HINSTANCE hInstance);
    void Show();
    void UpdateVersionInfo(const std::string& latestVersion);
    void BeginPatchUpdate(const std::string& url);
    void PromptMajorUpdate(const std::string& url, const std::string& latestVersion);
    void UpdateStatus(const std::string& message);
    void SetProgress(int percentage);
    
    HWND GetHwnd() const { return hwnd; }
    bool IsUpdating() const { return isUpdating; }
    void SetUpdating(bool updating) { isUpdating = updating; }
    
    // Control handles for access from UpdateHandler
    HWND GetProgressBar() const { return progressBar; }
    HWND GetPercentageLabel() const { return lblPercentage; }
    
    friend LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
};

// Window procedure
LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);