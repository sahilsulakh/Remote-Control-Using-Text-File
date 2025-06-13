"""Version management utilities."""

import sys
import pkg_resources

def get_current_version() -> str:
    """Get the current application version.
    
    Returns:
        Current version string
    """
    try:
        return pkg_resources.get_distribution('remote_control').version
    except pkg_resources.DistributionNotFound:
        return "1.0.0"  # Default version
