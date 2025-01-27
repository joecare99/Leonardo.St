using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Leonardo.Ava.ViewModels;

public partial class InputDialogViewModel : ObservableObject
{
    public Action<bool>? Close { get; set; }

    [ObservableProperty]
    string _message = "";

    [ObservableProperty]
    string _input = "";

    [RelayCommand]
    public void Ok()
    {
        Close(true);
    }

    [RelayCommand]
    public void Cancel()
    {
        Close(false);
    }
}
