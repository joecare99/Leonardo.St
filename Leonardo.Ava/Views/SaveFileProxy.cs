using Leonardo.ViewModels.Interfaces;
using System.Windows.Forms;
using System;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using Avalonia;
using Newtonsoft.Json.Linq;
using System.Linq;

public class SaveFileProxy : ISaveFileDialog
{
    FilePickerSaveOptions fileDialog = new FilePickerSaveOptions();
    public string Filter { get => string.Join("|", fileDialog.FileTypeChoices.Select(s=>$"{s.Name}|{s.ToString()}")); set => SetDialogFilter(value); }

    public string? FileName { get ; set; }

    private void SetDialogFilter(string value)
    {
        var sp = value.Split('|');
        List<FilePickerFileType> fileTypes = new();
        for (int i = 0; i < sp.Length; i += 2)
        {
            fileTypes.Add(new(sp[i]) { Patterns = sp[i + 1].Split(' ') });
        }
        fileDialog.FileTypeChoices = fileTypes;
    }


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
        var file = provider.SaveFilePickerAsync(fileDialog).Result;

        if (file != null)
        {
            FileName = file.Path.LocalPath;
            return true;
        }

        return false;
    }

}