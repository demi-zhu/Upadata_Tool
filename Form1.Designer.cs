namespace IAP_Demo
{
    partial class IAP_Upgrade
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IAP_Upgrade));
            this.comboBoxPortName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonBeginTest = new System.Windows.Forms.Button();
            this.buttonOpenfile1 = new System.Windows.Forms.Button();
            this.textBoxFile1Path = new System.Windows.Forms.TextBox();
            this.comboBox_AppSTAddress = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_PortType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox_CRC = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button_DownloadSPIF = new System.Windows.Forms.Button();
            this.butto_OpenSPIFFile = new System.Windows.Forms.Button();
            this.textBox_SPIFFilePath = new System.Windows.Forms.TextBox();
            this.textBox_SPIFAddress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkBox_WriteSPIF = new System.Windows.Forms.CheckBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxPortName
            // 
            this.comboBoxPortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPortName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxPortName.FormattingEnabled = true;
            this.comboBoxPortName.Location = new System.Drawing.Point(293, 80);
            this.comboBoxPortName.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxPortName.Name = "comboBoxPortName";
            this.comboBoxPortName.Size = new System.Drawing.Size(192, 28);
            this.comboBoxPortName.TabIndex = 9;
            this.comboBoxPortName.SelectedIndexChanged += new System.EventHandler(this.comboBoxPortName_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(41, 80);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "RS232 Port Type";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // buttonBeginTest
            // 
            this.buttonBeginTest.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonBeginTest.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBeginTest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonBeginTest.Location = new System.Drawing.Point(501, 243);
            this.buttonBeginTest.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.buttonBeginTest.Name = "buttonBeginTest";
            this.buttonBeginTest.Size = new System.Drawing.Size(178, 53);
            this.buttonBeginTest.TabIndex = 97;
            this.buttonBeginTest.Text = "Download";
            this.buttonBeginTest.UseVisualStyleBackColor = false;
            this.buttonBeginTest.Click += new System.EventHandler(this.buttonBeginTest_Click);
            // 
            // buttonOpenfile1
            // 
            this.buttonOpenfile1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOpenfile1.Location = new System.Drawing.Point(574, 151);
            this.buttonOpenfile1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.buttonOpenfile1.Name = "buttonOpenfile1";
            this.buttonOpenfile1.Size = new System.Drawing.Size(56, 36);
            this.buttonOpenfile1.TabIndex = 96;
            this.buttonOpenfile1.Text = ".....";
            this.buttonOpenfile1.UseVisualStyleBackColor = true;
            this.buttonOpenfile1.Click += new System.EventHandler(this.buttonOpenfile1_Click);
            // 
            // textBoxFile1Path
            // 
            this.textBoxFile1Path.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.textBoxFile1Path.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxFile1Path.Location = new System.Drawing.Point(45, 159);
            this.textBoxFile1Path.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.textBoxFile1Path.Name = "textBoxFile1Path";
            this.textBoxFile1Path.ReadOnly = true;
            this.textBoxFile1Path.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxFile1Path.Size = new System.Drawing.Size(473, 28);
            this.textBoxFile1Path.TabIndex = 95;
            // 
            // comboBox_AppSTAddress
            // 
            this.comboBox_AppSTAddress.DisplayMember = "0";
            this.comboBox_AppSTAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_AppSTAddress.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_AppSTAddress.FormattingEnabled = true;
            this.comboBox_AppSTAddress.Items.AddRange(new object[] {
            "08008000"});
            this.comboBox_AppSTAddress.Location = new System.Drawing.Point(238, 92);
            this.comboBox_AppSTAddress.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox_AppSTAddress.Name = "comboBox_AppSTAddress";
            this.comboBox_AppSTAddress.Size = new System.Drawing.Size(180, 28);
            this.comboBox_AppSTAddress.TabIndex = 99;
            this.comboBox_AppSTAddress.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(18, 95);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(204, 20);
            this.label1.TabIndex = 98;
            this.label1.Text = "App Start Address(0x)";
            this.label1.Visible = false;
            // 
            // comboBox_PortType
            // 
            this.comboBox_PortType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_PortType.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_PortType.FormattingEnabled = true;
            this.comboBox_PortType.Items.AddRange(new object[] {
            "RS232",
            "USB"});
            this.comboBox_PortType.Location = new System.Drawing.Point(293, 20);
            this.comboBox_PortType.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox_PortType.Name = "comboBox_PortType";
            this.comboBox_PortType.Size = new System.Drawing.Size(192, 28);
            this.comboBox_PortType.TabIndex = 101;
            this.comboBox_PortType.SelectedIndexChanged += new System.EventHandler(this.comboBox_PortType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(71, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 20);
            this.label3.TabIndex = 100;
            this.label3.Text = "Port Type";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // checkBox_CRC
            // 
            this.checkBox_CRC.AutoSize = true;
            this.checkBox_CRC.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkBox_CRC.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_CRC.Location = new System.Drawing.Point(471, 20);
            this.checkBox_CRC.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox_CRC.Name = "checkBox_CRC";
            this.checkBox_CRC.Size = new System.Drawing.Size(260, 24);
            this.checkBox_CRC.TabIndex = 102;
            this.checkBox_CRC.Text = "CRC Verify after download";
            this.checkBox_CRC.UseVisualStyleBackColor = true;
            this.checkBox_CRC.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(39, 302);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(251, 20);
            this.label4.TabIndex = 106;
            this.label4.Text = "SPIF Download Address(0x)";
            this.label4.Visible = false;
            // 
            // button_DownloadSPIF
            // 
            this.button_DownloadSPIF.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.button_DownloadSPIF.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_DownloadSPIF.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button_DownloadSPIF.Location = new System.Drawing.Point(537, 376);
            this.button_DownloadSPIF.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.button_DownloadSPIF.Name = "button_DownloadSPIF";
            this.button_DownloadSPIF.Size = new System.Drawing.Size(168, 45);
            this.button_DownloadSPIF.TabIndex = 105;
            this.button_DownloadSPIF.Text = "Download";
            this.button_DownloadSPIF.UseVisualStyleBackColor = false;
            this.button_DownloadSPIF.Visible = false;
            this.button_DownloadSPIF.Click += new System.EventHandler(this.button_DownloadSPIF_Click);
            // 
            // butto_OpenSPIFFile
            // 
            this.butto_OpenSPIFFile.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butto_OpenSPIFFile.Location = new System.Drawing.Point(650, 334);
            this.butto_OpenSPIFFile.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.butto_OpenSPIFFile.Name = "butto_OpenSPIFFile";
            this.butto_OpenSPIFFile.Size = new System.Drawing.Size(56, 36);
            this.butto_OpenSPIFFile.TabIndex = 104;
            this.butto_OpenSPIFFile.Text = ".....";
            this.butto_OpenSPIFFile.UseVisualStyleBackColor = true;
            this.butto_OpenSPIFFile.Visible = false;
            this.butto_OpenSPIFFile.Click += new System.EventHandler(this.butto_OpenSPIFFile_Click);
            // 
            // textBox_SPIFFilePath
            // 
            this.textBox_SPIFFilePath.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.textBox_SPIFFilePath.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_SPIFFilePath.Location = new System.Drawing.Point(44, 334);
            this.textBox_SPIFFilePath.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.textBox_SPIFFilePath.Name = "textBox_SPIFFilePath";
            this.textBox_SPIFFilePath.ReadOnly = true;
            this.textBox_SPIFFilePath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_SPIFFilePath.Size = new System.Drawing.Size(595, 28);
            this.textBox_SPIFFilePath.TabIndex = 103;
            this.textBox_SPIFFilePath.Visible = false;
            // 
            // textBox_SPIFAddress
            // 
            this.textBox_SPIFAddress.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_SPIFAddress.Location = new System.Drawing.Point(291, 296);
            this.textBox_SPIFAddress.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_SPIFAddress.MaxLength = 8;
            this.textBox_SPIFAddress.Name = "textBox_SPIFAddress";
            this.textBox_SPIFAddress.Size = new System.Drawing.Size(178, 28);
            this.textBox_SPIFAddress.TabIndex = 107;
            this.textBox_SPIFAddress.Text = "00000000";
            this.textBox_SPIFAddress.Visible = false;
            this.textBox_SPIFAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_SPIFAddress_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(486, 302);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 20);
            this.label5.TabIndex = 108;
            this.label5.Text = "SPIF Size";
            this.label5.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // checkBox_WriteSPIF
            // 
            this.checkBox_WriteSPIF.AutoSize = true;
            this.checkBox_WriteSPIF.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_WriteSPIF.Location = new System.Drawing.Point(45, 261);
            this.checkBox_WriteSPIF.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox_WriteSPIF.Name = "checkBox_WriteSPIF";
            this.checkBox_WriteSPIF.Size = new System.Drawing.Size(127, 24);
            this.checkBox_WriteSPIF.TabIndex = 109;
            this.checkBox_WriteSPIF.Text = "Write SPIF";
            this.checkBox_WriteSPIF.UseVisualStyleBackColor = true;
            this.checkBox_WriteSPIF.Visible = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(43, 210);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(406, 101);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 111;
            this.pictureBox2.TabStop = false;
            // 
            // IAP_Upgrade
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(709, 345);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.checkBox_WriteSPIF);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_SPIFAddress);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button_DownloadSPIF);
            this.Controls.Add(this.butto_OpenSPIFFile);
            this.Controls.Add(this.textBox_SPIFFilePath);
            this.Controls.Add(this.comboBox_PortType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox_AppSTAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonBeginTest);
            this.Controls.Add(this.buttonOpenfile1);
            this.Controls.Add(this.textBoxFile1Path);
            this.Controls.Add(this.comboBoxPortName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBox_CRC);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "IAP_Upgrade";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IAP_Upgrade_V1.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IAPDemo_FormClosing);
            this.Load += new System.EventHandler(this.IAPDemo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxPortName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonBeginTest;
        private System.Windows.Forms.Button buttonOpenfile1;
        private System.Windows.Forms.TextBox textBoxFile1Path;
        private System.Windows.Forms.ComboBox comboBox_AppSTAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_PortType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox_CRC;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_DownloadSPIF;
        private System.Windows.Forms.Button butto_OpenSPIFFile;
        private System.Windows.Forms.TextBox textBox_SPIFFilePath;
        private System.Windows.Forms.TextBox textBox_SPIFAddress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox checkBox_WriteSPIF;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}

