using BaseLib.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Leonardo.Models.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Leonardo.ViewModels.Interfaces;

public interface ILeonardoViewModel : INotifyPropertyChanged
{
    IRelayCommand EncodeCommand { get; }
    IRelayCommand DecodeCommand { get; }
    IRelayCommand GenerateCommand { get; }
    IRelayCommand TestCommand { get; }

    string MessageText { get; }
    Func<IFileDialog, bool>? ShowFileDialog { get; set; }
    Func<string, string>? InputShowDialog { get; set; }
    Action<string>? MessageBoxShow { get; set; }
    ECursor? CursorCurrent { get; }
    Bitmap? PicBoxDECImg { get; }
    void SetConsole(IConsole console);
}