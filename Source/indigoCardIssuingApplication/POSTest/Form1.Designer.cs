namespace POSTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lblDeviceId = new System.Windows.Forms.Label();
            this.tbDeviceID = new System.Windows.Forms.TextBox();
            this.lblOperatorCode = new System.Windows.Forms.Label();
            this.tbOperatorCode = new System.Windows.Forms.TextBox();
            this.lblPIN = new System.Windows.Forms.Label();
            this.tbPIN = new System.Windows.Forms.TextBox();
            this.btnHandShake = new System.Windows.Forms.Button();
            this.gbPIN = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblTMK = new System.Windows.Forms.Label();
            this.tbClearTPK = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbTMK = new System.Windows.Forms.TextBox();
            this.tbPAN = new System.Windows.Forms.TextBox();
            this.btnPIN = new System.Windows.Forms.Button();
            this.lblPAN = new System.Windows.Forms.Label();
            this.tbPinOutput = new System.Windows.Forms.TextBox();
            this.gpHandshake = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tbTPK = new System.Windows.Forms.TextBox();
            this.lblTPK = new System.Windows.Forms.Label();
            this.tbHandshakeOut = new System.Windows.Forms.TextBox();
            this.cbChannel = new System.Windows.Forms.ComboBox();
            this.tbChannel = new System.Windows.Forms.Label();
            this.tbServerAddress = new System.Windows.Forms.TextBox();
            this.lblServerAddress = new System.Windows.Forms.Label();
            this.btnCrypto = new System.Windows.Forms.Button();
            this.gbPIN.SuspendLayout();
            this.panel2.SuspendLayout();
            this.gpHandshake.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDeviceId
            // 
            this.lblDeviceId.AutoSize = true;
            this.lblDeviceId.Location = new System.Drawing.Point(20, 43);
            this.lblDeviceId.Name = "lblDeviceId";
            this.lblDeviceId.Size = new System.Drawing.Size(55, 13);
            this.lblDeviceId.TabIndex = 4;
            this.lblDeviceId.Text = "Device ID";
            // 
            // tbDeviceID
            // 
            this.tbDeviceID.Location = new System.Drawing.Point(102, 40);
            this.tbDeviceID.MaxLength = 8;
            this.tbDeviceID.Name = "tbDeviceID";
            this.tbDeviceID.Size = new System.Drawing.Size(141, 20);
            this.tbDeviceID.TabIndex = 5;
            // 
            // lblOperatorCode
            // 
            this.lblOperatorCode.AutoSize = true;
            this.lblOperatorCode.Location = new System.Drawing.Point(3, 31);
            this.lblOperatorCode.Name = "lblOperatorCode";
            this.lblOperatorCode.Size = new System.Drawing.Size(76, 13);
            this.lblOperatorCode.TabIndex = 2;
            this.lblOperatorCode.Text = "Operator Code";
            // 
            // tbOperatorCode
            // 
            this.tbOperatorCode.Location = new System.Drawing.Point(85, 28);
            this.tbOperatorCode.MaxLength = 4;
            this.tbOperatorCode.Name = "tbOperatorCode";
            this.tbOperatorCode.Size = new System.Drawing.Size(141, 20);
            this.tbOperatorCode.TabIndex = 3;
            // 
            // lblPIN
            // 
            this.lblPIN.AutoSize = true;
            this.lblPIN.Location = new System.Drawing.Point(3, 57);
            this.lblPIN.Name = "lblPIN";
            this.lblPIN.Size = new System.Drawing.Size(25, 13);
            this.lblPIN.TabIndex = 4;
            this.lblPIN.Text = "PIN";
            // 
            // tbPIN
            // 
            this.tbPIN.Location = new System.Drawing.Point(85, 54);
            this.tbPIN.MaxLength = 4;
            this.tbPIN.Name = "tbPIN";
            this.tbPIN.Size = new System.Drawing.Size(141, 20);
            this.tbPIN.TabIndex = 5;
            // 
            // btnHandShake
            // 
            this.btnHandShake.Location = new System.Drawing.Point(110, 5);
            this.btnHandShake.Name = "btnHandShake";
            this.btnHandShake.Size = new System.Drawing.Size(75, 23);
            this.btnHandShake.TabIndex = 1;
            this.btnHandShake.Text = "Handshake";
            this.btnHandShake.UseVisualStyleBackColor = true;
            this.btnHandShake.Click += new System.EventHandler(this.btnHandShake_Click);
            // 
            // gbPIN
            // 
            this.gbPIN.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPIN.Controls.Add(this.panel2);
            this.gbPIN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPIN.Location = new System.Drawing.Point(12, 315);
            this.gbPIN.Name = "gbPIN";
            this.gbPIN.Size = new System.Drawing.Size(914, 238);
            this.gbPIN.TabIndex = 7;
            this.gbPIN.TabStop = false;
            this.gbPIN.Text = "PIN";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.lblTMK);
            this.panel2.Controls.Add(this.tbClearTPK);
            this.panel2.Controls.Add(this.tbOperatorCode);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.lblPIN);
            this.panel2.Controls.Add(this.tbTMK);
            this.panel2.Controls.Add(this.lblOperatorCode);
            this.panel2.Controls.Add(this.tbPIN);
            this.panel2.Controls.Add(this.tbPAN);
            this.panel2.Controls.Add(this.btnPIN);
            this.panel2.Controls.Add(this.lblPAN);
            this.panel2.Controls.Add(this.tbPinOutput);
            this.panel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel2.Location = new System.Drawing.Point(6, 15);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(902, 216);
            this.panel2.TabIndex = 0;
            // 
            // lblTMK
            // 
            this.lblTMK.AutoSize = true;
            this.lblTMK.Location = new System.Drawing.Point(3, 5);
            this.lblTMK.Name = "lblTMK";
            this.lblTMK.Size = new System.Drawing.Size(57, 13);
            this.lblTMK.TabIndex = 0;
            this.lblTMK.Text = "Clear TMK";
            // 
            // tbClearTPK
            // 
            this.tbClearTPK.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.tbClearTPK.Location = new System.Drawing.Point(85, 154);
            this.tbClearTPK.Name = "tbClearTPK";
            this.tbClearTPK.ReadOnly = true;
            this.tbClearTPK.Size = new System.Drawing.Size(239, 20);
            this.tbClearTPK.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 157);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Clear TPK";
            // 
            // tbTMK
            // 
            this.tbTMK.Location = new System.Drawing.Point(85, 2);
            this.tbTMK.Name = "tbTMK";
            this.tbTMK.Size = new System.Drawing.Size(240, 20);
            this.tbTMK.TabIndex = 1;
            // 
            // tbPAN
            // 
            this.tbPAN.Location = new System.Drawing.Point(85, 81);
            this.tbPAN.MaxLength = 19;
            this.tbPAN.Name = "tbPAN";
            this.tbPAN.Size = new System.Drawing.Size(141, 20);
            this.tbPAN.TabIndex = 7;
            // 
            // btnPIN
            // 
            this.btnPIN.Location = new System.Drawing.Point(151, 107);
            this.btnPIN.Name = "btnPIN";
            this.btnPIN.Size = new System.Drawing.Size(75, 23);
            this.btnPIN.TabIndex = 8;
            this.btnPIN.Text = "PIN";
            this.btnPIN.UseVisualStyleBackColor = true;
            this.btnPIN.Click += new System.EventHandler(this.btnPIN_Click);
            // 
            // lblPAN
            // 
            this.lblPAN.AutoSize = true;
            this.lblPAN.Location = new System.Drawing.Point(3, 84);
            this.lblPAN.Name = "lblPAN";
            this.lblPAN.Size = new System.Drawing.Size(29, 13);
            this.lblPAN.TabIndex = 6;
            this.lblPAN.Text = "PAN";
            // 
            // tbPinOutput
            // 
            this.tbPinOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPinOutput.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.tbPinOutput.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPinOutput.Location = new System.Drawing.Point(343, 2);
            this.tbPinOutput.Multiline = true;
            this.tbPinOutput.Name = "tbPinOutput";
            this.tbPinOutput.ReadOnly = true;
            this.tbPinOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbPinOutput.Size = new System.Drawing.Size(553, 205);
            this.tbPinOutput.TabIndex = 9;
            this.tbPinOutput.WordWrap = false;
            // 
            // gpHandshake
            // 
            this.gpHandshake.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpHandshake.Controls.Add(this.panel1);
            this.gpHandshake.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpHandshake.Location = new System.Drawing.Point(12, 64);
            this.gpHandshake.Name = "gpHandshake";
            this.gpHandshake.Size = new System.Drawing.Size(914, 245);
            this.gpHandshake.TabIndex = 6;
            this.gpHandshake.TabStop = false;
            this.gpHandshake.Text = "Handshake";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tbTPK);
            this.panel1.Controls.Add(this.btnHandShake);
            this.panel1.Controls.Add(this.lblTPK);
            this.panel1.Controls.Add(this.tbHandshakeOut);
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(6, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(902, 220);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "First do handshake:";
            // 
            // tbTPK
            // 
            this.tbTPK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbTPK.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.tbTPK.Location = new System.Drawing.Point(85, 192);
            this.tbTPK.Name = "tbTPK";
            this.tbTPK.ReadOnly = true;
            this.tbTPK.Size = new System.Drawing.Size(240, 20);
            this.tbTPK.TabIndex = 4;
            // 
            // lblTPK
            // 
            this.lblTPK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTPK.AutoSize = true;
            this.lblTPK.Location = new System.Drawing.Point(3, 195);
            this.lblTPK.Name = "lblTPK";
            this.lblTPK.Size = new System.Drawing.Size(28, 13);
            this.lblTPK.TabIndex = 3;
            this.lblTPK.Text = "TPK";
            // 
            // tbHandshakeOut
            // 
            this.tbHandshakeOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbHandshakeOut.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.tbHandshakeOut.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbHandshakeOut.Location = new System.Drawing.Point(343, 5);
            this.tbHandshakeOut.Multiline = true;
            this.tbHandshakeOut.Name = "tbHandshakeOut";
            this.tbHandshakeOut.ReadOnly = true;
            this.tbHandshakeOut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbHandshakeOut.Size = new System.Drawing.Size(553, 207);
            this.tbHandshakeOut.TabIndex = 2;
            this.tbHandshakeOut.WordWrap = false;
            // 
            // cbChannel
            // 
            this.cbChannel.FormattingEnabled = true;
            this.cbChannel.Items.AddRange(new object[] {
            "Indigo SOAP",
            "Luminus ISO"});
            this.cbChannel.Location = new System.Drawing.Point(102, 13);
            this.cbChannel.Name = "cbChannel";
            this.cbChannel.Size = new System.Drawing.Size(141, 21);
            this.cbChannel.TabIndex = 1;
            this.cbChannel.SelectedIndexChanged += new System.EventHandler(this.cbChannel_SelectedIndexChanged);
            // 
            // tbChannel
            // 
            this.tbChannel.AutoSize = true;
            this.tbChannel.Location = new System.Drawing.Point(20, 16);
            this.tbChannel.Name = "tbChannel";
            this.tbChannel.Size = new System.Drawing.Size(46, 13);
            this.tbChannel.TabIndex = 0;
            this.tbChannel.Text = "Channel";
            // 
            // tbServerAddress
            // 
            this.tbServerAddress.Location = new System.Drawing.Point(360, 14);
            this.tbServerAddress.Name = "tbServerAddress";
            this.tbServerAddress.Size = new System.Drawing.Size(184, 20);
            this.tbServerAddress.TabIndex = 3;
            this.tbServerAddress.Visible = false;
            // 
            // lblServerAddress
            // 
            this.lblServerAddress.AutoSize = true;
            this.lblServerAddress.Location = new System.Drawing.Point(303, 17);
            this.lblServerAddress.Name = "lblServerAddress";
            this.lblServerAddress.Size = new System.Drawing.Size(38, 13);
            this.lblServerAddress.TabIndex = 2;
            this.lblServerAddress.Text = "Server";
            this.lblServerAddress.Visible = false;
            // 
            // btnCrypto
            // 
            this.btnCrypto.Location = new System.Drawing.Point(800, 17);
            this.btnCrypto.Name = "btnCrypto";
            this.btnCrypto.Size = new System.Drawing.Size(75, 23);
            this.btnCrypto.TabIndex = 8;
            this.btnCrypto.Text = "Crypto";
            this.btnCrypto.UseVisualStyleBackColor = true;
            this.btnCrypto.Click += new System.EventHandler(this.btnCrypto_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(931, 558);
            this.Controls.Add(this.btnCrypto);
            this.Controls.Add(this.lblServerAddress);
            this.Controls.Add(this.tbServerAddress);
            this.Controls.Add(this.tbChannel);
            this.Controls.Add(this.cbChannel);
            this.Controls.Add(this.tbDeviceID);
            this.Controls.Add(this.lblDeviceId);
            this.Controls.Add(this.gpHandshake);
            this.Controls.Add(this.gbPIN);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(947, 597);
            this.Name = "Form1";
            this.Text = "POS-Test";
            this.gbPIN.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.gpHandshake.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDeviceId;
        private System.Windows.Forms.TextBox tbDeviceID;
        private System.Windows.Forms.Label lblOperatorCode;
        private System.Windows.Forms.TextBox tbOperatorCode;
        private System.Windows.Forms.Label lblPIN;
        private System.Windows.Forms.TextBox tbPIN;
        private System.Windows.Forms.Button btnHandShake;
        private System.Windows.Forms.GroupBox gbPIN;
        private System.Windows.Forms.TextBox tbPinOutput;
        private System.Windows.Forms.Button btnPIN;
        private System.Windows.Forms.GroupBox gpHandshake;
        private System.Windows.Forms.TextBox tbHandshakeOut;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbChannel;
        private System.Windows.Forms.Label tbChannel;
        private System.Windows.Forms.TextBox tbPAN;
        private System.Windows.Forms.Label lblPAN;
        private System.Windows.Forms.TextBox tbTPK;
        private System.Windows.Forms.Label lblTPK;
        private System.Windows.Forms.TextBox tbTMK;
        private System.Windows.Forms.Label lblTMK;
        private System.Windows.Forms.TextBox tbServerAddress;
        private System.Windows.Forms.Label lblServerAddress;
        private System.Windows.Forms.TextBox tbClearTPK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCrypto;
    }
}

