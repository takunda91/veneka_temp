namespace IndigoToolkit
{
    partial class MainForm
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

            if (currentForm != null)
            {
                currentForm.Hide();
                currentForm.Dispose();
                //panel1.Controls.RemoveAt(0);
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tbLogWindow = new System.Windows.Forms.TextBox();
            this.cntxLogWindow = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnNewDB = new System.Windows.Forms.Button();
            this.tbSourceServer = new System.Windows.Forms.TextBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.btnConnectSource = new System.Windows.Forms.Button();
            this.tbSqlInfo = new System.Windows.Forms.TextBox();
            this.lblServerInfo = new System.Windows.Forms.Label();
            this.gbSourceServer = new System.Windows.Forms.GroupBox();
            this.ckbTrustServer = new System.Windows.Forms.CheckBox();
            this.cmbSqlAuth = new System.Windows.Forms.ComboBox();
            this.tbAuth = new System.Windows.Forms.Label();
            this.ckbEncrypted = new System.Windows.Forms.CheckBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnMigrate = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.saveLogMenu = new System.Windows.Forms.SaveFileDialog();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.cntxLogWindow.SuspendLayout();
            this.gbSourceServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbLogWindow
            // 
            this.tbLogWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLogWindow.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.tbLogWindow.ContextMenuStrip = this.cntxLogWindow;
            this.tbLogWindow.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLogWindow.Location = new System.Drawing.Point(489, 152);
            this.tbLogWindow.Multiline = true;
            this.tbLogWindow.Name = "tbLogWindow";
            this.tbLogWindow.ReadOnly = true;
            this.tbLogWindow.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbLogWindow.Size = new System.Drawing.Size(680, 482);
            this.tbLogWindow.TabIndex = 6;
            this.tbLogWindow.WordWrap = false;
            // 
            // cntxLogWindow
            // 
            this.cntxLogWindow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.toolStripSeparator1,
            this.clearAllToolStripMenuItem});
            this.cntxLogWindow.Name = "cntxLogWindow";
            this.cntxLogWindow.Size = new System.Drawing.Size(123, 98);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(119, 6);
            // 
            // clearAllToolStripMenuItem
            // 
            this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
            this.clearAllToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.clearAllToolStripMenuItem.Text = "Clear All";
            this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.clearAllToolStripMenuItem_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // btnNewDB
            // 
            this.btnNewDB.Enabled = false;
            this.btnNewDB.Location = new System.Drawing.Point(22, 108);
            this.btnNewDB.Name = "btnNewDB";
            this.btnNewDB.Size = new System.Drawing.Size(75, 23);
            this.btnNewDB.TabIndex = 1;
            this.btnNewDB.Text = "Create";
            this.btnNewDB.UseVisualStyleBackColor = true;
            this.btnNewDB.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbSourceServer
            // 
            this.tbSourceServer.Location = new System.Drawing.Point(75, 13);
            this.tbSourceServer.Name = "tbSourceServer";
            this.tbSourceServer.Size = new System.Drawing.Size(206, 20);
            this.tbSourceServer.TabIndex = 1;
            this.tbSourceServer.Text = "localhost";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(7, 16);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(38, 13);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "Server";
            // 
            // btnConnectSource
            // 
            this.btnConnectSource.Location = new System.Drawing.Point(584, 62);
            this.btnConnectSource.Name = "btnConnectSource";
            this.btnConnectSource.Size = new System.Drawing.Size(75, 23);
            this.btnConnectSource.TabIndex = 10;
            this.btnConnectSource.Text = "Connect";
            this.btnConnectSource.UseVisualStyleBackColor = true;
            this.btnConnectSource.Click += new System.EventHandler(this.btnConnectSource_Click);
            // 
            // tbSqlInfo
            // 
            this.tbSqlInfo.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.tbSqlInfo.Location = new System.Drawing.Point(854, 17);
            this.tbSqlInfo.Multiline = true;
            this.tbSqlInfo.Name = "tbSqlInfo";
            this.tbSqlInfo.ReadOnly = true;
            this.tbSqlInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbSqlInfo.Size = new System.Drawing.Size(297, 64);
            this.tbSqlInfo.TabIndex = 12;
            this.tbSqlInfo.WordWrap = false;
            // 
            // lblServerInfo
            // 
            this.lblServerInfo.AutoSize = true;
            this.lblServerInfo.Location = new System.Drawing.Point(789, 20);
            this.lblServerInfo.Name = "lblServerInfo";
            this.lblServerInfo.Size = new System.Drawing.Size(59, 13);
            this.lblServerInfo.TabIndex = 11;
            this.lblServerInfo.Text = "Server Info";
            // 
            // gbSourceServer
            // 
            this.gbSourceServer.Controls.Add(this.ckbTrustServer);
            this.gbSourceServer.Controls.Add(this.cmbSqlAuth);
            this.gbSourceServer.Controls.Add(this.tbAuth);
            this.gbSourceServer.Controls.Add(this.ckbEncrypted);
            this.gbSourceServer.Controls.Add(this.tbPassword);
            this.gbSourceServer.Controls.Add(this.tbUsername);
            this.gbSourceServer.Controls.Add(this.lblPassword);
            this.gbSourceServer.Controls.Add(this.lblUsername);
            this.gbSourceServer.Controls.Add(this.lblServerInfo);
            this.gbSourceServer.Controls.Add(this.tbSqlInfo);
            this.gbSourceServer.Controls.Add(this.btnConnectSource);
            this.gbSourceServer.Controls.Add(this.lblServer);
            this.gbSourceServer.Controls.Add(this.tbSourceServer);
            this.gbSourceServer.Location = new System.Drawing.Point(12, 12);
            this.gbSourceServer.Name = "gbSourceServer";
            this.gbSourceServer.Size = new System.Drawing.Size(1157, 90);
            this.gbSourceServer.TabIndex = 0;
            this.gbSourceServer.TabStop = false;
            this.gbSourceServer.Text = "SQL Server";
            // 
            // ckbTrustServer
            // 
            this.ckbTrustServer.AutoSize = true;
            this.ckbTrustServer.Location = new System.Drawing.Point(75, 62);
            this.ckbTrustServer.Name = "ckbTrustServer";
            this.ckbTrustServer.Size = new System.Drawing.Size(134, 17);
            this.ckbTrustServer.TabIndex = 3;
            this.ckbTrustServer.Text = "Trust Server Certificate";
            this.ckbTrustServer.UseVisualStyleBackColor = true;
            // 
            // cmbSqlAuth
            // 
            this.cmbSqlAuth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSqlAuth.FormattingEnabled = true;
            this.cmbSqlAuth.Location = new System.Drawing.Point(372, 12);
            this.cmbSqlAuth.Name = "cmbSqlAuth";
            this.cmbSqlAuth.Size = new System.Drawing.Size(206, 21);
            this.cmbSqlAuth.TabIndex = 5;
            this.cmbSqlAuth.SelectedIndexChanged += new System.EventHandler(this.cmbSqlAuth_SelectedIndexChanged);
            // 
            // tbAuth
            // 
            this.tbAuth.AutoSize = true;
            this.tbAuth.Location = new System.Drawing.Point(291, 15);
            this.tbAuth.Name = "tbAuth";
            this.tbAuth.Size = new System.Drawing.Size(75, 13);
            this.tbAuth.TabIndex = 4;
            this.tbAuth.Text = "Authentication";
            // 
            // ckbEncrypted
            // 
            this.ckbEncrypted.AutoSize = true;
            this.ckbEncrypted.Location = new System.Drawing.Point(75, 41);
            this.ckbEncrypted.Name = "ckbEncrypted";
            this.ckbEncrypted.Size = new System.Drawing.Size(131, 17);
            this.ckbEncrypted.TabIndex = 2;
            this.ckbEncrypted.Text = "Encrypted Connection";
            this.ckbEncrypted.UseVisualStyleBackColor = true;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(372, 64);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(206, 20);
            this.tbPassword.TabIndex = 9;
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(372, 39);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(206, 20);
            this.tbUsername.TabIndex = 7;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(291, 67);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 8;
            this.lblPassword.Text = "Password";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(291, 42);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(55, 13);
            this.lblUsername.TabIndex = 6;
            this.lblUsername.Text = "Username";
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(2, 152);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(481, 482);
            this.panel1.TabIndex = 5;
            // 
            // btnMigrate
            // 
            this.btnMigrate.Enabled = false;
            this.btnMigrate.Location = new System.Drawing.Point(103, 108);
            this.btnMigrate.Name = "btnMigrate";
            this.btnMigrate.Size = new System.Drawing.Size(75, 23);
            this.btnMigrate.TabIndex = 2;
            this.btnMigrate.Text = "Migrate";
            this.btnMigrate.UseVisualStyleBackColor = true;
            this.btnMigrate.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(184, 108);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 3;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Visible = false;
            // 
            // saveLogMenu
            // 
            this.saveLogMenu.FileOk += new System.ComponentModel.CancelEventHandler(this.saveLogMenu_FileOk);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(609, 117);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(367, 23);
            this.progressBar.TabIndex = 7;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1181, 646);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnMigrate);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnNewDB);
            this.Controls.Add(this.tbLogWindow);
            this.Controls.Add(this.gbSourceServer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Indigo Toolkit 2.1.3.3";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IndigoToolkit_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.cntxLogWindow.ResumeLayout(false);
            this.gbSourceServer.ResumeLayout(false);
            this.gbSourceServer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tbLogWindow;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnNewDB;
        private System.Windows.Forms.TextBox tbSourceServer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Button btnConnectSource;
        private System.Windows.Forms.TextBox tbSqlInfo;
        private System.Windows.Forms.Label lblServerInfo;
        private System.Windows.Forms.GroupBox gbSourceServer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnMigrate;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.CheckBox ckbEncrypted;
        private System.Windows.Forms.ContextMenuStrip cntxLogWindow;
        private System.Windows.Forms.ToolStripMenuItem clearAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.SaveFileDialog saveLogMenu;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.Label tbAuth;
        private System.Windows.Forms.ComboBox cmbSqlAuth;
        private System.Windows.Forms.CheckBox ckbTrustServer;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}

