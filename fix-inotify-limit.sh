#!/bin/bash
# Script to fix inotify limit issue on Linux
# This increases the maximum number of inotify instances

echo "Fixing inotify limit issue..."

# Check current limit
CURRENT_LIMIT=$(sysctl fs.inotify.max_user_instances | awk '{print $3}')
echo "Current inotify limit: $CURRENT_LIMIT"

# Increase the limit temporarily (until reboot)
echo "Setting temporary limit to 512..."
sudo sysctl -w fs.inotify.max_user_instances=512

# Make it permanent by adding to /etc/sysctl.conf
if ! grep -q "fs.inotify.max_user_instances" /etc/sysctl.conf 2>/dev/null; then
    echo "Making the change permanent..."
    echo "fs.inotify.max_user_instances=512" | sudo tee -a /etc/sysctl.conf
    echo "Permanent limit set. The change will take effect after reboot."
else
    echo "Limit already configured in /etc/sysctl.conf"
fi

# Verify the new limit
NEW_LIMIT=$(sysctl fs.inotify.max_user_instances | awk '{print $3}')
echo "New inotify limit: $NEW_LIMIT"

echo ""
echo "Done! You can now run your application."
echo "Note: If you still encounter issues, you may need to:"
echo "  1. Restart your system, or"
echo "  2. Set DOTNET_USE_POLLING_FILE_WATCHER=1 environment variable to use polling instead of inotify"

