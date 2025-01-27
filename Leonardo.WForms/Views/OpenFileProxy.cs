using Leonardo.ViewModels.Interfaces;
using System;
using System.Windows.Forms;
public class OpenFileProxy : IOpenFileDialog
{
    FileDialog fileDialog = new OpenFileDialog();
    public string Filter { get => fileDialog.Filter; set => fileDialog.Filter=value; }
    public string FileName { get => fileDialog.FileName; set => fileDialog.FileName=value; }

    public void Dispose()
    {

    }

    public bool ShowDialog()
    {
        return fileDialog.ShowDialog() == DialogResult.OK;
    }
}