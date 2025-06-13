# 🎮 Remote Control Using Text File

Remotely control and update your applications using simple text files hosted online. This project supports C++, .NET (Framework & Latest), and Python implementations, each with both Control Status and Update Handler modules.

---

## 📁 Directory Structure

```
Remote Control Using Text File/
├── C++/
│   ├── Control Status/
│   └── Update Handler/
├── .NET/
│   ├── Framework/
│   │   ├── Control Status/
│   │   └── Update Handler/
│   └── Latest/
│       ├── Control Status/
│       └── Update Handler/
└── Python/
    ├── Control Status/
    ├── Update Handler/
    └── Integration/
```

---

## 🚀 Quick Start

1. **Clone the repository**
2. **Pick your language**: C++, .NET, or Python
3. **Choose your module**: Control Status or Update Handler
4. **Follow the README in that folder for setup and usage**

---

## 🔑 Remote File Setup (Required for All Languages)

You need two URLs for your app to work:
- `control.txt` (for remote control)
- `update.txt` (for updates)

### 🌐 Using KeyMaster (Recommended)
1. Go to [KeyMaster](https://keymaster-agni.vercel.app/)
2. Create a vault and add:
   - `control.txt` (content: ACTIVE, PAUSED, or STOPPED)
   - `update.txt` (content: download_url, version, update_type)
3. Copy your vault URLs:
   - `https://keymaster-agni.vercel.app/api/vault/YOUR_VAULT_ID/control.txt`
   - `https://keymaster-agni.vercel.app/api/vault/YOUR_VAULT_ID/update.txt`
4. Paste these URLs into your app's config or code

### 🔄 Other Hosting Options
- GitHub Gist, Azure Blob, AWS S3, or any HTTPS-accessible text file
- Make sure files are public and fast to access

---

## 🛠️ Features
- **Remote Control**: Change app state (ACTIVE, PAUSED, STOPPED) from anywhere
- **Update Handler**: Push updates remotely, with version checks and progress
- **Cross-language**: Use in C++, .NET, or Python
- **Modern UI**: Windows Forms or Tkinter
- **Easy Integration**: Just set your URLs and go!

---

## 📚 Documentation & Support
- Each language/module has its own README for setup, usage, and troubleshooting
- For help, open an issue or discussion on GitHub

---

## 📜 License
MIT License

---

Crafted with ❤️ by Agniveer Corporation
