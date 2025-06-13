# 🎮 Control Status (.NET Framework)

## 🌟 Overview
This is the .NET Framework version of the Control Status application. It allows you to remotely control the state of your application using a remote text file.

## 🔑 Control File Setup

### Required URL
You need a publicly accessible URL hosting a text file (`control.txt`) containing one of these states:
- `ACTIVE`
- `PAUSED`
- `STOPPED`

### Using KeyMaster (Recommended)
1. Visit [KeyMaster](https://keymaster-agni.vercel.app/)
2. Create an account & login
3. Create a new vault
4. Create `control.txt` with content:
   ```
   ACTIVE
   ```
5. Get your vault URL:
   ```
   https://keymaster-agni.vercel.app/api/vault/YOUR_VAULT_ID/control.txt
   ```
6. Use this URL in the application

### Alternative Hosting Options
You can host control.txt on:
- GitHub Gist
- Azure Blob Storage
- AWS S3
- Custom web server

Requirements:
- HTTPS endpoint
- Fast response (≤ 1 second)
- High availability
- Plain text response

### File Format
control.txt should contain exactly one line with:
```
ACTIVE    # Normal operation
PAUSED    # Maintenance mode
STOPPED   # Force shutdown
```

## 📋 Prerequisites
- 🖥️ Windows OS
- 🎯 .NET Framework 4.7.2 or later
- 💻 Visual Studio 2019 or later
- 🌐 Internet connection

## 📦 NuGet Packages
- 🌐 **System.Net.Http**: HTTP client for remote file access
- 🖼️ **System.Windows.Forms**: Windows UI components

## 🏗️ Code Architecture

### 📱 Form1.cs
Main application window and entry point:
```csharp
public Form1()
{
    InitializeComponent();
    _master = new MasterClass(this);
}

private void Form1_Load(object sender, EventArgs e)
{
    _master.Start();
}
```

### ⚙️ MasterClass.cs
Core control logic:
- 🕒 Timer-based polling (2-second intervals)
- 🌐 HTTP requests to remote control file
- 🔄 State management and UI updates
- 📝 Error logging

```csharp
private async Task CheckControlFileAsync()
{
    try
    {
        string raw = await Http.GetStringAsync(ControlFileUrl);
        string status = raw.Trim().ToUpperInvariant();
        if (status != _lastStatus)
        {
            _lastStatus = status;
            ApplyStatus(status);
        }
    }
    catch (Exception ex)
    {
        File.AppendAllText("masterclass_log.txt",
            $"{DateTime.Now:u}  {ex.GetType().Name}: {ex.Message}{Environment.NewLine}");
    }
}
```

## 🚀 Setup & Usage

1. 📂 Open in Visual Studio
2. 🛠️ Build the solution
3. ▶️ Run the application
4. 🌐 Application states:
   - 🟢 **ACTIVE**: Normal operation
   - 🟡 **PAUSED**: UI disabled, maintenance mode
   - 🔴 **STOPPED**: Application exits

## 🔌 Integration Steps

1. 📥 Download the application
2. ⚙️ Configure control file URL
3. 🚀 Deploy to target machines
4. 🔄 Update control file to manage states

## 🛡️ Security

- 🔒 Always use HTTPS
- 🔑 Implement access controls on control file
- 🛡️ Keep .NET Framework updated
- 📝 Monitor error logs

## 🔧 Configuration

### Remote Control File Format
```plaintext
ACTIVE   # Normal operation
PAUSED   # Maintenance mode
STOPPED  # Force shutdown
```

### Error Logging
- 📁 Location: `masterclass_log.txt`
- 📊 Format: `[Timestamp] ErrorType: Message`

## ⚠️ Troubleshooting

### Common Issues
1. 🌐 Connection Failed
   - Check internet connection
   - Verify control file URL
   - Confirm HTTPS settings

2. 🔒 Security Errors
   - Update .NET Framework
   - Check TLS settings
   - Verify certificates

3. 🕒 Timing Issues
   - Check system clock
   - Verify polling interval
   - Monitor network latency

### Debug Tips
- 📝 Check `masterclass_log.txt`
- 🔍 Use network monitoring tools
- 🛠️ Enable .NET Framework logging

## 📚 Best Practices

1. 🔄 Regular Testing
   - Test all states
   - Verify error handling
   - Check logging

2. 🛡️ Security
   - Use HTTPS only
   - Regular updates
   - Access control

3. 🔧 Maintenance
   - Monitor logs
   - Clean old logs
   - Update configurations

## 🤝 Support
Need help? Check:
- 📘 Documentation
- 🐛 Issue tracker
- 💬 Discussion forum

---

🔄 For modern .NET support, see the .NET Latest folder.
❓ Questions? Open an issue on our repository.

Made with ❤️ by Agniveer Tutorials
