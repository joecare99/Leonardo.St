using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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

        var result = await window.ShowDialog<InputDialogViewModel>(owner);

        return result.Input;
    }
}