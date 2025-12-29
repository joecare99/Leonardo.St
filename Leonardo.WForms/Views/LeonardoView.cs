using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Leonardo.Properties;
using Leonardo.ViewModels.Interfaces;

namespace Views;
public partial class LeonardoView : Form
{
    private ILeonardoViewModel _vm;

    public LeonardoView(ILeonardoViewModel model)
    {
        //IL_0036: Unknown result type (might be due to invalid IL or missing references)
        //IL_0040: Expected O, but got Unknown
        InitializeComponent();
        this.Tag = _vm= model;
        _vm.PropertyChanged += OnVMPropertyChanged;
        _vm.EncodeCommand.CanExecuteChanged += (sender, e) => EncBTN.Enabled = _vm.EncodeCommand.CanExecute(null);
        _vm.DecodeCommand.CanExecuteChanged += (sender, e) => DecBTN.Enabled = _vm.DecodeCommand.CanExecute(null);
        _vm.GenerateCommand.CanExecuteChanged += (sender, e) => GallBTN.Enabled = _vm.GenerateCommand.CanExecute(null);
        _vm.ShowFileDialog = (dialog) => dialog.ShowDialog();
        _vm.MessageBoxShow = (message) => MessageBox.Show(message);
        _vm.InputShowDialog = (message) => InputDialog.ShowDialog(message);
        DECMsgLabel.Text =_vm.MessageText;

        DECuploadBTN.FlatAppearance.MouseOverBackColor = Color.Transparent;
        DecBTN.FlatAppearance.MouseOverBackColor = Color.Transparent;
        EncBTN.FlatAppearance.MouseOverBackColor = Color.Transparent;
        UploadBTN.FlatAppearance.MouseOverBackColor = Color.Transparent;
        GallBTN.FlatAppearance.MouseOverBackColor = Color.Transparent;
        CopyToClipboardButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
        DECuploadBTN.MouseDown += Button_MouseDown;
        DecBTN.MouseDown += Button_MouseDown;
        EncBTN.MouseDown += Button_MouseDown;
        UploadBTN.MouseDown += Button_MouseDown;
        GallBTN.MouseDown += Button_MouseDown;
        CopyToClipboardButton.MouseDown += Button_MouseDown;
        DECuploadBTN.MouseHover += Button_MouseHover;
        DECuploadBTN.MouseLeave += Button_MouseLeave;
    }

    private void OnVMPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_vm.MessageText))
        {
            DECMsgLabel.Text = _vm.MessageText;
        }
        if (e.PropertyName == nameof(_vm.CursorCurrent))
        {
            Cursor = _vm.CursorCurrent switch { Leonardo.Models.Interfaces.ECursor.WaitCursor => Cursors.WaitCursor,
                Leonardo.Models.Interfaces.ECursor.Default => Cursors.Default,
                _ => throw new NotImplementedException()
            };
        }
        if (e.PropertyName == nameof(_vm.PicBoxDECImg))
        {
            pictureBox1.Image = _vm.PicBoxDECImg;
        }
    }

    private void Button_MouseDown(object sender, MouseEventArgs e)
    {
        if (sender is Button button)
        {
            button.FlatAppearance.MouseDownBackColor = Color.Transparent;
        }
    }

    private void EncBTN_Click(object sender, EventArgs e) => _vm.EncodeCommand.Execute(null);
    private void DecBTN_Click(object sender, EventArgs e) => _vm.DecodeCommand.Execute(null);
    private async void GallBTN_Click(object sender, EventArgs e) => _vm.GenerateCommand.Execute(null);
    private async void testbtn_Click(object sender, EventArgs e) => _vm.TestCommand.Execute(null);

    private void DECuploadBTN_Click(object sender, EventArgs e) => Process.Start("https://github.com/2alf/Leonardo");

    private void UploadBTN_Click(object sender, EventArgs e)
    {
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
    }

    private void CopyToClipboardButton_Click(object sender, EventArgs e)
    {
        Clipboard.SetText(DECMsgLabel.Text);
        MessageBox.Show("copied to clipboard!");
    }

    private void DECuploadBTN_MouseHover(object sender, EventArgs e) => DECuploadBTN.BackgroundImage = Resources.btneyehover2;
    private void DECuploadBTN_MouseLeave(object sender, EventArgs e) => DECuploadBTN.BackgroundImage = Resources.btnleaver2;
   private void DecBTN_MouseMove(object sender, MouseEventArgs e)    {    }
    private void DecBTN_MouseHover(object sender, EventArgs e) => DecBTN.BackgroundImage = Resources.leo_0000s_0001_Layer_16_copy;
    private void DecBTN_MouseLeave(object sender, EventArgs e) => DecBTN.BackgroundImage = Resources.leo_0001s_0001_Layer_16_copy;
    private void GallBTN_MouseHover(object sender, EventArgs e) => GallBTN.BackgroundImage = Resources.leo_0000s_0000_gen_copy;
    private void GallBTN_MouseLeave(object sender, EventArgs e) => GallBTN.BackgroundImage = Resources.leo_0001s_0000_gen_copy;
    private void EncBTN_MouseHover(object sender, EventArgs e) => EncBTN.BackgroundImage = Resources.leo_0000s_0002_Layer_15_copy_2;
    private void EncBTN_MouseLeave(object sender, EventArgs e) => EncBTN.BackgroundImage = Resources.leo_0001s_0002_Layer_15_copy_2;
    private void CopyToClipboardButton_MouseEnter(object sender, EventArgs e)    {    }
    private void CopyToClipboardButton_MouseHover(object sender, EventArgs e) => CopyToClipboardButton.BackgroundImage = Resources.BLUEHOVER;
    private void CopyToClipboardButton_MouseLeave(object sender, EventArgs e) => CopyToClipboardButton.BackgroundImage = Resources.GREENHOVER;

    private void Timer1_Tick(object sender, EventArgs e) => base.Opacity += 0.1;

    private void Form1_Load(object sender, EventArgs e) => timer1.Start();

    private void Button_MouseHover(object sender, EventArgs e)
    {
        Button button = sender as Button;
        if (button != null)
        {
            timer = new Timer();
            timer.Interval = 20;
            timer.Tick += delegate
            {
                FadeInTimer_Tick(button);
            };
            timer.Start();
        }
    }

    private void Button_MouseLeave(object sender, EventArgs e)
    {
        Button button = sender as Button;
        if (button != null)
        {
            timer = new Timer();
            timer.Interval = 20;
            timer.Tick += delegate
            {
                FadeOutTimer_Tick(button);
            };
            timer.Start();
        }
    }

    private void FadeInTimer_Tick(Button button)
    {
        if (button.BackgroundImage != null && button.BackgroundImage.Tag != null)
        {
            double num = (double)button.BackgroundImage.Tag;
            if (num < targetOpacity)
            {
                num += opacityIncrement;
                button.BackgroundImage.Tag = num;
                button.BackgroundImage = ChangeOpacity((Bitmap)button.BackgroundImage, num);
            }
            else
            {
                timer.Stop();
            }
        }
    }

    private void FadeOutTimer_Tick(Button button)
    {
        if (button.BackgroundImage != null && button.BackgroundImage.Tag != null)
        {
            double num = (double)button.BackgroundImage.Tag;
            if (num > 0.0)
            {
                num -= opacityIncrement;
                button.BackgroundImage.Tag = num;
                button.BackgroundImage = ChangeOpacity((Bitmap)button.BackgroundImage, num);
            }
            else
            {
                timer.Stop();
            }
        }
    }

    private Bitmap ChangeOpacity(Bitmap originalImage, double opacity)
    {
        Bitmap bitmap = new Bitmap(originalImage.Width, originalImage.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        ColorMatrix colorMatrix = new ColorMatrix();
        colorMatrix.Matrix33 = (float)opacity;
        ImageAttributes imageAttributes = new ImageAttributes();
        imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        graphics.DrawImage(originalImage, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel, imageAttributes);
        return bitmap;
    }

    static void HideGeneratingMessage()
    {
        Form[] array = Application.OpenForms.Cast<Form>().ToArray();
        foreach (Form form2 in array)
        {
            if (form2.Text == "Please Wait")
            {
                form2.Close();
                break;
            }
        }
    }
    static void ShowGeneratingMessage()
    {
        MessageBox.Show("Generated", "Please Wait", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        Form[] array2 = Application.OpenForms.Cast<Form>().ToArray();
        foreach (Form form3 in array2)
        {
            if (form3.Text == "Please Wait")
            {
                form3.Close();
                break;
            }
        }
    }

}
