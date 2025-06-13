"""Logging utility functions."""

import logging
import sys
from typing import Dict

def setup_logger(config: Dict = None) -> logging.Logger:
    """Set up and configure the application logger.
    
    Args:
        config: Logger configuration dictionary
        
    Returns:
        Configured logger instance
    """
    if config is None:
        config = {}
    
    # Create logger
    logger = logging.getLogger('remote_control')
    logger.setLevel(config.get('level', 'INFO'))
    
    # Remove existing handlers
    logger.handlers = []
    
    # Console handler
    console_handler = logging.StreamHandler(sys.stdout)
    console_handler.setFormatter(logging.Formatter(
        '%(asctime)s - %(name)s - %(levelname)s - %(message)s'
    ))
    logger.addHandler(console_handler)
    
    # File handler
    if 'file' in config:
        file_handler = logging.FileHandler(config['file'])
        file_handler.setFormatter(logging.Formatter(
            '%(asctime)s - %(name)s - %(levelname)s - %(message)s'
        ))
        logger.addHandler(file_handler)
    
    return logger

def get_logger(name: str) -> logging.Logger:
    """Get a logger instance for a module.
    
    Args:
        name: Module name
        
    Returns:
        Logger instance for the module
    """
    return logging.getLogger(f'remote_control.{name}')
