using Leonardo.ViewModels.Interfaces;
using System.Windows.Forms;
using System;

public class SaveFileProxy : ISaveFileDialog
{
    FileDialog fileDialog = new SaveFileDialog();
    public string Filter { get => fileDialog.Filter; set => fileDialog.Filter = value; }
    public string FileName { get => fileDialog.FileName; set => fileDialog.FileName = value; }

    public void Dispose()
    {
    }

    public bool ShowDialog()
    {
        return fileDialog.ShowDialog() == DialogResult.OK;
    }
}