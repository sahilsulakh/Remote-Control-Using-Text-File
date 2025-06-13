#!/usr/bin/env python3
"""
Main entry point for the Remote Control and Update Handler application.
"""

import sys
import tkinter as tk
from gui.main_window import MainWindow
from utils.config import load_config
from utils.logger import setup_logger

def main():
    """Initialize and run the application."""
    try:
        # Load configuration
        config = load_config()
        
        # Setup logging
        logger = setup_logger(config.get('logging', {}))
        
        # Create main window
        root = tk.Tk()
        app = MainWindow(root, config)
        
        # Configure window
        root.title("Remote Control & Update Handler")
        root.geometry("600x400")
        root.minsize(500, 300)
        
        # Start the application
        root.mainloop()
        
    except Exception as e:
        logger.error(f"Application failed to start: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()
