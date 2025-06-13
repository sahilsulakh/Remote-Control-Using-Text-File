"""Main window implementation for the application."""

import tkinter as tk
from tkinter import ttk, messagebox
from .styles import COLORS, FONTS, PADDING
from ..control_status import ControlStatus
from ..update_handler import UpdateHandler

class MainWindow:
    """Main window class implementing the GUI."""
    
    def __init__(self, root: tk.Tk, config: dict):
        """Initialize the main window.
        
        Args:
            root: The root Tkinter window
            config: Application configuration dictionary
        """
        self.root = root
        self.config = config
        
        # Initialize components
        self.control_status = ControlStatus(config.get('control', {}), self._on_state_change)
        self.update_handler = UpdateHandler(config.get('update', {}), self._on_update_progress)
        
        self._setup_ui()
        self._start_monitoring()
    
    def _setup_ui(self):
        """Set up the user interface."""
        # Main frame
        self.main_frame = ttk.Frame(self.root, padding=PADDING['medium'])
        self.main_frame.grid(row=0, column=0, sticky="nsew")
        
        # Configure grid
        self.root.columnconfigure(0, weight=1)
        self.root.rowconfigure(0, weight=1)
        self.main_frame.columnconfigure(1, weight=1)
        
        # Status section
        self._setup_status_section()
        
        # Update section
        self._setup_update_section()
        
        # Log section
        self._setup_log_section()
    
    def _setup_status_section(self):
        """Set up the status section of the UI."""
        status_frame = ttk.LabelFrame(self.main_frame, text="Application Status", padding=PADDING['medium'])
        status_frame.grid(row=0, column=0, columnspan=2, sticky="nsew", pady=PADDING['small'])
        
        self.status_label = ttk.Label(
            status_frame,
            text="Current Status: Checking...",
            font=FONTS['normal']
        )
        self.status_label.pack(pady=PADDING['small'])
    
    def _setup_update_section(self):
        """Set up the update section of the UI."""
        update_frame = ttk.LabelFrame(self.main_frame, text="Updates", padding=PADDING['medium'])
        update_frame.grid(row=1, column=0, columnspan=2, sticky="nsew", pady=PADDING['small'])
        
        # Version info
        version_frame = ttk.Frame(update_frame)
        version_frame.pack(fill="x", pady=PADDING['small'])
        
        self.current_version_label = ttk.Label(
            version_frame,
            text="Current Version: Checking...",
            font=FONTS['normal']
        )
        self.current_version_label.pack(side="left", padx=PADDING['medium'])
        
        self.latest_version_label = ttk.Label(
            version_frame,
            text="Latest Version: Checking...",
            font=FONTS['normal']
        )
        self.latest_version_label.pack(side="right", padx=PADDING['medium'])
        
        # Progress bar
        self.progress_var = tk.DoubleVar()
        self.progress_bar = ttk.Progressbar(
            update_frame,
            variable=self.progress_var,
            maximum=100
        )
        self.progress_bar.pack(fill="x", pady=PADDING['small'])
        
        # Update button
        self.update_button = ttk.Button(
            update_frame,
            text="Check for Updates",
            command=self._check_updates
        )
        self.update_button.pack(pady=PADDING['small'])
    
    def _setup_log_section(self):
        """Set up the log section of the UI."""
        log_frame = ttk.LabelFrame(self.main_frame, text="Log", padding=PADDING['medium'])
        log_frame.grid(row=2, column=0, columnspan=2, sticky="nsew", pady=PADDING['small'])
        
        # Log text widget
        self.log_text = tk.Text(
            log_frame,
            height=5,
            font=FONTS['small'],
            wrap="word"
        )
        self.log_text.pack(fill="both", expand=True)
    
    def _start_monitoring(self):
        """Start monitoring for control status and updates."""
        self.control_status.start_monitoring()
        self.update_handler.start_monitoring()
    
    def _on_state_change(self, new_state: str):
        """Handle state changes from the control status.
        
        Args:
            new_state: The new application state
        """
        self.status_label.config(text=f"Current Status: {new_state}")
        self._log(f"State changed to: {new_state}")
        
        if new_state == "STOPPED":
            if messagebox.showinfo("Application Stopped", "The application will now close."):
                self.root.quit()
    
    def _on_update_progress(self, progress: float, message: str):
        """Handle update progress changes.
        
        Args:
            progress: Progress percentage (0-100)
            message: Progress message
        """
        self.progress_var.set(progress)
        self._log(message)
    
    def _check_updates(self):
        """Manually trigger an update check."""
        self.update_button.config(state="disabled")
        self.update_handler.check_updates()
        self.update_button.config(state="normal")
    
    def _log(self, message: str):
        """Add a message to the log.
        
        Args:
            message: The message to log
        """
        self.log_text.insert("end", f"{message}\n")
        self.log_text.see("end")
