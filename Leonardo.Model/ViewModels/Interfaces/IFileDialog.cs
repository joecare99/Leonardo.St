using System;

namespace Leonardo.ViewModels.Interfaces;

public interface IFileDialog : IDisposable
{
    string Filter { get; set; }
    string? FileName { get; set; }

    bool ShowDialog();
}