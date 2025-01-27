using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;
using Leonardo.ViewModels.Interfaces;
using Leonardo.ViewModels;
using Leonardo.Models.Interfaces;
using Leonardo.Models;
using BaseLib.Interfaces;
using BaseLib.Models;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Init();
        Application.Run(Ioc.Default.GetRequiredService<Views.LeonardoView>());
    }

    private static void Init()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(defaultValue: false);

        var sp = new ServiceCollection()
            .AddSingleton<ILeonardoClass, LeonardoClass>()
            .AddTransient<ILeonardoViewModel, LeonardoViewModel>()
            .AddSingleton<IOpenFileDialog, OpenFileProxy>()
            .AddSingleton<ISaveFileDialog, SaveFileProxy>()
            .AddSingleton<ISteganography, Steganography>()
            .AddTransient<IHttpClient, HttpClientProxy>()
            .AddTransient<IConsole, ConsoleProxy>()
            .AddTransient<Views.LeonardoView, Views.LeonardoView>()
            .AddTransient<Views.InputDialog, Views.InputDialog>()
            .AddTransient<Views.LoadingDialog, Views.LoadingDialog>()
            .BuildServiceProvider();

        Ioc.Default.ConfigureServices(sp);
    }
}
