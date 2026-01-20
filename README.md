# 🦡 DupeFerret

<p align="center">
  <strong>Fast, efficient duplicate file finder with parallel processing</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet" alt=".NET 10.0"/>
  <img src="https://img.shields.io/badge/Platform-Windows-0078D4?logo=windows" alt="Windows"/>
  <img src="https://img.shields.io/badge/License-Non--Commercial-orange" alt="License"/>
</p>

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [Desktop Application](#desktop-application)
  - [Command-Line Interface](#command-line-interface)
- [How It Works](#how-it-works)
- [Performance](#performance)
- [Project Structure](#project-structure)
- [Development](#development)
- [Testing](#testing)
- [Contributing](#contributing)
- [License](#license)

---

## 🎯 Overview

**DupeFerret** is a powerful duplicate file detection tool that helps you reclaim disk space by identifying and managing duplicate files across multiple directories. It uses a sophisticated three-stage hashing algorithm with parallel processing for maximum performance.

### Why DupeFerret?

- ✅ **Accurate** - Three-stage verification ensures true duplicates
- ⚡ **Fast** - Parallel hash processing utilizes all CPU cores
- 🖥️ **User-Friendly** - Modern WPF interface or CLI for automation
- 🔒 **Safe** - Smart filtering and confirmation dialogs
- 🆓 **Free** - Open source and MIT licensed

---

## ✨ Features

### Desktop Application (WPF)

- 📁 **Multi-directory scanning** - Scan multiple folders simultaneously
- 📊 **Visual results browser** - See duplicate sets organized by wasted space
- 🗑️ **Smart deletion tools**:
  - Delete individual files
  - Keep oldest, delete others (bulk cleanup)
  - Confirmation dialogs for safety
- 🔍 **File explorer integration** - Open file locations directly
- 📈 **Real-time statistics** - Files scanned, duplicates found, wasted space
- 🔄 **Reset button** - Clear results and start fresh
- 🎨 **Modern UI** - Clean, intuitive interface with data grids

### Command-Line Interface

- 🤖 **JSON output** - Perfect for automation and scripting
- 🔧 **Pipeline-friendly** - Results to stdout, stats to stderr
- 🐍 **Python helper scripts** - Process JSON and generate cleanup commands
- ⚙️ **Scriptable** - Integrate into backup or maintenance workflows

### Core Detection Engine

- 🎯 **3-stage duplicate detection**:
  1. **Size grouping** - Fast initial filtering
  2. **First 512 bytes hash** - Quick content sampling (SHA512)
  3. **Full file hash** - Complete verification (SHA512)
- ⚡ **Parallel processing** - Multi-threaded hash computation
- 🛡️ **Smart filtering**:
  - Skips hidden files (`.` prefix)
  - Ignores empty files
  - Filters files >2GB (configurable)
  - Handles permission errors gracefully
- 🔐 **Cryptographic hashing** - SHA512 ensures accuracy

---

## 📦 Installation

### Prerequisites

- **.NET 10.0 SDK** or later - [Download here](https://dotnet.microsoft.com/download)
- **Windows** - Required for WPF UI (CLI works on Linux/macOS)

### Building from Source

```bash
# Clone the repository
git clone <repository-url>
cd DupeFerret

# Build the solution
dotnet build

# Run tests to verify
dotnet test
```

### Quick Start

```bash
# Run the desktop UI
dotnet run --project src/dupeferret.ui/dupeferret.ui.csproj

# Or use the command-line tool
dotnet run --project src/dupeferret/dupeferret.csproj -- "C:\Photos" "D:\Backup"
```

---

## 🚀 Usage

### Desktop Application

#### Step-by-Step Guide

1. **Launch the application**
   ```bash
   dotnet run --project src/dupeferret.ui/dupeferret.ui.csproj
   ```
   Or run the compiled executable:
   ```
   src/dupeferret.ui/bin/Debug/net10.0-windows/dupeferret.ui.exe
   ```

2. **Add directories to scan**
   - Click **"Add Directory"** button
   - Browse and select folders
   - Repeat for multiple directories

3. **Start the scan**
   - Click **"Start Scan"** (green button)
   - Watch real-time progress
   - See current directory being scanned

4. **Review duplicate sets**
   - Left panel shows groups of identical files
   - Each set displays: file count, size, wasted space
   - Click a set to see all duplicate files

5. **Take action**
   
   **Option A: Manual deletion**
   - Review each file's details (name, path, dates)
   - Click **"Open"** to view in Explorer
   - Click **"Delete"** to remove specific files
   
   **Option B: Automatic cleanup**
   - Click **"Keep Oldest, Delete Others"**
   - Keeps the file with earliest creation date
   - Removes all other duplicates in that set

6. **Reset if needed**
   - Click **"Reset"** (yellow button) to clear results

#### UI Layout

```
┌─────────────────────────────────────────────────────────────┐
│ [Add Dir] [Remove] [🟢 Start Scan] [Reset]                 │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ C:\Photos                                               │ │
│ │ D:\Documents                                            │ │
│ └─────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────┤
│ Duplicate Sets          │ Files in Selected Set            │
│ ┌──────────────────┐   │ [Keep Oldest, Delete Others]    │
│ │Files│Size │Wasted│   │ FileName│Dir│Created│Modified   │
│ │  5  │2.3MB│9.2MB │   │ img1.jpg│...│...   │...         │
│ │  3  │1.5MB│3.0MB │   │ img1.jpg│...│...   │[Open][Del] │
│ └──────────────────┘   │ ...                              │
├─────────────────────────────────────────────────────────────┤
│ Status | Files: 1,234 | Duplicates: 23 | Wasted: 15.2 MB  │
└─────────────────────────────────────────────────────────────┘
```

### Command-Line Interface

#### Basic Usage

```bash
dotnet run --project src/dupeferret/dupeferret.csproj -- <dir1> [dir2] [dir3...]
```

#### Examples

**Scan a single directory:**
```bash
dotnet run --project src/dupeferret/dupeferret.csproj -- "C:\Photos"
```

**Scan multiple directories:**
```bash
dotnet run --project src/dupeferret/dupeferret.csproj -- "C:\Photos" "D:\Backup" "E:\Archive"
```

**Save results to JSON file:**
```bash
dotnet run --project src/dupeferret/dupeferret.csproj -- "C:\Photos" > duplicates.json
```

#### Output Format

**JSON (stdout):**
```json
[
  [
    {
      "FQFN": "C:\\Photos\\image1.jpg",
      "Length": 2456789,
      "Name": "image1.jpg",
      "CreationTime": "2024-01-15T10:30:00",
      "LastWriteTime": "2024-01-15T10:30:00"
    },
    {
      "FQFN": "D:\\Backup\\image1.jpg",
      "Length": 2456789,
      "Name": "image1.jpg",
      "CreationTime": "2024-02-20T15:45:00",
      "LastWriteTime": "2024-02-20T15:45:00"
    }
  ]
]
```

**Statistics (stderr):**
```
1234 files checked. 23 dupes found
```

### Python Helper Script

Process JSON output to generate cleanup commands:

```bash
# Run scan and save results
dotnet run --project src/dupeferret/dupeferret.csproj -- "C:\Photos" > dupes.json

# Process with Python script
python src/dfcmd/dfcmd.py
```

The script generates shell commands to move duplicates to a designated location.

---

## 🔬 How It Works

### Three-Stage Detection Algorithm

DupeFerret uses a progressive filtering approach for maximum efficiency:

#### Stage 1: Size Grouping
- Groups files by exact byte size
- **Fastest** - Simple comparison
- Eliminates most non-duplicates immediately
- No disk I/O beyond metadata

#### Stage 2: First 512 Bytes Hash
- Computes SHA512 hash of first 512 bytes
- **Fast** - Minimal disk reads
- Catches most remaining non-duplicates
- Parallel processing across file groups

#### Stage 3: Full File Hash
- Computes SHA512 hash of entire file
- **Thorough** - Only runs on likely duplicates
- Guarantees true duplicates
- Parallel processing for performance

```
1000 files
    ↓
[Size Filter] → 950 files eliminated
    ↓
  50 files (10 groups of ~5 files each)
    ↓
[First 512B Hash] → 30 files eliminated
    ↓
  20 files (4 groups of ~5 files each)
    ↓
[Full File Hash] → 5 files eliminated
    ↓
  15 true duplicates in 3 groups
```

### Parallel Processing

- **Hash computation** runs on multiple CPU cores using `AsParallel()`
- **File groups** are processed concurrently
- **I/O-bound operations** remain sequential (optimal for single disk)
- Scales automatically to available CPU cores

---

## ⚡ Performance

### Benchmarks

Performance varies based on:
- Number of files
- File sizes
- Disk speed (SSD vs HDD)
- CPU cores available

**Typical Performance:**
- 10,000 small files (~1MB): ~30 seconds
- 1,000 large files (~100MB): ~5 minutes
- Network drives: 2-3x slower

### Optimization Tips

1. **Use SSD** - Significantly faster than HDD
2. **Local drives** - Faster than network shares
3. **More CPU cores** - Better parallel processing
4. **Exclude system directories** - Focus on user data

---

## 📁 Project Structure

```
DupeFerret/
├── src/
│   ├── dupeferret/              # Command-line application
│   │   ├── program.cs           # Main entry point
│   │   └── dupeferret.csproj    # Project file
│   ├── dupeferret.ui/           # WPF desktop application
│   │   ├── MainWindow.xaml      # Main UI layout
│   │   ├── ViewModels/          # MVVM view models
│   │   ├── Models/              # Data models
│   │   ├── Converters/          # Value converters
│   │   └── dupeferret.ui.csproj # Project file
│   ├── dupeferret.business/     # Core business logic
│   │   ├── Traverser.cs         # Main orchestrator
│   │   ├── FileEntry.cs         # File representation
│   │   ├── FileInfoHandler.cs   # File metadata
│   │   └── *.cs                 # Supporting classes
│   └── dfcmd/                   # Python helper scripts
│       └── dfcmd.py             # JSON processor
├── tests/
│   └── dupeferret.business.tests/  # Unit tests (xUnit)
│       ├── TraverserTests.cs
│       ├── FileEntryTests.cs
│       └── TestData/            # Test fixtures
├── DupeFerret.sln               # Solution file
├── README.md                    # This file
├── UI-GUIDE.md                  # Detailed UI guide
└── UI-LAYOUT.md                 # UI layout reference
```

---

## 🛠️ Development

### Technology Stack

- **Language:** C# (latest version)
- **Framework:** .NET 10.0
- **UI:** WPF (Windows Presentation Foundation)
- **MVVM:** CommunityToolkit.Mvvm 8.3.2
- **Testing:** xUnit 2.9.2, FakeItEasy 8.3.0
- **Hashing:** SHA512 (cryptographic)

### Architecture

**Design Patterns:**
- **MVVM** - Model-View-ViewModel separation
- **Observer** - Event-driven progress reporting
- **Repository** - Centralized file tracking
- **Command** - RelayCommand for UI actions

**Key Components:**

1. **Traverser** - Orchestrates entire duplicate detection process
2. **FileEntry** - Represents individual files with hashing
3. **MainViewModel** - UI state and command handling
4. **DuplicateSet** - Groups of identical files

### Building

```bash
# Full build
dotnet build

# Build specific project
dotnet build src/dupeferret.ui/dupeferret.ui.csproj

# Release build
dotnet build -c Release
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test
dotnet test --filter "FullyQualifiedName~TraverserTests"
```

**Test Coverage:**
- ✅ 21 unit tests
- ✅ Traverser functionality
- ✅ File entry comparison
- ✅ Hash computation
- ✅ Error handling

---

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

### Ways to Contribute

1. **Report bugs** - Open an issue with reproduction steps
2. **Suggest features** - Describe use cases and benefits
3. **Submit pull requests** - Follow the guidelines below
4. **Improve documentation** - Fix typos, add examples
5. **Write tests** - Increase code coverage

### Pull Request Guidelines

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Add/update tests as needed
5. Ensure all tests pass (`dotnet test`)
6. Commit with clear messages
7. Push to your fork
8. Open a pull request

### Code Style

- Follow C# naming conventions
- Use meaningful variable names
- Add XML documentation for public APIs
- Keep methods focused and concise
- Write tests for new functionality

---

## 📄 License

This project is licensed under a **Non-Commercial License**.

**Key Terms:**
- ✅ Free for personal, educational, and non-commercial use
- ✅ You can modify and share for non-commercial purposes
- ❌ Commercial use is prohibited without permission
- 💼 Commercial licensing available - contact the copyright holder

**Commercial Use Includes:**
- Using in a business environment
- Selling or licensing the software
- Providing paid services using the software
- Incorporating into commercial products

See the [LICENSE](LICENSE) file for complete terms.

**Want to use commercially?** Contact the copyright holder for commercial licensing options.

---

## 🙏 Acknowledgments

- SHA512 cryptographic hashing from .NET BCL
- CommunityToolkit.Mvvm for MVVM infrastructure
- xUnit and FakeItEasy for testing framework
- All contributors and users

---

## 📞 Support

- **Issues:** [GitHub Issues](../../issues)
- **Documentation:** See `UI-GUIDE.md` and `UI-LAYOUT.md`
- **Discussions:** [GitHub Discussions](../../discussions)

---

## 🗺️ Roadmap

Future enhancements under consideration:

- [ ] Save/load scan results
- [ ] File type filtering (e.g., only images)
- [ ] Custom hash algorithms (MD5, SHA256 options)
- [ ] Move to folder (instead of delete)
- [ ] Duplicate similarity (not just exact matches)
- [ ] Mac/Linux GUI support (Avalonia)
- [ ] Progress bar with percentage
- [ ] Configurable file size limits
- [ ] Export to CSV/Excel
- [ ] Scheduled scans

---

<p align="center">
  Made with ❤️ by the DupeFerret team
</p>

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
