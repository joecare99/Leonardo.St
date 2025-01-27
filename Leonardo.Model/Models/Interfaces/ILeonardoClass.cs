using BaseLib.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;

namespace Leonardo.Models.Interfaces;

public interface ILeonardoClass: INotifyPropertyChanged
{
    Func<string, string?>? SaveFileQuery { get; set; }
    Action<string>? MessageBoxShow { get; set; }

    Bitmap? PicBoxDECImg { get; }
    string MessageText { get; }
    Func<string, string>? InputQuery { get; set; }
    Action? ShowGeneratingMessage { get; set; }
    Action? HideGeneratingMessage { get; set; }

    Task HuggingRequest();
    Task HuggingRequest2ENC(string text);
    void PromptAndDecrypt(string fileName);
    void PromptAndEncrypt(string fileName);
    void SetConsole(IConsole console);
}