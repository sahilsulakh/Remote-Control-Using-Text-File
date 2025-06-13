# 🚀 Update Handler (C++ Version)

Hey there! 👋 Welcome to the C++ version of our Update Handler. Don't worry if you're new to C++ - we'll walk you through everything step by step!

## 🎯 What Does This Do?
This app checks for updates to your software and installs them automatically. Think of it like your phone's app updater, but for Windows applications! 

## 🛠️ Prerequisites
Before you start, make sure you have:
- Windows 10 or newer
- Visual Studio 2019 or newer
- CMake 3.16 or newer
- Basic C++ knowledge (but don't worry, we'll help!)

## 🏃‍♂️ Quick Start

### 1️⃣ Building the Project
```bash
# 1. Create a build directory
mkdir build
cd build

# 2. Generate Visual Studio solution
cmake ..

# 3. Build the project
cmake --build . --config Release
```

### 2️⃣ Running the App
Just double-click the `AutoUpdater.exe` in your build folder! You'll see:
- Current version
- Latest available version
- Update button (if an update is available)
- Progress bar during downloads

## 🎨 Main Components

### 📱 Main Window (`MainForm`)
```cpp
// Creating the main window is as simple as:
MainForm form;
form.Initialize(hInstance);
form.Show();
```

### 🔄 Update Handler
```cpp
// Checking for updates is easy:
UpdateHandler updater(&mainForm);
updater.CheckForUpdates();
```

### 📊 Version Management
```cpp
// Working with versions:
Version currentVersion("1.0.0.0");
Version newVersion("1.0.1.0");
if (currentVersion < newVersion) {
    // Time to update!
}
```

## 🔑 Update File Setup

### Required URL
You need a publicly accessible URL hosting a text file (`update.txt`) containing update information:
- Download URL
- Version number
- Update type

### Using KeyMaster (Recommended)
1. Visit [KeyMaster](https://keymaster-agni.vercel.app/)
2. Create an account & login
3. Create a new vault
4. Create `update.txt` with content:
   ```
   https://your-domain.com/downloads/app-1.0.1.zip
   1.0.1
   patch
   ```
5. Get your vault URL:
   ```
   https://keymaster-agni.vercel.app/api/vault/YOUR_VAULT_ID/update.txt
   ```
6. Update `Constants.h` or your configuration with this URL

### Alternative Hosting Options
You can host update.txt on:
- GitHub Gist
- Azure Blob Storage
- AWS S3
- Custom web server

Requirements:
- HTTPS endpoint
- Fast response times
- High availability
- Plain text response

### File Format
update.txt should contain exactly three lines:
```
https://your-domain.com/downloads/your-app-version.zip
1.0.1
patch    # or 'major' for major updates
```

## 🎮 Controls & Features

### Buttons
- 🔄 **Update Now**: Starts the update process
- ❌ **Cancel**: Stops an ongoing update
- 📊 **Progress Bar**: Shows download progress

### Status Messages
- 🟢 "Up to date!"
- 🔄 "Downloading update..."
- ✅ "Update complete!"
- ❌ "Update failed"

## 🔧 Advanced Usage

### Custom Update URL
```cpp
// In UpdateHandler.cpp:
updateUrl = "https://your-custom-domain.com/update.txt"; //<<<--- GET IT FROM KEYMASTER VAULT PAGE (if using keymaster)
```

### Auto-Check Interval
```cpp
// In MainForm.cpp:
SetTimer(hwnd, ID_TIMER_CHECK, 3600000, NULL); // Check every hour
```

## 🛠️ Troubleshooting

### Common Issues & Solutions

1. **Build Fails** ❌
   ```bash
   # Try this:
   cmake .. -A x64  # Force 64-bit build
   ```

2. **Can't Download Updates** 📶
   - Check internet connection
   - Verify update URL
   - Check Windows firewall

3. **Version Problems** 🔢
   - Make sure version numbers are in format: X.X.X.X
   - Each number should be 0-999

## 💡 Tips & Tricks

1. **Testing Updates**
   ```cpp
   // Force an update check:
   SendMessage(hwnd, WM_COMMAND, ID_BUTTON_UPDATE_NOW, 0);
   ```

2. **Debug Mode**
   ```bash
   cmake -DCMAKE_BUILD_TYPE=Debug ..
   ```

3. **Memory Checks**
   ```bash
   cmake -DENABLE_MEMCHECK=ON ..
   ```

## 🤝 Need Help?

- 🐛 Found a bug? Open an issue!
- 💡 Have a suggestion? We'd love to hear it!
- ❓ Questions? Check our [discussions](link-to-discussions)

## 🌟 Fun Facts

- Uses modern C++17 features
- Less than 1MB executable size
- Super fast download speeds
- Memory efficient

## 📚 Learning Resources

Want to learn more? Check out:
- [Modern C++ Tutorial](https://www.learncpp.com/)
- [Windows API Guide](https://docs.microsoft.com/en-us/windows/win32/)
- [CMake Tutorial](https://cmake.org/cmake/help/latest/guide/tutorial/index.html)

---

Made with ❤️ by Agniveer Tutorials

Remember: Updating software should be fun, not frustrating! 😊

Questions? Need help? Contact us anytime!
