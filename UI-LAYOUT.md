# DupeFerret UI Screenshots & Layout

## Main Window Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ DupeFerret - Duplicate File Finder                        [_][□][X]│
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│ ╔═══════════════ Directories to Scan ═══════════════════════╗  │
│ ║                                                            ║  │
│ ║  [Add Directory] [Remove Selected] [🟢 Start Scan]       ║  │
│ ║                                                            ║  │
│ ║  ┌──────────────────────────────────────────────────┐    ║  │
│ ║  │ C:\Photos                                        │    ║  │
│ ║  │ D:\Documents                                     │    ║  │
│ ║  │ E:\Backup                                        │    ║  │
│ ║  └──────────────────────────────────────────────────┘    ║  │
│ ║                                                            ║  │
│ ║  Scanning: C:\Photos\Vacation\2024\...                   ║  │
│ ╚════════════════════════════════════════════════════════════╝  │
│                                                                 │
│ ┌──────────────────┬─┬─────────────────────────────────────┐  │
│ │ Duplicate Sets   │ │ Files in Selected Set               │  │
│ ├──────────────────┤ │                                     │  │
│ │Files│Size │Wasted│ │ [🔴 Keep Oldest, Delete Others]    │  │
│ ├─────┼─────┼──────┤ ├─────────────────────────────────────┤  │
│ │  5  │2.3MB│9.2MB │ │FileName  │Directory │Created │Mod  │  │
│ │  3  │1.5MB│3.0MB │ │ IMG_123  │C:\Photos │2024-01 │[...] │  │
│ │  2  │856KB│856KB │ │ IMG_123  │D:\Backup │2024-02 │[...] │  │
│ │  7  │245KB│1.5MB │ │ IMG_123  │E:\Backup │2024-03 │[...] │  │
│ │  4  │128KB│384KB │ │                                     │  │
│ │  2  │64KB │64KB  │ │ [Open] [Delete] for each file      │  │
│ └─────┴─────┴──────┘ └─────────────────────────────────────┘  │
│ Click a set to see files. Files in each set are identical.    │
└─────────────────────────────────────────────────────────────────┤
│ ● Scan complete. Found 23 duplicate files in 5 sets.           │
│                    Files Scanned: 1,234 | Duplicates: 23 | ... │
└─────────────────────────────────────────────────────────────────┘
```

## Key UI Components

### Top Section - Directory Management
- **Add Directory Button**: Opens folder browser dialog
- **Remove Selected Button**: Removes selected directory from scan list
- **Start Scan Button**: Begins the duplicate detection process (green, bold)
- **Directory List**: Shows all directories queued for scanning
- **Status Line**: Displays current directory being scanned

### Middle Section - Results Display

#### Left Panel: Duplicate Sets
- Displays groups of duplicate files
- Columns:
  - **Files**: Number of duplicate files in the set
  - **Size**: Size of each file in the set
  - **Wasted**: Total wasted space (size × (count - 1))
- Click a row to view files in that set
- Sorted by wasted space (largest first)

#### Right Panel: Files in Selected Set
- Shows individual files in the selected duplicate set
- **Keep Oldest, Delete Others Button**: Automated cleanup
- Columns:
  - **File Name**: Name of the file
  - **Directory**: Full directory path
  - **Created**: File creation timestamp
  - **Modified**: Last write timestamp
- **Actions per file**:
  - **Open**: Opens Windows Explorer at file location
  - **Delete**: Removes individual file with confirmation

### Bottom Section - Status Bar
- **Left Side**: Current status message
- **Right Side**: Statistics
  - Files Scanned: Total unique files examined
  - Duplicates: Number of duplicate files found
  - Wasted Space: Total bytes used by duplicates

## Color Scheme
- **Start Scan Button**: Light green background (action button)
- **Delete Buttons**: Light coral background (warning)
- **Status Text**: Bold for emphasis
- **Selected Rows**: Standard Windows selection highlight

## Behavior

### During Scan
- Start Scan button disabled
- Current directory updates in real-time
- Files scanned counter increments

### After Scan
- Duplicate sets populate left panel
- Click any set to see file details
- Delete actions update UI immediately
- Sets with ≤1 file are auto-removed

### Safety Features
- All delete operations show confirmation dialog
- Detailed file information before deletion
- No undo - files are permanently deleted
- Visual distinction for destructive actions (red buttons)
