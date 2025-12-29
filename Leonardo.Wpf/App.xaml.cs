using BaseLib.Interfaces;
using BaseLib.Models;
using CommunityToolkit.Mvvm.DependencyInjection;
using Leonardo.Models;
using Leonardo.Models.Interfaces;
using Leonardo.ViewModels;
using Leonardo.ViewModels.Interfaces;
using Leonardo.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Leonardo.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection()
                .AddSingleton<ILeonardoClass, LeonardoClass>()
                .AddTransient<ILeonardoViewModel, LeonardoViewModel>()
                .AddSingleton<IOpenFileDialog, OpenFileProxy>()
                .AddSingleton<ISaveFileDialog, SaveFileProxy>()
                .AddSingleton<ISteganography, Steganography>()
                .AddTransient<IHttpClient, HttpClientProxy>()
                .AddTransient<IConsole, ConsoleProxy>();

            Services = services.BuildServiceProvider();
            Ioc.Default.ConfigureServices(Services);

            base.OnStartup(e);
        }

        IServiceProvider? Services { get; set; }
    }

}
