using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using Leonardo.Ava.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Leonardo.Ava.Views;

public partial class InputDialog : UserControl
{
    public InputDialog()
    {
        InitializeComponent();
        if (this.DataContext is InputDialogViewModel vm)
        {
            vm.Close = (b)=> { };  
        }
    }

    public static async Task<string> ShowDialog(Window owner, string arg)
    {
        var window = new InputDialog()
        {
            DataContext = new InputDialogViewModel()
            {
                Message = arg
            }
        };
        var c = owner.Content;
        owner.Content = window;
        while ((window.DataContext as InputDialogViewModel)?.Input == null)
        {
            Thread.Sleep(10);
        }
        owner.Content = c;  
        return (window.DataContext as InputDialogViewModel)?.Input;
    }
}