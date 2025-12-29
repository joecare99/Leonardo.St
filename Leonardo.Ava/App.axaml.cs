using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using BaseLib.Interfaces;
using BaseLib.Models;
using CommunityToolkit.Mvvm.DependencyInjection;
using Leonardo.Ava.Views;
using Leonardo.Models;
using Leonardo.Models.Interfaces;
using Leonardo.ViewModels;
using Leonardo.ViewModels.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Leonardo.Ava;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            InitDesktopApp(desktop);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void InitDesktopApp(IClassicDesktopStyleApplicationLifetime desktop)
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

        // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
        // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
        DisableAvaloniaDataAnnotationValidation();
        desktop.MainWindow = new MainWindow
        {
            DataContext =  Ioc.Default.GetRequiredService<ILeonardoViewModel>()
        };
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    public IServiceProvider? Services { get; private set; }
}