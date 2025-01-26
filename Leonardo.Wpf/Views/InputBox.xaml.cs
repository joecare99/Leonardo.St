using Leonardo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Leonardo.Views;

/// <summary>
/// Interaktionslogik für InputBox.xaml
/// </summary>
public partial class InputBox : Window
{
    public InputBox()
    {
        InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e)
    {
        DataContext = new InputBoxViewModel(); 
        base.OnInitialized(e);
        if (DataContext is InputBoxViewModel vm)
        {
            vm.Close = (b) => Close();
        }
    }

    public static string ShowDialog(string text)
    {
        var dlg = new InputBox();
        if (dlg.DataContext is InputBoxViewModel vm)
        {
            vm.Message = text;
            dlg.ShowDialog();
            return vm.Input;
        }
        return "";
    }
}