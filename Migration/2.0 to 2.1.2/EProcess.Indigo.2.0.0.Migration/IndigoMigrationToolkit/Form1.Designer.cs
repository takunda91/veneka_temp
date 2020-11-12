namespace IndigoMigrationToolkit
{
    partial class IndigoToolkit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndigoToolkit));
            this.tbSourceServer = new System.Windows.Forms.TextBox();
            this.gbSourceServer = new System.Windows.Forms.GroupBox();
            this.lblServerInfo = new System.Windows.Forms.Label();
            this.tbSqlInfo = new System.Windows.Forms.TextBox();
            this.btnConnectSource = new System.Windows.Forms.Button();
            this.lblServer = new System.Windows.Forms.Label();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.cmbSourceDbs = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gbMigrationOptions = new System.Windows.Forms.GroupBox();
            this.tbPostScripts = new System.Windows.Forms.TextBox();
            this.btnSelectPost = new System.Windows.Forms.Button();
            this.lblPostScripts = new System.Windows.Forms.Label();
            this.tbSourceCollation = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSourceVersion = new System.Windows.Forms.Label();
            this.cmbSourceVersion = new System.Windows.Forms.ComboBox();
            this.btnStartMigration = new System.Windows.Forms.Button();
            this.tbExportPath = new System.Windows.Forms.TextBox();
            this.lblExportPath = new System.Windows.Forms.Label();
            this.tbKeyExportPath = new System.Windows.Forms.TextBox();
            this.tbConfirmMasterKey = new System.Windows.Forms.TextBox();
            this.lblConfirmMasterKey = new System.Windows.Forms.Label();
            this.lblMasterKey = new System.Windows.Forms.Label();
            this.tbMasterKey = new System.Windows.Forms.TextBox();
            this.ckbExportKeys = new System.Windows.Forms.CheckBox();
            this.tbLogWindow = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.gbCreateDB = new System.Windows.Forms.GroupBox();
            this.chkCreateEnterprise = new System.Windows.Forms.CheckBox();
            this.cmbTargetCollation = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblConfirmExportKey = new System.Windows.Forms.Label();
            this.lblExportKey = new System.Windows.Forms.Label();
            this.lblEncrytionExport = new System.Windows.Forms.Label();
            this.tbExportKey = new System.Windows.Forms.TextBox();
            this.tbExportKeyConfirm = new System.Windows.Forms.TextBox();
            this.btnCreateDatabase = new System.Windows.Forms.Button();
            this.lblTargetVersion = new System.Windows.Forms.Label();
            this.cmbTargetVersion = new System.Windows.Forms.ComboBox();
            this.cmbTargetDatabase = new System.Windows.Forms.ComboBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.gbSourceServer.SuspendLayout();
            this.gbMigrationOptions.SuspendLayout();
            this.gbCreateDB.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbSourceServer
            // 
            this.tbSourceServer.Location = new System.Drawing.Point(75, 17);
            this.tbSourceServer.Name = "tbSourceServer";
            this.tbSourceServer.Size = new System.Drawing.Size(206, 20);
            this.tbSourceServer.TabIndex = 1;
            this.tbSourceServer.Text = "veneka-dev";
            // 
            // gbSourceServer
            // 
            this.gbSourceServer.Controls.Add(this.lblServerInfo);
            this.gbSourceServer.Controls.Add(this.tbSqlInfo);
            this.gbSourceServer.Controls.Add(this.btnConnectSource);
            this.gbSourceServer.Controls.Add(this.lblServer);
            this.gbSourceServer.Controls.Add(this.tbSourceServer);
            this.gbSourceServer.Location = new System.Drawing.Point(12, 12);
            this.gbSourceServer.Name = "gbSourceServer";
            this.gbSourceServer.Size = new System.Drawing.Size(390, 90);
            this.gbSourceServer.TabIndex = 0;
            this.gbSourceServer.TabStop = false;
            this.gbSourceServer.Text = "SQL Server";
            // 
            // lblServerInfo
            // 
            this.lblServerInfo.AutoSize = true;
            this.lblServerInfo.Location = new System.Drawing.Point(7, 43);
            this.lblServerInfo.Name = "lblServerInfo";
            this.lblServerInfo.Size = new System.Drawing.Size(59, 13);
            this.lblServerInfo.TabIndex = 4;
            this.lblServerInfo.Text = "Server Info";
            // 
            // tbSqlInfo
            // 
            this.tbSqlInfo.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.tbSqlInfo.Location = new System.Drawing.Point(75, 43);
            this.tbSqlInfo.Multiline = true;
            this.tbSqlInfo.Name = "tbSqlInfo";
            this.tbSqlInfo.ReadOnly = true;
            this.tbSqlInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbSqlInfo.Size = new System.Drawing.Size(206, 36);
            this.tbSqlInfo.TabIndex = 5;
            this.tbSqlInfo.WordWrap = false;
            // 
            // btnConnectSource
            // 
            this.btnConnectSource.Location = new System.Drawing.Point(303, 15);
            this.btnConnectSource.Name = "btnConnectSource";
            this.btnConnectSource.Size = new System.Drawing.Size(75, 23);
            this.btnConnectSource.TabIndex = 3;
            this.btnConnectSource.Text = "Connect";
            this.btnConnectSource.UseVisualStyleBackColor = true;
            this.btnConnectSource.Click += new System.EventHandler(this.btnConnectSource_Click);
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(7, 20);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(38, 13);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "Server";
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(7, 25);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(90, 13);
            this.lblDatabase.TabIndex = 0;
            this.lblDatabase.Text = "Source Database";
            // 
            // cmbSourceDbs
            // 
            this.cmbSourceDbs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSourceDbs.FormattingEnabled = true;
            this.cmbSourceDbs.Location = new System.Drawing.Point(103, 22);
            this.cmbSourceDbs.Name = "cmbSourceDbs";
            this.cmbSourceDbs.Size = new System.Drawing.Size(260, 21);
            this.cmbSourceDbs.TabIndex = 1;
            this.cmbSourceDbs.SelectedIndexChanged += new System.EventHandler(this.cmbSourceDbs_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Target Database";
            // 
            // gbMigrationOptions
            // 
            this.gbMigrationOptions.Controls.Add(this.tbPostScripts);
            this.gbMigrationOptions.Controls.Add(this.btnSelectPost);
            this.gbMigrationOptions.Controls.Add(this.lblPostScripts);
            this.gbMigrationOptions.Controls.Add(this.tbSourceCollation);
            this.gbMigrationOptions.Controls.Add(this.label3);
            this.gbMigrationOptions.Controls.Add(this.lblSourceVersion);
            this.gbMigrationOptions.Controls.Add(this.cmbSourceVersion);
            this.gbMigrationOptions.Controls.Add(this.lblDatabase);
            this.gbMigrationOptions.Controls.Add(this.cmbSourceDbs);
            this.gbMigrationOptions.Controls.Add(this.btnStartMigration);
            this.gbMigrationOptions.Enabled = false;
            this.gbMigrationOptions.Location = new System.Drawing.Point(13, 466);
            this.gbMigrationOptions.Name = "gbMigrationOptions";
            this.gbMigrationOptions.Size = new System.Drawing.Size(389, 169);
            this.gbMigrationOptions.TabIndex = 2;
            this.gbMigrationOptions.TabStop = false;
            this.gbMigrationOptions.Text = "Migration Options";
            // 
            // tbPostScripts
            // 
            this.tbPostScripts.Location = new System.Drawing.Point(103, 102);
            this.tbPostScripts.Name = "tbPostScripts";
            this.tbPostScripts.ReadOnly = true;
            this.tbPostScripts.Size = new System.Drawing.Size(204, 20);
            this.tbPostScripts.TabIndex = 7;
            this.tbPostScripts.Text = "C:\\veneka\\Migration\\PostScripts";
            // 
            // btnSelectPost
            // 
            this.btnSelectPost.Location = new System.Drawing.Point(314, 101);
            this.btnSelectPost.Name = "btnSelectPost";
            this.btnSelectPost.Size = new System.Drawing.Size(49, 22);
            this.btnSelectPost.TabIndex = 8;
            this.btnSelectPost.Text = "Select";
            this.btnSelectPost.UseVisualStyleBackColor = true;
            this.btnSelectPost.Click += new System.EventHandler(this.btnSelectPost_Click);
            // 
            // lblPostScripts
            // 
            this.lblPostScripts.AutoSize = true;
            this.lblPostScripts.Location = new System.Drawing.Point(6, 105);
            this.lblPostScripts.Name = "lblPostScripts";
            this.lblPostScripts.Size = new System.Drawing.Size(63, 13);
            this.lblPostScripts.TabIndex = 6;
            this.lblPostScripts.Text = "Post Scripts";
            // 
            // tbSourceCollation
            // 
            this.tbSourceCollation.Location = new System.Drawing.Point(103, 76);
            this.tbSourceCollation.Name = "tbSourceCollation";
            this.tbSourceCollation.ReadOnly = true;
            this.tbSourceCollation.Size = new System.Drawing.Size(260, 20);
            this.tbSourceCollation.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Database Collation";
            // 
            // lblSourceVersion
            // 
            this.lblSourceVersion.AutoSize = true;
            this.lblSourceVersion.Location = new System.Drawing.Point(6, 52);
            this.lblSourceVersion.Name = "lblSourceVersion";
            this.lblSourceVersion.Size = new System.Drawing.Size(79, 13);
            this.lblSourceVersion.TabIndex = 2;
            this.lblSourceVersion.Text = "Source Version";
            // 
            // cmbSourceVersion
            // 
            this.cmbSourceVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSourceVersion.FormattingEnabled = true;
            this.cmbSourceVersion.Location = new System.Drawing.Point(103, 49);
            this.cmbSourceVersion.Name = "cmbSourceVersion";
            this.cmbSourceVersion.Size = new System.Drawing.Size(260, 21);
            this.cmbSourceVersion.TabIndex = 3;
            // 
            // btnStartMigration
            // 
            this.btnStartMigration.Location = new System.Drawing.Point(140, 136);
            this.btnStartMigration.Name = "btnStartMigration";
            this.btnStartMigration.Size = new System.Drawing.Size(75, 23);
            this.btnStartMigration.TabIndex = 9;
            this.btnStartMigration.Text = "Do Migration";
            this.btnStartMigration.UseVisualStyleBackColor = true;
            this.btnStartMigration.Click += new System.EventHandler(this.btnStartMigration_Click);
            // 
            // tbExportPath
            // 
            this.tbExportPath.Location = new System.Drawing.Point(109, 155);
            this.tbExportPath.Name = "tbExportPath";
            this.tbExportPath.Size = new System.Drawing.Size(260, 20);
            this.tbExportPath.TabIndex = 11;
            this.tbExportPath.Text = "C:\\veneka\\Migration";
            // 
            // lblExportPath
            // 
            this.lblExportPath.AutoSize = true;
            this.lblExportPath.Location = new System.Drawing.Point(14, 158);
            this.lblExportPath.Name = "lblExportPath";
            this.lblExportPath.Size = new System.Drawing.Size(62, 13);
            this.lblExportPath.TabIndex = 10;
            this.lblExportPath.Text = "Export Path";
            // 
            // tbKeyExportPath
            // 
            this.tbKeyExportPath.Location = new System.Drawing.Point(102, 33);
            this.tbKeyExportPath.Name = "tbKeyExportPath";
            this.tbKeyExportPath.Size = new System.Drawing.Size(260, 20);
            this.tbKeyExportPath.TabIndex = 2;
            this.tbKeyExportPath.Text = "C:\\veneka\\Migration";
            // 
            // tbConfirmMasterKey
            // 
            this.tbConfirmMasterKey.Location = new System.Drawing.Point(109, 129);
            this.tbConfirmMasterKey.Name = "tbConfirmMasterKey";
            this.tbConfirmMasterKey.Size = new System.Drawing.Size(260, 20);
            this.tbConfirmMasterKey.TabIndex = 9;
            // 
            // lblConfirmMasterKey
            // 
            this.lblConfirmMasterKey.AutoSize = true;
            this.lblConfirmMasterKey.Location = new System.Drawing.Point(14, 132);
            this.lblConfirmMasterKey.Name = "lblConfirmMasterKey";
            this.lblConfirmMasterKey.Size = new System.Drawing.Size(63, 13);
            this.lblConfirmMasterKey.TabIndex = 8;
            this.lblConfirmMasterKey.Text = "Confirm Key";
            // 
            // lblMasterKey
            // 
            this.lblMasterKey.AutoSize = true;
            this.lblMasterKey.Location = new System.Drawing.Point(14, 106);
            this.lblMasterKey.Name = "lblMasterKey";
            this.lblMasterKey.Size = new System.Drawing.Size(60, 13);
            this.lblMasterKey.TabIndex = 6;
            this.lblMasterKey.Text = "Master Key";
            // 
            // tbMasterKey
            // 
            this.tbMasterKey.Location = new System.Drawing.Point(109, 103);
            this.tbMasterKey.Name = "tbMasterKey";
            this.tbMasterKey.Size = new System.Drawing.Size(260, 20);
            this.tbMasterKey.TabIndex = 7;
            // 
            // ckbExportKeys
            // 
            this.ckbExportKeys.AutoSize = true;
            this.ckbExportKeys.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckbExportKeys.Checked = true;
            this.ckbExportKeys.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbExportKeys.Location = new System.Drawing.Point(7, 10);
            this.ckbExportKeys.Name = "ckbExportKeys";
            this.ckbExportKeys.Size = new System.Drawing.Size(109, 17);
            this.ckbExportKeys.TabIndex = 0;
            this.ckbExportKeys.Text = "Export Encryption";
            this.ckbExportKeys.UseVisualStyleBackColor = true;
            this.ckbExportKeys.CheckedChanged += new System.EventHandler(this.ckbExportKeys_CheckedChanged);
            // 
            // tbLogWindow
            // 
            this.tbLogWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLogWindow.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.tbLogWindow.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLogWindow.Location = new System.Drawing.Point(419, 60);
            this.tbLogWindow.Multiline = true;
            this.tbLogWindow.Name = "tbLogWindow";
            this.tbLogWindow.ReadOnly = true;
            this.tbLogWindow.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbLogWindow.Size = new System.Drawing.Size(472, 574);
            this.tbLogWindow.TabIndex = 4;
            this.tbLogWindow.WordWrap = false;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(419, 27);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(472, 23);
            this.progressBar.TabIndex = 3;
            // 
            // gbCreateDB
            // 
            this.gbCreateDB.Controls.Add(this.chkCreateEnterprise);
            this.gbCreateDB.Controls.Add(this.cmbTargetCollation);
            this.gbCreateDB.Controls.Add(this.label2);
            this.gbCreateDB.Controls.Add(this.groupBox1);
            this.gbCreateDB.Controls.Add(this.btnCreateDatabase);
            this.gbCreateDB.Controls.Add(this.lblTargetVersion);
            this.gbCreateDB.Controls.Add(this.cmbTargetVersion);
            this.gbCreateDB.Controls.Add(this.cmbTargetDatabase);
            this.gbCreateDB.Controls.Add(this.label1);
            this.gbCreateDB.Controls.Add(this.tbMasterKey);
            this.gbCreateDB.Controls.Add(this.tbExportPath);
            this.gbCreateDB.Controls.Add(this.lblMasterKey);
            this.gbCreateDB.Controls.Add(this.lblExportPath);
            this.gbCreateDB.Controls.Add(this.lblConfirmMasterKey);
            this.gbCreateDB.Controls.Add(this.tbConfirmMasterKey);
            this.gbCreateDB.Enabled = false;
            this.gbCreateDB.Location = new System.Drawing.Point(12, 108);
            this.gbCreateDB.Name = "gbCreateDB";
            this.gbCreateDB.Size = new System.Drawing.Size(390, 352);
            this.gbCreateDB.TabIndex = 1;
            this.gbCreateDB.TabStop = false;
            this.gbCreateDB.Text = "Create Database";
            // 
            // chkCreateEnterprise
            // 
            this.chkCreateEnterprise.AutoSize = true;
            this.chkCreateEnterprise.Location = new System.Drawing.Point(16, 181);
            this.chkCreateEnterprise.Name = "chkCreateEnterprise";
            this.chkCreateEnterprise.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkCreateEnterprise.Size = new System.Drawing.Size(107, 17);
            this.chkCreateEnterprise.TabIndex = 12;
            this.chkCreateEnterprise.Text = "Create Enterprise";
            this.chkCreateEnterprise.UseVisualStyleBackColor = true;
            // 
            // cmbTargetCollation
            // 
            this.cmbTargetCollation.AllowDrop = true;
            this.cmbTargetCollation.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbTargetCollation.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbTargetCollation.FormattingEnabled = true;
            this.cmbTargetCollation.Location = new System.Drawing.Point(109, 76);
            this.cmbTargetCollation.Name = "cmbTargetCollation";
            this.cmbTargetCollation.Size = new System.Drawing.Size(260, 21);
            this.cmbTargetCollation.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Database Collation";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblConfirmExportKey);
            this.groupBox1.Controls.Add(this.ckbExportKeys);
            this.groupBox1.Controls.Add(this.lblExportKey);
            this.groupBox1.Controls.Add(this.tbKeyExportPath);
            this.groupBox1.Controls.Add(this.lblEncrytionExport);
            this.groupBox1.Controls.Add(this.tbExportKey);
            this.groupBox1.Controls.Add(this.tbExportKeyConfirm);
            this.groupBox1.Location = new System.Drawing.Point(6, 200);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(378, 114);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // lblConfirmExportKey
            // 
            this.lblConfirmExportKey.AutoSize = true;
            this.lblConfirmExportKey.Location = new System.Drawing.Point(7, 88);
            this.lblConfirmExportKey.Name = "lblConfirmExportKey";
            this.lblConfirmExportKey.Size = new System.Drawing.Size(63, 13);
            this.lblConfirmExportKey.TabIndex = 5;
            this.lblConfirmExportKey.Text = "Confirm Key";
            // 
            // lblExportKey
            // 
            this.lblExportKey.AutoSize = true;
            this.lblExportKey.Location = new System.Drawing.Point(7, 62);
            this.lblExportKey.Name = "lblExportKey";
            this.lblExportKey.Size = new System.Drawing.Size(58, 13);
            this.lblExportKey.TabIndex = 3;
            this.lblExportKey.Text = "Export Key";
            // 
            // lblEncrytionExport
            // 
            this.lblEncrytionExport.AutoSize = true;
            this.lblEncrytionExport.Location = new System.Drawing.Point(7, 36);
            this.lblEncrytionExport.Name = "lblEncrytionExport";
            this.lblEncrytionExport.Size = new System.Drawing.Size(80, 13);
            this.lblEncrytionExport.TabIndex = 1;
            this.lblEncrytionExport.Text = "Path On Server";
            // 
            // tbExportKey
            // 
            this.tbExportKey.Location = new System.Drawing.Point(102, 59);
            this.tbExportKey.Name = "tbExportKey";
            this.tbExportKey.Size = new System.Drawing.Size(260, 20);
            this.tbExportKey.TabIndex = 4;
            // 
            // tbExportKeyConfirm
            // 
            this.tbExportKeyConfirm.Location = new System.Drawing.Point(102, 85);
            this.tbExportKeyConfirm.Name = "tbExportKeyConfirm";
            this.tbExportKeyConfirm.Size = new System.Drawing.Size(260, 20);
            this.tbExportKeyConfirm.TabIndex = 6;
            // 
            // btnCreateDatabase
            // 
            this.btnCreateDatabase.Location = new System.Drawing.Point(141, 320);
            this.btnCreateDatabase.Name = "btnCreateDatabase";
            this.btnCreateDatabase.Size = new System.Drawing.Size(75, 23);
            this.btnCreateDatabase.TabIndex = 11;
            this.btnCreateDatabase.Text = "Create";
            this.btnCreateDatabase.UseVisualStyleBackColor = true;
            this.btnCreateDatabase.Click += new System.EventHandler(this.btnCreateDatabase_Click);
            // 
            // lblTargetVersion
            // 
            this.lblTargetVersion.AutoSize = true;
            this.lblTargetVersion.Location = new System.Drawing.Point(14, 50);
            this.lblTargetVersion.Name = "lblTargetVersion";
            this.lblTargetVersion.Size = new System.Drawing.Size(76, 13);
            this.lblTargetVersion.TabIndex = 2;
            this.lblTargetVersion.Text = "Target Version";
            // 
            // cmbTargetVersion
            // 
            this.cmbTargetVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetVersion.FormattingEnabled = true;
            this.cmbTargetVersion.Location = new System.Drawing.Point(109, 47);
            this.cmbTargetVersion.Name = "cmbTargetVersion";
            this.cmbTargetVersion.Size = new System.Drawing.Size(260, 21);
            this.cmbTargetVersion.TabIndex = 3;
            // 
            // cmbTargetDatabase
            // 
            this.cmbTargetDatabase.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbTargetDatabase.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbTargetDatabase.FormattingEnabled = true;
            this.cmbTargetDatabase.Location = new System.Drawing.Point(109, 19);
            this.cmbTargetDatabase.Name = "cmbTargetDatabase";
            this.cmbTargetDatabase.Size = new System.Drawing.Size(260, 21);
            this.cmbTargetDatabase.TabIndex = 1;
            this.cmbTargetDatabase.SelectedIndexChanged += new System.EventHandler(this.cmbTargetDatabase_SelectedIndexChanged);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // IndigoToolkit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(903, 646);
            this.Controls.Add(this.gbCreateDB);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.tbLogWindow);
            this.Controls.Add(this.gbMigrationOptions);
            this.Controls.Add(this.gbSourceServer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "IndigoToolkit";
            this.Text = "Indigo Setup Toolkit";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IndigoToolkit_FormClosing);
            this.gbSourceServer.ResumeLayout(false);
            this.gbSourceServer.PerformLayout();
            this.gbMigrationOptions.ResumeLayout(false);
            this.gbMigrationOptions.PerformLayout();
            this.gbCreateDB.ResumeLayout(false);
            this.gbCreateDB.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSourceServer;
        private System.Windows.Forms.GroupBox gbSourceServer;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.ComboBox cmbSourceDbs;
        private System.Windows.Forms.Button btnConnectSource;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbMigrationOptions;
        private System.Windows.Forms.Button btnStartMigration;
        private System.Windows.Forms.TextBox tbExportPath;
        private System.Windows.Forms.Label lblExportPath;
        private System.Windows.Forms.TextBox tbKeyExportPath;
        private System.Windows.Forms.TextBox tbConfirmMasterKey;
        private System.Windows.Forms.Label lblConfirmMasterKey;
        private System.Windows.Forms.Label lblMasterKey;
        private System.Windows.Forms.TextBox tbMasterKey;
        private System.Windows.Forms.CheckBox ckbExportKeys;
        private System.Windows.Forms.TextBox tbLogWindow;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox tbSqlInfo;
        private System.Windows.Forms.Label lblSourceVersion;
        private System.Windows.Forms.ComboBox cmbSourceVersion;
        private System.Windows.Forms.GroupBox gbCreateDB;
        private System.Windows.Forms.TextBox tbExportKey;
        private System.Windows.Forms.Label lblTargetVersion;
        private System.Windows.Forms.ComboBox cmbTargetVersion;
        private System.Windows.Forms.ComboBox cmbTargetDatabase;
        private System.Windows.Forms.TextBox tbExportKeyConfirm;
        private System.Windows.Forms.Label lblConfirmExportKey;
        private System.Windows.Forms.Label lblExportKey;
        private System.Windows.Forms.Label lblEncrytionExport;
        private System.Windows.Forms.Button btnCreateDatabase;
        private System.Windows.Forms.Label lblServerInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbTargetCollation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbSourceCollation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbPostScripts;
        private System.Windows.Forms.Button btnSelectPost;
        private System.Windows.Forms.Label lblPostScripts;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox chkCreateEnterprise;
    }
}

