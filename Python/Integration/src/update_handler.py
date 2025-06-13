"""Update handler implementation."""

import os
import threading
import tempfile
import requests
from typing import Callable
from packaging import version
from .utils.logger import get_logger
from .utils.version import get_current_version

logger = get_logger(__name__)

class UpdateHandler:
    """Handles software updates."""
    
    def __init__(self, config: dict, progress_callback: Callable[[float, str], None]):
        """Initialize the update handler.
        
        Args:
            config: Configuration dictionary
            progress_callback: Callback for update progress
        """
        self.config = config
        self.progress_callback = progress_callback
        self.update_url = config.get('url', 'https://keymaster-agni.vercel.app/update.txt')
        self.check_interval = config.get('check_interval', 3600)  # 1 hour
        self.running = False
        self.thread = None
    
    def start_monitoring(self):
        """Start monitoring for updates."""
        if self.thread and self.thread.is_alive():
            return
            
        self.running = True
        self.thread = threading.Thread(target=self._monitor_loop, daemon=True)
        self.thread.start()
        logger.info("Started update monitoring")
    
    def stop_monitoring(self):
        """Stop monitoring for updates."""
        self.running = False
        if self.thread:
            self.thread.join()
        logger.info("Stopped update monitoring")
    
    def check_updates(self):
        """Manually check for updates."""
        try:
            self._check_and_apply_update()
        except Exception as e:
            logger.error(f"Update check failed: {e}")
            self.progress_callback(0, f"Update check failed: {e}")
    
    def _monitor_loop(self):
        """Main monitoring loop."""
        while self.running:
            try:
                self._check_and_apply_update()
                time.sleep(self.check_interval)
            except Exception as e:
                logger.error(f"Error in update monitoring: {e}")
                time.sleep(self.check_interval)
    
    def _check_and_apply_update(self):
        """Check for and apply any available updates."""
        try:
            # Get update info
            response = requests.get(self.update_url, timeout=5)
            response.raise_for_status()
            
            update_info = response.text.strip().split('\n')
            if len(update_info) < 2:
                raise ValueError("Invalid update info format")
            
            download_url = update_info[0].strip()
            new_version = update_info[1].strip()
            
            # Compare versions
            current_ver = version.parse(get_current_version())
            update_ver = version.parse(new_version)
            
            if update_ver <= current_ver:
                self.progress_callback(100, "Already up to date!")
                return
            
            # Download and apply update
            self.progress_callback(0, f"Downloading version {new_version}...")
            self._download_and_apply_update(download_url, new_version)
            
        except requests.RequestException as e:
            logger.error(f"Failed to fetch update info: {e}")
            self.progress_callback(0, "Update check failed")
    
    def _download_and_apply_update(self, url: str, new_version: str):
        """Download and apply an update.
        
        Args:
            url: Update package URL
            new_version: New version string
        """
        try:
            # Download file
            response = requests.get(url, stream=True)
            response.raise_for_status()
            
            total_size = int(response.headers.get('content-length', 0))
            block_size = 8192
            downloaded = 0
            
            # Create temporary file
            with tempfile.NamedTemporaryFile(delete=False) as temp_file:
                for data in response.iter_content(block_size):
                    temp_file.write(data)
                    downloaded += len(data)
                    progress = (downloaded / total_size) * 100 if total_size else 0
                    self.progress_callback(progress, f"Downloading: {progress:.1f}%")
            
            # Apply update
            self.progress_callback(100, "Installing update...")
            self._apply_update(temp_file.name, new_version)
            
        except Exception as e:
            logger.error(f"Update failed: {e}")
            self.progress_callback(0, f"Update failed: {e}")
            if 'temp_file' in locals():
                os.unlink(temp_file.name)
    
    def _apply_update(self, update_file: str, new_version: str):
        """Apply the downloaded update.
        
        Args:
            update_file: Path to the update file
            new_version: New version string
        """
        try:
            # TODO: Implement update application logic
            # This will depend on your specific update mechanism
            pass
            
        finally:
            os.unlink(update_file)  # Clean up
            
        self.progress_callback(100, f"Updated to version {new_version}")
        logger.info(f"Successfully updated to version {new_version}")
