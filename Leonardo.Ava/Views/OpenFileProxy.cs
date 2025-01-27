using Leonardo.ViewModels.Interfaces;
using System;
using Avalonia;
using Avalonia.Dialogs.Internal;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System.Linq;
using System.Collections.Generic;
public class OpenFileProxy : IOpenFileDialog
{
    FilePickerOpenOptions fileDialog = new FilePickerOpenOptions();
    public string Filter { get => string.Join("|",fileDialog.FileTypeFilter.Select(s=>$"{s.Name}|{s.ToString()}")); set => SetDialogFilter(value); }

    private void SetDialogFilter(string value)
    {
        var sp=value.Split('|');
        List<FilePickerFileType> fileTypes = new();
        for (int i = 0; i < sp.Length; i += 2)
        {
            fileTypes.Add(new(sp[i]) { Patterns = sp[i + 1].Split(' ')});
        }
        fileDialog.FileTypeFilter =fileTypes;
    }

    public string FileName { get ; set ; }

    public void Dispose()
    {

    }

    public bool ShowDialog()
    {
        // See IoCFileOps project for an example of how to accomplish this.
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        fileDialog.SuggestedFileName = FileName;
        var files = provider.OpenFilePickerAsync(fileDialog).Result;

        if (files?.Count >= 1)
        {
            FileName = files[0].Path.LocalPath;
            return true;
        }

        return false;
    }
}