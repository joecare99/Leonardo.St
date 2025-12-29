using BaseLib.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Leonardo.Models.Interfaces;
using Leonardo.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Leonardo.Ava.ViewModels;

public class MainWindowViewModel: ILeonardoViewModel
{
    public IRelayCommand EncodeCommand => new RelayCommand(() => { });

    public IRelayCommand DecodeCommand => new RelayCommand(() => { });

    public IRelayCommand GenerateCommand => new RelayCommand(() => { });

    public IRelayCommand TestCommand => new RelayCommand(() => { });

    public string MessageText => "some Txt";

    public Func<IFileDialog, bool>? ShowFileDialog { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Func<string, string>? InputShowDialog { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Action<string>? MessageBoxShow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ECursor? CursorCurrent => throw new NotImplementedException();

    public Bitmap? PicBoxDECImg => throw new NotImplementedException();
   
    #region INotifyPropertyChanged Implementation
    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        if (name != null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public void SetConsole(IConsole console) { }
        
    #endregion

}