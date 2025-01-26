using Leonardo.ViewModels.Interfaces;
using System;
using System.Threading;
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
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            return fileDialog.ShowDialog() == DialogResult.OK;
        else
        {
            bool result = false;
            Thread thread = new Thread(() => result = fileDialog.ShowDialog() == DialogResult.OK);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return result;
        }
    }
}