# 🐍 Python Integration - Control Status & Update Handler

## 🌟 Overview
This is a Python implementation of the Remote Control and Update Handler system, combining both functionalities into a single, efficient solution using modern Python features and best practices.

## 🔑 Remote Control Setup

### Required URLs
You'll need two URLs for the application to work:
1. Control URL - For application state management
2. Update URL - For version updates

### Using KeyMaster (Recommended)
1. Go to [KeyMaster](https://keymaster-agni.vercel.app/)
2. Create an account & sign in
3. Create a new vault
4. Add two files:
   ```
   control.txt:
   ACTIVE    # or PAUSED or STOPPED

   update.txt:
   https://your-download-url/app-1.0.1.zip
   1.0.1
   patch    # or major
   ```
5. Get your vault URLs:
   ```yaml
   Control URL: https://keymaster-agni.vercel.app/api/vault/YOUR_VAULT_ID/control.txt
   Update URL:  https://keymaster-agni.vercel.app/api/vault/YOUR_VAULT_ID/update.txt
   ```
6. Update `config.yaml` with these URLs

### Using Alternative Services
You can use any service that provides HTTPS access to text files:
- GitHub Gist
- Azure Blob Storage
- AWS S3
- Custom web server

Requirements:
- HTTPS endpoint
- Fast response times
- High availability
- Plain text file support

## 📋 Prerequisites
- Python 3.8 or newer
- Required packages (install via `pip install -r requirements.txt`):
  - tkinter (usually comes with Python)
  - requests
  - packaging
  - watchdog

## 🗂️ Project Structure
```
Python Integration/
├── src/
│   ├── __init__.py
│   ├── main.py              # Main entry point
│   ├── control_status.py    # Remote control implementation
│   ├── update_handler.py    # Update management
│   ├── gui/
│   │   ├── __init__.py
│   │   ├── main_window.py   # Main GUI window
│   │   └── styles.py        # GUI styles and themes
│   └── utils/
│       ├── __init__.py
│       ├── version.py       # Version management
│       ├── config.py        # Configuration handling
│       └── logger.py        # Logging utility
├── tests/                   # Unit tests
├── requirements.txt         # Project dependencies
└── config.yaml             # Configuration file
```

## 🚀 Quick Start
1. Clone the repository
2. Install dependencies:
   ```bash
   pip install -r requirements.txt
   ```
3. Run the application:
   ```bash
   python src/main.py
   ```

## ⚙️ Configuration
Edit `config.yaml` to set:
- Control file URL
- Update check URL
- Check intervals
- Log settings

## 🔄 Features

### Remote Control
- Real-time state monitoring
- Automatic UI updates
- State persistence
- Error recovery

### Update Handler
- Automatic update checks
- Progress tracking
- Safe installation
- Version comparison
- Rollback support

## 🛠️ Development

### Running Tests
```bash
python -m pytest tests/
```

### Building Distribution
```bash
python setup.py bdist_wheel
```

## 📝 API Documentation
See the docstrings in individual modules or generate documentation:
```bash
pdoc --html src/
```

## 🤝 Contributing
1. Fork the repository
2. Create your feature branch
3. Write tests
4. Submit pull request

## 📜 License
MIT License

Made with ❤️ by Agniveer Tutorials
