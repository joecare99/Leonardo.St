using System.Net.Http;
using System.Windows.Forms;
using System;
using System.ComponentModel;

namespace Views;

public partial class LeonardoView
{
    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new Container();
        ComponentResourceManager resources = new ComponentResourceManager(typeof(LeonardoView));
        EncBTN = new Button();
        DecBTN = new Button();
        GallBTN = new Button();
        DecryptPanel = new Panel();
        progressBar1 = new ProgressBar();
        CopyToClipboardButton = new Button();
        DECMsgLabel = new Label();
        PicBoxDEC = new PictureBox();
        DECuploadBTN = new Button();
        pictureBox1 = new PictureBox();
        GalleryPanel = new Panel();
        MenuPanel = new Panel();
        EncryptPanel = new Panel();
        UploadBTN = new Button();
        timer1 = new Timer(components);
        testbtn = new Button();
        DecryptPanel.SuspendLayout();
        ((ISupportInitialize)PicBoxDEC).BeginInit();
        ((ISupportInitialize)pictureBox1).BeginInit();
        EncryptPanel.SuspendLayout();
        SuspendLayout();
        // 
        // EncBTN
        // 
        EncBTN.BackColor = System.Drawing.Color.Transparent;
        EncBTN.BackgroundImage = Leonardo.Properties.Resources.leo_0001s_0002_Layer_15_copy_2;
        EncBTN.BackgroundImageLayout = ImageLayout.None;
        EncBTN.FlatAppearance.BorderSize = 0;
        EncBTN.FlatStyle = FlatStyle.Flat;
        EncBTN.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F);
        EncBTN.ForeColor = System.Drawing.Color.Linen;
        EncBTN.Location = new System.Drawing.Point(135, 757);
        EncBTN.Margin = new Padding(0);
        EncBTN.Name = "EncBTN";
        EncBTN.Size = new System.Drawing.Size(224, 120);
        EncBTN.TabIndex = 0;
        EncBTN.UseVisualStyleBackColor = false;
        EncBTN.Click += EncBTN_Click;
        EncBTN.MouseLeave += EncBTN_MouseLeave;
        EncBTN.MouseHover += EncBTN_MouseHover;
        // 
        // DecBTN
        // 
        DecBTN.BackColor = System.Drawing.Color.Transparent;
        DecBTN.BackgroundImage = Leonardo.Properties.Resources.leo_0001s_0001_Layer_16_copy;
        DecBTN.BackgroundImageLayout = ImageLayout.None;
        DecBTN.FlatAppearance.BorderSize = 0;
        DecBTN.FlatStyle = FlatStyle.Flat;
        DecBTN.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F);
        DecBTN.ForeColor = System.Drawing.Color.Linen;
        DecBTN.Location = new System.Drawing.Point(580, 750);
        DecBTN.Margin = new Padding(0);
        DecBTN.Name = "DecBTN";
        DecBTN.Size = new System.Drawing.Size(333, 127);
        DecBTN.TabIndex = 1;
        DecBTN.UseVisualStyleBackColor = false;
        DecBTN.Click += DecBTN_Click;
        DecBTN.MouseLeave += DecBTN_MouseLeave;
        DecBTN.MouseHover += DecBTN_MouseHover;
        DecBTN.MouseMove += DecBTN_MouseMove;
        // 
        // GallBTN
        // 
        GallBTN.BackColor = System.Drawing.Color.Transparent;
        GallBTN.BackgroundImage = Leonardo.Properties.Resources.leo_0001s_0000_gen_copy;
        GallBTN.BackgroundImageLayout = ImageLayout.None;
        GallBTN.FlatAppearance.BorderSize = 0;
        GallBTN.FlatStyle = FlatStyle.Flat;
        GallBTN.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F);
        GallBTN.ForeColor = System.Drawing.Color.Linen;
        GallBTN.Location = new System.Drawing.Point(1080, 757);
        GallBTN.Margin = new Padding(0);
        GallBTN.Name = "GallBTN";
        GallBTN.Size = new System.Drawing.Size(198, 142);
        GallBTN.TabIndex = 2;
        GallBTN.UseVisualStyleBackColor = false;
        GallBTN.Click += GallBTN_Click;
        GallBTN.MouseLeave += GallBTN_MouseLeave;
        GallBTN.MouseHover += GallBTN_MouseHover;
        // 
        // DecryptPanel
        // 
        DecryptPanel.BackColor = System.Drawing.Color.Transparent;
        DecryptPanel.BackgroundImageLayout = ImageLayout.None;
        DecryptPanel.Controls.Add(progressBar1);
        DecryptPanel.Controls.Add(CopyToClipboardButton);
        DecryptPanel.Controls.Add(DECMsgLabel);
        DecryptPanel.Controls.Add(PicBoxDEC);
        DecryptPanel.Controls.Add(DECuploadBTN);
        DecryptPanel.Controls.Add(pictureBox1);
        DecryptPanel.Location = new System.Drawing.Point(105, 46);
        DecryptPanel.Margin = new Padding(3, 4, 3, 4);
        DecryptPanel.Name = "DecryptPanel";
        DecryptPanel.Size = new System.Drawing.Size(1192, 674);
        DecryptPanel.TabIndex = 4;
        // 
        // progressBar1
        // 
        progressBar1.Location = new System.Drawing.Point(352, 613);
        progressBar1.Margin = new Padding(3, 4, 3, 4);
        progressBar1.Name = "progressBar1";
        progressBar1.Size = new System.Drawing.Size(455, 26);
        progressBar1.TabIndex = 5;
        progressBar1.Visible = false;
        // 
        // CopyToClipboardButton
        // 
        CopyToClipboardButton.BackColor = System.Drawing.Color.Transparent;
        CopyToClipboardButton.BackgroundImage = Leonardo.Properties.Resources.GREENHOVER;
        CopyToClipboardButton.BackgroundImageLayout = ImageLayout.None;
        CopyToClipboardButton.FlatAppearance.BorderSize = 0;
        CopyToClipboardButton.FlatStyle = FlatStyle.Flat;
        CopyToClipboardButton.Location = new System.Drawing.Point(19, 562);
        CopyToClipboardButton.Margin = new Padding(3, 4, 3, 4);
        CopyToClipboardButton.Name = "CopyToClipboardButton";
        CopyToClipboardButton.Size = new System.Drawing.Size(106, 88);
        CopyToClipboardButton.TabIndex = 3;
        CopyToClipboardButton.UseVisualStyleBackColor = false;
        CopyToClipboardButton.Click += CopyToClipboardButton_Click;
        CopyToClipboardButton.MouseEnter += CopyToClipboardButton_MouseEnter;
        CopyToClipboardButton.MouseLeave += CopyToClipboardButton_MouseLeave;
        CopyToClipboardButton.MouseHover += CopyToClipboardButton_MouseHover;
        // 
        // DECMsgLabel
        // 
        DECMsgLabel.AutoSize = true;
        DECMsgLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold);
        DECMsgLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
        DECMsgLabel.Location = new System.Drawing.Point(132, 600);
        DECMsgLabel.Name = "DECMsgLabel";
        DECMsgLabel.Size = new System.Drawing.Size(56, 31);
        DECMsgLabel.TabIndex = 2;
        DECMsgLabel.Text = "< >";
        // 
        // PicBoxDEC
        // 
        PicBoxDEC.BackColor = System.Drawing.Color.Transparent;
        PicBoxDEC.Location = new System.Drawing.Point(1108, 17);
        PicBoxDEC.Margin = new Padding(3, 4, 3, 4);
        PicBoxDEC.Name = "PicBoxDEC";
        PicBoxDEC.Size = new System.Drawing.Size(63, 67);
        PicBoxDEC.TabIndex = 1;
        PicBoxDEC.TabStop = false;
        PicBoxDEC.Visible = false;
        // 
        // DECuploadBTN
        // 
        DECuploadBTN.BackColor = System.Drawing.Color.Transparent;
        DECuploadBTN.BackgroundImage = Leonardo.Properties.Resources.btnleaver2;
        DECuploadBTN.BackgroundImageLayout = ImageLayout.Zoom;
        DECuploadBTN.FlatAppearance.BorderSize = 0;
        DECuploadBTN.FlatStyle = FlatStyle.Flat;
        DECuploadBTN.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
        DECuploadBTN.Location = new System.Drawing.Point(879, 501);
        DECuploadBTN.Margin = new Padding(3, 4, 3, 4);
        DECuploadBTN.Name = "DECuploadBTN";
        DECuploadBTN.Size = new System.Drawing.Size(240, 139);
        DECuploadBTN.TabIndex = 0;
        DECuploadBTN.UseVisualStyleBackColor = false;
        DECuploadBTN.Click += DECuploadBTN_Click;
        DECuploadBTN.MouseLeave += DECuploadBTN_MouseLeave;
        DECuploadBTN.MouseHover += DECuploadBTN_MouseHover;
        // 
        // pictureBox1
        // 
        pictureBox1.BackgroundImageLayout = ImageLayout.None;
        pictureBox1.Image = Leonardo.Properties.Resources.tttttt;
        pictureBox1.Location = new System.Drawing.Point(50, 23);
        pictureBox1.Margin = new Padding(3, 4, 3, 4);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new System.Drawing.Size(1069, 589);
        pictureBox1.TabIndex = 4;
        pictureBox1.TabStop = false;
        // 
        // GalleryPanel
        // 
        GalleryPanel.BackColor = System.Drawing.Color.Transparent;
        GalleryPanel.Location = new System.Drawing.Point(1336, 114);
        GalleryPanel.Margin = new Padding(3, 4, 3, 4);
        GalleryPanel.Name = "GalleryPanel";
        GalleryPanel.Size = new System.Drawing.Size(471, 164);
        GalleryPanel.TabIndex = 5;
        // 
        // MenuPanel
        // 
        MenuPanel.BackColor = System.Drawing.Color.Transparent;
        MenuPanel.Location = new System.Drawing.Point(1336, 307);
        MenuPanel.Margin = new Padding(3, 4, 3, 4);
        MenuPanel.Name = "MenuPanel";
        MenuPanel.Size = new System.Drawing.Size(412, 294);
        MenuPanel.TabIndex = 6;
        // 
        // EncryptPanel
        // 
        EncryptPanel.BackColor = System.Drawing.Color.Transparent;
        EncryptPanel.BackgroundImageLayout = ImageLayout.Stretch;
        EncryptPanel.Controls.Add(UploadBTN);
        EncryptPanel.Location = new System.Drawing.Point(1336, 14);
        EncryptPanel.Margin = new Padding(3, 4, 3, 4);
        EncryptPanel.Name = "EncryptPanel";
        EncryptPanel.Size = new System.Drawing.Size(134, 79);
        EncryptPanel.TabIndex = 3;
        EncryptPanel.Visible = false;
        // 
        // UploadBTN
        // 
        UploadBTN.BackColor = System.Drawing.Color.LawnGreen;
        UploadBTN.FlatAppearance.BorderSize = 0;
        UploadBTN.FlatStyle = FlatStyle.Flat;
        UploadBTN.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Bold);
        UploadBTN.ForeColor = System.Drawing.Color.FromArgb(0, 0, 192);
        UploadBTN.Location = new System.Drawing.Point(871, 443);
        UploadBTN.Margin = new Padding(3, 4, 3, 4);
        UploadBTN.Name = "UploadBTN";
        UploadBTN.Size = new System.Drawing.Size(135, 62);
        UploadBTN.TabIndex = 0;
        UploadBTN.Text = "UPLOAD";
        UploadBTN.UseVisualStyleBackColor = false;
        UploadBTN.Click += UploadBTN_Click;
        // 
        // timer1
        // 
        timer1.Interval = 10;
        timer1.Tick += Timer1_Tick;
        // 
        // testbtn
        // 
        testbtn.BackColor = System.Drawing.Color.Transparent;
        testbtn.Location = new System.Drawing.Point(1311, 838);
        testbtn.Margin = new Padding(3, 4, 3, 4);
        testbtn.Name = "testbtn";
        testbtn.Size = new System.Drawing.Size(87, 26);
        testbtn.TabIndex = 7;
        testbtn.UseVisualStyleBackColor = false;
        testbtn.Visible = false;
        testbtn.Click += testbtn_Click;
        // 
        // LeonardoView
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(39, 39, 39);
        BackgroundImage = Leonardo.Properties.Resources.emtbg;
        BackgroundImageLayout = ImageLayout.None;
        ClientSize = new System.Drawing.Size(1401, 915);
        Controls.Add(testbtn);
        Controls.Add(GalleryPanel);
        Controls.Add(DecryptPanel);
        Controls.Add(MenuPanel);
        Controls.Add(GallBTN);
        Controls.Add(DecBTN);
        Controls.Add(EncBTN);
        Controls.Add(EncryptPanel);
        Cursor = Cursors.PanNW;
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(3, 4, 3, 4);
        Name = "LeonardoView";
        Opacity = 0D;
        Text = "Leonardo";
        Load += Form1_Load;
        DecryptPanel.ResumeLayout(false);
        DecryptPanel.PerformLayout();
        ((ISupportInitialize)PicBoxDEC).EndInit();
        ((ISupportInitialize)pictureBox1).EndInit();
        EncryptPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    private string _dogImageUrl;
    private Timer timer;
    private double opacityIncrement = 0.05;
    private double targetOpacity = 1.0;
    private IContainer components;
    private Button EncBTN;
    private Button DecBTN;
    private Button GallBTN;
    private Panel EncryptPanel;
    private Panel DecryptPanel;
    private Panel GalleryPanel;
    private Panel MenuPanel;
    private Button UploadBTN;
    private Button DECuploadBTN;
    private Button CopyToClipboardButton;
    private Label DECMsgLabel;
    private PictureBox PicBoxDEC;
    private Timer timer1;
    private PictureBox pictureBox1;
    private ProgressBar progressBar1;
    private Button testbtn;

}