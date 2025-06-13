#pragma once
#include <windows.h>
#include <string>
#include <vector>

class MainForm;

class UpdateHandler {
private:
    MainForm* mainForm;
    std::string updateUrl;
    bool isUpdating;
    
public:
    explicit UpdateHandler(MainForm* form);
    
    void InitializeWithUpdateCheck();
    void CheckForUpdates();
    void DownloadAndUpdate(const std::string& url);
    bool IsUpdating() const { return isUpdating; }
    
private:
    std::string DownloadString(const std::string& url);
    bool DownloadFileWithProgress(const std::string& url, const std::string& outputPath);
    void CreateUpdateScript(const std::string& updateFile);
    
    // Utility functions
    std::vector<std::string> SplitLines(const std::string& str);
    std::string Trim(const std::string& str);
    std::string ToLower(const std::string& str);
    std::string GetFileNameFromPath(const std::string& path);
};