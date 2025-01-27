using System.Windows.Forms;

namespace Views;

public class LoadingDialog
    {
        public static void ShowDialog()
        {
            Form form = new Form();
            form.Width = 300;
            form.Height = 120;
            form.Text = "Generating...";
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            ProgressBar value = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                Width = 250,
                Height = 20,
                Left = 25,
                Top = 20,
                Visible = true
            };
            Label value2 = new Label
            {
                Text = "Generating...",
                Left = 25,
                Top = 50,
                Width = 250,
                Visible = true
            };
            form.Controls.Add(value);
            form.Controls.Add(value2);
            form.ShowDialog();
        }

        public static void CloseDialog()
        {
            Form.ActiveForm?.Close();
        }
    }
