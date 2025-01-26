using Avalonia.Controls;
using Leonardo.ViewModels.Interfaces;
using System;

namespace Leonardo.Ava.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (DataContext is ILeonardoViewModel vm)
            {
                vm.ShowFileDialog = (s)=>s.ShowDialog();
                vm.MessageBoxShow = (s)=> this.FindControl<TextBlock>("txtMessage").Text = s;
                vm.InputShowDialog = (s) => { return "" };
            }
        }
    }
}