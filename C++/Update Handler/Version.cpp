#include "Version.h"
#include <sstream>

Version::Version(const std::string& versionStr) {
    std::vector<std::string> parts = split(versionStr, '.');
    major = parts.size() > 0 ? std::stoi(parts[0]) : 0;
    minor = parts.size() > 1 ? std::stoi(parts[1]) : 0;
    build = parts.size() > 2 ? std::stoi(parts[2]) : 0;
    revision = parts.size() > 3 ? std::stoi(parts[3]) : 0;
}

bool Version::operator<(const Version& other) const {
    if (major != other.major) return major < other.major;
    if (minor != other.minor) return minor < other.minor;
    if (build != other.build) return build < other.build;
    return revision < other.revision;
}

std::string Version::toString() const {
    return std::to_string(major) + "." + std::to_string(minor) + "." + 
           std::to_string(build) + "." + std::to_string(revision);
}

std::vector<std::string> Version::split(const std::string& str, char delimiter) {
    std::vector<std::string> tokens;
    std::stringstream ss(str);
    std::string token;
    while (std::getline(ss, token, delimiter)) {
        tokens.push_back(token);
    }
    return tokens;
}

std::string VersionManager::getCurrentVersion() {
    return "1.0.0.0"; // Hardcoded version - change this as needed
}