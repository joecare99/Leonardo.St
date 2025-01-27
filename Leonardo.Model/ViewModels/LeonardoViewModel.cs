using System;
using System.ComponentModel;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using Leonardo.Models.Interfaces;
using Leonardo.ViewModels.Interfaces;
using BaseLib.Interfaces;

namespace Leonardo.ViewModels;

public partial class LeonardoViewModel : ObservableObject, ILeonardoViewModel
{
    private ILeonardoClass _model; 
    public Func<IFileDialog,bool>? ShowFileDialog { get; set; }
    public Func<string, string>? InputShowDialog { get; set; }
    public Action<string>? MessageBoxShow { get; set; }

    [ObservableProperty]
    private ECursor? _cursorCurrent; 

    [ObservableProperty]
    private string _messageText;

    [ObservableProperty]
    private Bitmap? _picBoxDECImg;

    public LeonardoViewModel()
    {
        _model = null!;
    }
    public LeonardoViewModel(ILeonardoClass model)
    {
        _model = model;
        _model.PropertyChanged += OnModelPropertyChanged;
        _model.SaveFileQuery = SaveFileQuery;
        _model.MessageBoxShow = (string message) => MessageBoxShow?.Invoke(message);
        _model.InputQuery = (string message) => InputShowDialog?.Invoke(message);
        MessageText = _model.MessageText;
        PicBoxDECImg = _model.PicBoxDECImg;
    }

    private string? SaveFileQuery(string arg)
    {
        using IFileDialog saveFileDialog = Ioc.Default.GetRequiredService<ISaveFileDialog>();
        saveFileDialog.Filter = arg;
        if (ShowFileDialog?.Invoke(saveFileDialog) ?? false)
        {
            return saveFileDialog.FileName;
        }
        return null;
    }

    private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_model.MessageText):
                MessageText = _model.MessageText;
                break;
            case nameof(_model.PicBoxDECImg):
                PicBoxDECImg = _model.PicBoxDECImg;
                break;
        }
    }

    [RelayCommand()]
    private void Encode()
    {
        using IFileDialog openFileDialog = Ioc.Default.GetRequiredService<IOpenFileDialog>();
        openFileDialog.Filter = "Image Files (*.png, *.jpg)|*.png;*.jpg;";
        if (ShowFileDialog?.Invoke(openFileDialog) ?? false )
        {
            string fileName = openFileDialog.FileName;
            _model.PromptAndEncrypt(fileName);
        }
    }

    [RelayCommand()]
    private void Decode()
    {
        using IFileDialog openFileDialog = Ioc.Default.GetRequiredService<IOpenFileDialog>();
        openFileDialog.Filter = "Image Files (*.png, *.jpg)|*.png;*.jpg;";
        if (ShowFileDialog?.Invoke(openFileDialog) ?? false)
        {
            string fileName = openFileDialog.FileName;
            _model.PromptAndDecrypt(fileName);
        }
    }

    [RelayCommand()]
    private void Generate()
    {
        try
        {
            CursorCurrent = ECursor.WaitCursor;
            string text = InputShowDialog("SD prompt");
            if (!string.IsNullOrEmpty(text))
            {
                _model.HuggingRequest2ENC(text);
            }
            else
            {
                MessageBoxShow("Please enter a string to encrypt.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    [RelayCommand()]
    private async void Test()
    {
        await _model.HuggingRequest();
    }

    public void SetConsole(IConsole console) => _model.SetConsole(console);
}