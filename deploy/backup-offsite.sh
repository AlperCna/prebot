#!/bin/sh
# Copies PreBot's nightly backups off the server (run from the host's cron, e.g. 05:30 every day):
#   30 5 * * *  /opt/prebot/deploy/backup-offsite.sh >> /var/log/prebot-backup.log 2>&1
#
# One-time setup on the VPS:
#   1) apt install rclone
#   2) rclone config   -> create a remote for your storage (Backblaze B2, Cloudflare R2, another server...)
#                         then a "crypt" remote on top of it named "prebot-backup", so files are encrypted
#                         before they leave the server.
#   3) chmod +x deploy/backup-offsite.sh
#
# The app writes the backups itself at 05:00 Turkey time into the prebot-data volume (/data/backups).
set -eu

VOLUME_DIR="$(docker volume inspect -f '{{ .Mountpoint }}' prebot_prebot-data)"
rclone sync "$VOLUME_DIR/backups" prebot-backup:backups --create-empty-src-dirs
echo "$(date -Is) offsite backup ok"
