# DupeFerret UI User Guide

## Quick Start

1. **Launch the Application**
   - Run `dupeferret.ui.exe` from the build output, or
   - Use `dotnet run --project src/dupeferret.ui/dupeferret.ui.csproj`

2. **Add Directories**
   - Click the **"Add Directory"** button
   - Browse and select a folder to scan
   - Repeat for additional directories

3. **Start Scanning**
   - Click the **"Start Scan"** button
   - Watch the progress as directories are scanned
   - The status bar shows files scanned and current directory

4. **Review Results**
   - The left panel shows **duplicate sets** (groups of identical files)
   - Each set displays:
     - Number of duplicate files
     - File size
     - Wasted disk space

5. **Manage Duplicates**
   - Click on a duplicate set to see all identical files
   - The right panel shows file details:
     - File name
     - Directory location
     - Creation date
     - Last modified date
   
6. **Take Action**
   
   **Option 1: Manual Selection**
   - Click **"Open"** to view the file location in Windows Explorer
   - Click **"Delete"** to remove a specific file
   - You'll be asked to confirm before deletion
   
   **Option 2: Automatic Cleanup**
   - Click **"Keep Oldest, Delete Others"** to:
     - Keep the file with the earliest creation date
     - Delete all other duplicates in that set
   - This is useful when you want to preserve the original file

## Features

### Smart Duplicate Detection
DupeFerret uses a 3-stage algorithm:
1. Groups files by size (fast)
2. Compares SHA512 hash of first 512 bytes (medium)
3. Compares SHA512 hash of entire file (thorough)

Only files that match all three stages are considered duplicates.

### Safety Features
- Confirmation dialogs before deletion
- Files are permanently deleted (not moved to Recycle Bin)
- Real-time UI updates after deletions
- Automatic removal of duplicate sets with only one file remaining

### Automatic Filtering
DupeFerret automatically skips:
- Hidden files (starting with `.`)
- Zero-byte files
- Files larger than 2 GB
- Inaccessible files (permission errors)

## Tips

- **Scan multiple drives**: Add directories from different drives to find duplicates across locations
- **Keep backups**: Consider backing up important files before bulk deletion
- **Review before deleting**: Check the file details carefully, especially creation dates
- **Network drives**: Works with network shares, but may be slower
- **Large scans**: For directories with many files, the initial scan may take several minutes

## Keyboard Shortcuts

- The UI uses standard Windows controls
- Use Tab to navigate between controls
- Press Enter on focused buttons to activate them

## Status Bar Information

- **Files Scanned**: Total unique files found during scan
- **Duplicates**: Number of duplicate files found
- **Wasted Space**: Total disk space used by duplicate files

## Troubleshooting

**Q: Why aren't all my files showing up?**
A: Check if files are hidden (start with `.`), empty, or larger than 2GB.

**Q: The scan is slow**
A: Large directories and network drives take time. File hashing is CPU-intensive.

**Q: Can I undo deletions?**
A: No, files are permanently deleted. Always review carefully before deleting.

**Q: What if I accidentally close the application during a scan?**
A: You'll need to restart the scan. Results are not saved between sessions.

## Safety Notice

⚠️ **Important**: Deleted files are permanently removed from disk. They are not sent to the Recycle Bin. Always verify you're deleting the correct files before confirming the deletion.
