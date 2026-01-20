using System.Collections.ObjectModel;
using System.IO;
using dupeferret.business;

namespace dupeferret.ui.Models;

public class DuplicateSet
{
    public ObservableCollection<FileEntryViewModel> Files { get; set; } = new();
    public long FileSize { get; set; }
    public int FileCount => Files.Count;
    public long TotalWastedSpace => FileSize * (FileCount - 1);
    public string FormattedSize => FormatBytes(FileSize);
    public string FormattedWastedSpace => FormatBytes(TotalWastedSpace);

    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

public class FileEntryViewModel
{
    public string FullPath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Directory { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime LastWriteTime { get; set; }
    public bool IsMarkedForDeletion { get; set; }

    public static FileEntryViewModel FromFileEntry(ISimpleFileEntry entry)
    {
        return new FileEntryViewModel
        {
            FullPath = entry.FQFN,
            FileName = entry.Name,
            Directory = Path.GetDirectoryName(entry.FQFN) ?? string.Empty,
            Size = entry.Length,
            CreationTime = entry.CreationTime,
            LastWriteTime = entry.LastWriteTime
        };
    }
}
