using CommunityToolkit.Mvvm.DependencyInjection;
using Leonardo.ViewModels.Interfaces;
using Leonardo.Views;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Leonardo.Views
{
    /// <summary>
    /// Interaktionslogik für LeonardoView.xaml
    /// </summary>
    public partial class LeonardoView : Page
    {
        public LeonardoView()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            ILeonardoViewModel _vm;
            DataContext = _vm = Ioc.Default.GetRequiredService<ILeonardoViewModel>();
            _vm.ShowFileDialog = (s) => s.ShowDialog();
            _vm.MessageBoxShow = (s) => MessageBox.Show(s);
            _vm.InputShowDialog = (s) => InputBox.ShowDialog(s);
            base.OnInitialized(e);
        }
    }
}
