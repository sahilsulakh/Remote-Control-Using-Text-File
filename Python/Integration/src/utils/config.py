"""Configuration management utilities."""

import os
import yaml
from typing import Dict

def load_config() -> Dict:
    """Load the application configuration.
    
    Returns:
        Configuration dictionary
    """
    config_path = os.environ.get(
        'REMOTE_CONTROL_CONFIG',
        'config.yaml'
    )
    
    try:
        with open(config_path, 'r') as f:
            return yaml.safe_load(f)
    except FileNotFoundError:
        return _get_default_config()
    except yaml.YAMLError as e:
        print(f"Error loading config: {e}")
        return _get_default_config()

def _get_default_config() -> Dict:
    """Get default configuration.
    
    Returns:
        Default configuration dictionary
    """
    return {
        'control': {
            'url': 'https://keymaster-agni.vercel.app/control.txt',
            'check_interval': 2.0  # seconds
        },
        'update': {
            'url': 'https://keymaster-agni.vercel.app/update.txt',
            'check_interval': 3600  # seconds
        },
        'logging': {
            'level': 'INFO',
            'file': 'remote_control.log'
        }
    }
