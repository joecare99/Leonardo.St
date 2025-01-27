using System.Windows.Forms;

namespace Views;

public class InputDialog
    {
        public static string ShowDialog(string prompt)
        {
            Form promptForm = new Form();
            promptForm.Width = 500;
            promptForm.Height = 150;
            promptForm.Text = prompt;
            TextBox textBox = new TextBox
            {
                Left = 50,
                Top = 50,
                Width = 400
            };
            Button button = new Button
            {
                Text = "OK",
                Left = 350,
                Width = 100,
                Top = 70,
                DialogResult = DialogResult.OK
            };
            button.Click += delegate
            {
                promptForm.Close();
            };
            promptForm.Controls.Add(textBox);
            promptForm.Controls.Add(button);
            promptForm.AcceptButton = button;
            if (promptForm.ShowDialog() != DialogResult.OK)
            {
                return "";
            }
            return textBox.Text;
        }
    }
