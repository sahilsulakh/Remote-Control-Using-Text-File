#include "UpdateHandler.h"
#include "MainForm.h"
#include "Version.h"
#include <wininet.h>
#include <fstream>
#include <sstream>
#include <thread>
#include <chrono>
#include <algorithm>
#include <stdexcept>

#pragma comment(lib, "wininet.lib")

UpdateHandler::UpdateHandler(MainForm* form) : mainForm(form), isUpdating(false) {
    updateUrl = "https://keymaster-agni.vercel.app/api/vault/ellRHrNSMrcAvtnKsvHVqyOvgbT2/update.txt";
}

void UpdateHandler::InitializeWithUpdateCheck() {
    std::thread([this]() {
        CheckForUpdates();
    }).detach();
}

void UpdateHandler::CheckForUpdates() {
    try {
        std::string currentVersionStr = VersionManager::getCurrentVersion();
        Version currentVersion(currentVersionStr);
        
        std::string updateInfo = DownloadString(updateUrl);
        if (updateInfo.empty()) {
            mainForm->UpdateStatus("Failed to check for updates");
            return;
        }
        
        std::vector<std::string> lines = SplitLines(updateInfo);
        if (lines.size() < 3) {
            mainForm->UpdateStatus("Invalid update info format");
            return;
        }
        
        std::string downloadUrl = Trim(lines[0]);
        std::string latestVersionStr = Trim(lines[1]);
        std::string updateType = ToLower(Trim(lines[2]));
        
        Version latestVersion(latestVersionStr);
        
        // Update UI with version info
        mainForm->UpdateVersionInfo(latestVersionStr);
        
        if (!(currentVersion < latestVersion)) {
            mainForm->UpdateStatus("Your application is up to date");
            return;
        }
        
        if (updateType == "major") {
            mainForm->PromptMajorUpdate(downloadUrl, latestVersionStr);
        } else {
            mainForm->BeginPatchUpdate(downloadUrl);
        }
        
    } catch (const std::exception& e) {
        mainForm->UpdateStatus("Update check failed: " + std::string(e.what()));
    }
}

void UpdateHandler::DownloadAndUpdate(const std::string& url) {
    if (isUpdating) return;
    isUpdating = true;
    mainForm->SetUpdating(true);
    
    std::thread([this, url]() {
        try {
            mainForm->UpdateStatus("Downloading update...");
            
            char tempPath[MAX_PATH];
            GetTempPathA(MAX_PATH, tempPath);
            
            std::string tempFile = std::string(tempPath) + "update_" + 
                                 std::to_string(GetTickCount64()) + ".exe";
            
            if (!DownloadFileWithProgress(url, tempFile)) {
                throw std::runtime_error("Failed to download update");
            }
            
            mainForm->UpdateStatus("Preparing installer...");
            CreateUpdateScript(tempFile);
            
            mainForm->UpdateStatus("Restarting application...");
            std::this_thread::sleep_for(std::chrono::milliseconds(500));
            
            PostQuitMessage(0);
            
        } catch (const std::exception& e) {
            mainForm->UpdateStatus("Error: " + std::string(e.what()));
            isUpdating = false;
            mainForm->SetUpdating(false);
        }
    }).detach();
}

std::string UpdateHandler::DownloadString(const std::string& url) {
    HINTERNET hInternet = InternetOpenA("UpdaterApp/1.0", INTERNET_OPEN_TYPE_PRECONFIG, NULL, NULL, 0);
    if (!hInternet) return "";
    
    HINTERNET hConnect = InternetOpenUrlA(hInternet, url.c_str(), NULL, 0, 
                                         INTERNET_FLAG_RELOAD | INTERNET_FLAG_NO_CACHE_WRITE, 0);
    if (!hConnect) {
        InternetCloseHandle(hInternet);
        return "";
    }
    
    std::string result;
    char buffer[4096];
    DWORD bytesRead;
    
    while (InternetReadFile(hConnect, buffer, sizeof(buffer), &bytesRead) && bytesRead > 0) {
        result.append(buffer, bytesRead);
    }
    
    InternetCloseHandle(hConnect);
    InternetCloseHandle(hInternet);
    return result;
}

bool UpdateHandler::DownloadFileWithProgress(const std::string& url, const std::string& outputPath) {
    HINTERNET hInternet = InternetOpenA("UpdaterApp/1.0", INTERNET_OPEN_TYPE_PRECONFIG, NULL, NULL, 0);
    if (!hInternet) return false;
    
    HINTERNET hConnect = InternetOpenUrlA(hInternet, url.c_str(), NULL, 0, 
                                         INTERNET_FLAG_RELOAD | INTERNET_FLAG_NO_CACHE_WRITE, 0);
    if (!hConnect) {
        InternetCloseHandle(hInternet);
        return false;
    }
    
    // Get content length
    DWORD contentLength = 0;
    DWORD bufferSize = sizeof(contentLength);
    HttpQueryInfoA(hConnect, HTTP_QUERY_CONTENT_LENGTH | HTTP_QUERY_FLAG_NUMBER, 
                  &contentLength, &bufferSize, NULL);
    
    std::ofstream file(outputPath, std::ios::binary);
    if (!file.is_open()) {
        InternetCloseHandle(hConnect);
        InternetCloseHandle(hInternet);
        return false;
    }
    
    char buffer[8192];
    DWORD bytesRead;
    DWORD totalRead = 0;
    
    while (InternetReadFile(hConnect, buffer, sizeof(buffer), &bytesRead) && bytesRead > 0) {
        file.write(buffer, bytesRead);
        totalRead += bytesRead;
        
        if (contentLength > 0) {
            int progress = (int)(totalRead * 100 / contentLength);
            mainForm->SetProgress(progress);
        }
    }
    
    file.close();
    InternetCloseHandle(hConnect);
    InternetCloseHandle(hInternet);
    return true;
}

void UpdateHandler::CreateUpdateScript(const std::string& updateFile) {
    char currentExePath[MAX_PATH];
    GetModuleFileNameA(NULL, currentExePath, MAX_PATH);
    
    char tempPath[MAX_PATH];
    GetTempPathA(MAX_PATH, tempPath);
    std::string batchPath = std::string(tempPath) + "update.bat";
    
    std::string currentExeFileName = GetFileNameFromPath(currentExePath);
    
    std::string script = 
        "@echo off\n"
        "chcp 65001 >nul\n"
        ":loop\n"
        "tasklist /fi \"IMAGENAME eq " + currentExeFileName + "\" | find /i \"" + currentExeFileName + "\" >nul\n"
        "if %errorlevel%==0 (\n"
        "    timeout /t 1 /nobreak >nul\n"
        "    goto loop\n"
        ")\n"
        "del \"" + std::string(currentExePath) + ".bak\" 2>nul\n"
        "move \"" + std::string(currentExePath) + "\" \"" + std::string(currentExePath) + ".bak\" 2>nul\n"
        "move \"" + updateFile + "\" \"" + std::string(currentExePath) + "\"\n"
        "start \"\" \"" + std::string(currentExePath) + "\"\n"
        "del \"" + batchPath + "\"\n"
        "exit\n";
    
    std::ofstream batchFile(batchPath);
    batchFile << script;
    batchFile.close();
    
    STARTUPINFOA si = { sizeof(si) };
    PROCESS_INFORMATION pi;
    
    std::string cmdLine = "cmd.exe /c start \"\" \"" + batchPath + "\"";
    CreateProcessA(NULL, &cmdLine[0], NULL, NULL, FALSE, 
                  CREATE_NO_WINDOW, NULL, NULL, &si, &pi);
    
    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);
}

std::vector<std::string> UpdateHandler::SplitLines(const std::string& str) {
    std::vector<std::string> lines;
    std::stringstream ss(str);
    std::string line;
    while (std::getline(ss, line)) {
        if (!line.empty()) {
            lines.push_back(line);
        }
    }
    return lines;
}

std::string UpdateHandler::Trim(const std::string& str) {
    size_t first = str.find_first_not_of(" \t\r\n");
    if (first == std::string::npos) return "";
    size_t last = str.find_last_not_of(" \t\r\n");
    return str.substr(first, (last - first + 1));
}

std::string UpdateHandler::ToLower(const std::string& str) {
    std::string lower = str;
    std::transform(lower.begin(), lower.end(), lower.begin(), ::tolower);
    return lower;
}

std::string UpdateHandler::GetFileNameFromPath(const std::string& path) {
    size_t pos = path.find_last_of("\\/");
    return (pos == std::string::npos) ? path : path.substr(pos + 1);
}