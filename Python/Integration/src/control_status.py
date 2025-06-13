"""Control status implementation."""

import time
import threading
import requests
from typing import Callable
from .utils.logger import get_logger

logger = get_logger(__name__)

class ControlStatus:
    """Handles the remote control status functionality."""
    
    def __init__(self, config: dict, state_callback: Callable[[str], None]):
        """Initialize the control status handler.
        
        Args:
            config: Configuration dictionary
            state_callback: Callback function for state changes
        """
        self.config = config
        self.state_callback = state_callback
        self.control_url = config.get('url', 'https://keymaster-agni.vercel.app/control.txt')
        self.check_interval = config.get('check_interval', 2.0)  # seconds
        self.current_state = None
        self.running = False
        self.thread = None
    
    def start_monitoring(self):
        """Start monitoring the control file."""
        if self.thread and self.thread.is_alive():
            return
        
        self.running = True
        self.thread = threading.Thread(target=self._monitor_loop, daemon=True)
        self.thread.start()
        logger.info("Started control status monitoring")
    
    def stop_monitoring(self):
        """Stop monitoring the control file."""
        self.running = False
        if self.thread:
            self.thread.join()
        logger.info("Stopped control status monitoring")
    
    def _monitor_loop(self):
        """Main monitoring loop."""
        while self.running:
            try:
                self._check_status()
                time.sleep(self.check_interval)
            except Exception as e:
                logger.error(f"Error in monitoring loop: {e}")
                time.sleep(self.check_interval)
    
    def _check_status(self):
        """Check the current control status."""
        try:
            response = requests.get(self.control_url, timeout=5)
            response.raise_for_status()
            
            new_state = response.text.strip().upper()
            if new_state != self.current_state:
                self.current_state = new_state
                logger.info(f"State changed to: {new_state}")
                self.state_callback(new_state)
                
        except requests.RequestException as e:
            logger.error(f"Failed to fetch control status: {e}")
            
    def get_current_state(self) -> str:
        """Get the current control state.
        
        Returns:
            The current state string
        """
        return self.current_state or "UNKNOWN"
