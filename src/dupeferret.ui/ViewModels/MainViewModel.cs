using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using dupeferret.business;
using dupeferret.ui.Models;
using Microsoft.Win32;

namespace dupeferret.ui.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly Traverser _traverser;

    [ObservableProperty]
    private ObservableCollection<string> _directories = new();

    [ObservableProperty]
    private ObservableCollection<DuplicateSet> _duplicateSets = new();

    [ObservableProperty]
    private DuplicateSet? _selectedDuplicateSet;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private int _filesScanned;

    [ObservableProperty]
    private int _duplicatesFound;

    [ObservableProperty]
    private long _totalWastedSpace;

    [ObservableProperty]
    private string _currentDirectory = string.Empty;

    public MainViewModel()
    {
        _traverser = new Traverser();
        _traverser.RaiseFoundDirectoryEvent += OnDirectoryFound;
        _traverser.RaiseDupeFoundEvent += OnDuplicateFound;
        
        // Hook into collection changes to update command state
        _directories.CollectionChanged += (s, e) => StartScanCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void AddDirectory()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select Directory to Scan",
            Multiselect = false
        };

        if (dialog.ShowDialog() == true)
        {
            string folderName = dialog.FolderName;
            if (!string.IsNullOrEmpty(folderName) && !Directories.Contains(folderName))
            {
                Directories.Add(folderName);
            }
        }
    }

    [RelayCommand]
    private void RemoveDirectory(string? directory)
    {
        if (directory != null && Directories.Contains(directory))
        {
            Directories.Remove(directory);
        }
    }

    [RelayCommand(CanExecute = nameof(CanStartScan))]
    private async Task StartScan()
    {
        IsScanning = true;
        StatusMessage = "Scanning...";
        FilesScanned = 0;
        DuplicatesFound = 0;
        TotalWastedSpace = 0;
        DuplicateSets.Clear();

        try
        {
            // Reset traverser
            _traverser.UniqueFiles.Clear();
            _traverser.FilesByLength.Clear();
            _traverser.BaseDirectories.Clear();

            // Add directories
            foreach (var dir in Directories)
            {
                _traverser.AddBaseDirectory(dir);
            }

            // Run scan in background
            await Task.Run(() =>
            {
                var dupeSets = _traverser.GetDupeSets();

                // Convert to view models
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var dupeSet in dupeSets)
                    {
                        var duplicateSet = new DuplicateSet
                        {
                            FileSize = dupeSet.First().Info.Length
                        };

                        foreach (var fileEntry in dupeSet.OrderBy(f => f.Info.CreationTime))
                        {
                            duplicateSet.Files.Add(FileEntryViewModel.FromFileEntry(fileEntry));
                        }

                        DuplicateSets.Add(duplicateSet);
                        TotalWastedSpace += duplicateSet.TotalWastedSpace;
                    }

                    FilesScanned = _traverser.UniqueFiles.Count;
                    DuplicatesFound = DuplicateSets.Sum(ds => ds.FileCount - 1);
                });
            });

            StatusMessage = $"Scan complete. Found {DuplicatesFound} duplicate files in {DuplicateSets.Count} sets.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            MessageBox.Show($"Error during scan: {ex.Message}", "Scan Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsScanning = false;
        }
    }

    private bool CanStartScan() => Directories.Count > 0 && !IsScanning;

    [RelayCommand]
    private void OpenFileLocation(FileEntryViewModel? file)
    {
        if (file != null && File.Exists(file.FullPath))
        {
            string argument = $"/select, \"{file.FullPath}\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }
    }

    [RelayCommand]
    private void DeleteFile(FileEntryViewModel? file)
    {
        if (file == null || !File.Exists(file.FullPath))
            return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete:\n\n{file.FullPath}",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                File.Delete(file.FullPath);
                
                // Remove from UI
                foreach (var dupeSet in DuplicateSets)
                {
                    var fileToRemove = dupeSet.Files.FirstOrDefault(f => f.FullPath == file.FullPath);
                    if (fileToRemove != null)
                    {
                        dupeSet.Files.Remove(fileToRemove);
                        
                        // If only one file left, remove the entire set
                        if (dupeSet.Files.Count <= 1)
                        {
                            DuplicateSets.Remove(dupeSet);
                        }
                        break;
                    }
                }

                StatusMessage = $"Deleted: {file.FileName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting file: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private void KeepOldestDeleteOthers(DuplicateSet? dupeSet)
    {
        if (dupeSet == null || dupeSet.Files.Count <= 1)
            return;

        var oldestFile = dupeSet.Files.OrderBy(f => f.CreationTime).First();
        var filesToDelete = dupeSet.Files.Where(f => f != oldestFile).ToList();

        var result = MessageBox.Show(
            $"Keep: {oldestFile.FileName}\n\nDelete {filesToDelete.Count} other file(s)?",
            "Confirm Delete Others",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            foreach (var file in filesToDelete)
            {
                try
                {
                    if (File.Exists(file.FullPath))
                    {
                        File.Delete(file.FullPath);
                        dupeSet.Files.Remove(file);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting {file.FileName}: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (dupeSet.Files.Count <= 1)
            {
                DuplicateSets.Remove(dupeSet);
            }

            StatusMessage = $"Kept oldest file, deleted {filesToDelete.Count} duplicate(s)";
        }
    }

    [RelayCommand]
    private void Reset()
    {
        DuplicateSets.Clear();
        SelectedDuplicateSet = null;
        FilesScanned = 0;
        DuplicatesFound = 0;
        TotalWastedSpace = 0;
        CurrentDirectory = string.Empty;
        StatusMessage = "Ready";
    }

    private void OnDirectoryFound(object? sender, EventMessageArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            CurrentDirectory = e.Message;
        });
    }

    private void OnDuplicateFound(object? sender, EventMessageArgs e)
    {
        // Update UI periodically
    }

    partial void OnIsScanningChanged(bool value)
    {
        StartScanCommand.NotifyCanExecuteChanged();
    }

    partial void OnDirectoriesChanged(ObservableCollection<string> value)
    {
        StartScanCommand.NotifyCanExecuteChanged();
    }
}
