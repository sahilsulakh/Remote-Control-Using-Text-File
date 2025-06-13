#pragma once
#include <string>
#include <vector>

class Version {
public:
    int major, minor, build, revision;
    
    explicit Version(const std::string& versionStr);
    bool operator<(const Version& other) const;
    std::string toString() const;
    
private:
    std::vector<std::string> split(const std::string& str, char delimiter);
};

class VersionManager {
public:
    static std::string getCurrentVersion();
};