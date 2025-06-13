# 🎮 Control + Update Handler (MERGED)

## 🌟 Overview
This is the unified application that combines both the Control Status and Update Handler functionalities into a single, efficient solution. It provides remote control capabilities while maintaining automatic updates.

## 📋 Prerequisites
- 🖥️ Windows OS
- 🎯 Choose your .NET version:
  - .NET Framework 4.7.2+ (Framework version)
  - .NET 6/7/8 (Latest version)
- 💻 Visual Studio 2019+ (Framework) or 2022+ (Latest)
- 🌐 Internet connection

## 🏗️ Architecture

### 📱 Form1.cs
Unified interface combining both functionalities:
```csharp
public partial class Form1 : Form
{
    private readonly MasterClass _controller;
    private readonly UpdateHandler _updater;
    
    public Form1()
    {
        InitializeComponent();
        _controller = new MasterClass(this);
        _updater = new UpdateHandler(this);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        _controller.Start();
        _updater.StartAutoCheck();
    }
}
```

## 🔌 Key Features

### 🎮 Remote Control
- 🟢 **ACTIVE**: Full functionality
- 🟡 **PAUSED**: Maintenance mode
- 🔴 **STOPPED**: Graceful shutdown
- ⏱️ Real-time state updates
- 📝 Status logging

### 🔄 Auto Updates
- 🔍 Version checking
- 📥 Background downloads
- 📊 Progress tracking
- 🔄 Silent updates
- 🔙 Rollback support

## 🚀 Setup & Usage

1. 📂 Open in Visual Studio
2. 🛠️ Build the solution
3. ▶️ Run the application
4. ⚙️ Configure:
   - Control file URL
   - Update check URL
   - Check intervals
   - Update preferences

## 🔑 Remote File Setup

### Required URLs
You need two publicly accessible URLs:
1. `control.txt` - For application state control
2. `update.txt` - For update information

### Using KeyMaster (Recommended)
1. Visit [KeyMaster](https://keymaster-agni.vercel.app/)
2. Create an account & login
3. Create a new vault
4. Create two files:

   control.txt:
   ```
   ACTIVE    # or PAUSED or STOPPED
   ```

   update.txt:
   ```
   https://your-domain.com/downloads/app-1.0.1.zip
   1.0.1
   patch
   ```
5. Get your vault URLs:
   ```
   Control URL: https://keymaster-agni.vercel.app/api/vault/YOUR_VAULT_ID/control.txt
   Update URL:  https://keymaster-agni.vercel.app/api/vault/YOUR_VAULT_ID/update.txt
   ```
6. Configure these URLs in your application

### Alternative Hosting Options
You can host the files on:
- GitHub Gist
- Azure Blob Storage
- AWS S3
- Custom web server

Requirements:
- HTTPS endpoints
- Fast response times (≤ 1 second)
- High availability
- Plain text responses

### File Formats

control.txt:
```
ACTIVE    # Normal operation
PAUSED    # Maintenance mode
STOPPED   # Force shutdown
```

update.txt:
```
https://your-domain.com/downloads/your-app-version.zip
1.0.1
patch    # or 'major' for major updates
```

## 🔧 Configuration

### 📝 Control File Format
```plaintext
ACTIVE    # Normal operation
PAUSED    # Maintenance mode
STOPPED   # Force shutdown
```

### 📦 Update File Format
```json
{
  "version": "1.2.3",
  "url": "https://your-domain.com/updates/v1.2.3.zip",
  "notes": "Bug fixes and improvements",
  "mandatory": false
}
```

## 🛡️ Security Features

- 🔒 HTTPS enforcement
- 🔑 Package verification
- 🛡️ State validation
- 📝 Audit logging
- 🔙 Backup creation

## ⚠️ Troubleshooting

### 🌐 Network Issues
1. Check internet connection
2. Verify URLs
3. Check proxy settings
4. Validate SSL/TLS

### 🔄 Update Problems
1. Verify disk space
2. Check permissions
3. Review update logs
4. Validate package integrity

### 🎮 Control Issues
1. Check control file
2. Verify permissions
3. Review status logs
4. Check application state

## 📊 Monitoring

### 📝 Log Files
- `control_log.txt`: State changes
- `update_log.txt`: Update events
- `error_log.txt`: Error tracking

### 📈 Metrics
- Update success rate
- State change frequency
- Download performance
- Error statistics

## 💡 Best Practices

### 🔧 Deployment
1. Test in staging
2. Deploy gradually
3. Monitor metrics
4. Backup regularly

### 🛡️ Security
1. Use HTTPS only
2. Verify packages
3. Implement timeouts
4. Log all changes

### 🔄 Updates
1. Version carefully
2. Test thoroughly
3. Stage rollouts
4. Plan rollbacks

## 🤝 Support

Need help?
- 📘 Check documentation
- 🐛 Report issues
- 💬 Join discussions
- 📧 Contact support

## 📚 API Reference

### MasterClass Methods
```csharp
void Start()              // Start control monitoring
void Stop()              // Stop control monitoring
void SetState(string)     // Update application state
bool IsActive()          // Check if active
```

### UpdateHandler Methods
```csharp
Task CheckForUpdates()    // Manual update check
void StartAutoCheck()     // Begin auto-updates
void StopAutoCheck()      // Stop auto-updates
Task InstallUpdate()      // Install pending update
```

## 🔄 Workflow

1. Application Start
   - Initialize components
   - Start control monitoring
   - Begin update checks

2. Normal Operation
   - Monitor control file
   - Check for updates
   - Handle state changes

3. Update Process
   - Download in background
   - Prepare installation
   - Apply when ready

4. State Changes
   - Validate new state
   - Apply changes
   - Log transition

---

🔗 See individual components:
- [Control Status](../Control%20Status/README.md)
- [Update Handler](../Update%20Handler/README.md)

Crafted with ❤️ by Agniveer Corporation
