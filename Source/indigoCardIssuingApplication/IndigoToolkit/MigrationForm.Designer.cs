namespace IndigoToolkit
{
    partial class MigrationForm
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
            this.gbMigrationOptions = new System.Windows.Forms.GroupBox();
            this.btnValidation = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.tbExportPath = new System.Windows.Forms.TextBox();
            this.lblExportPath = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblConfirmExportKey = new System.Windows.Forms.Label();
            this.ckbExportKeys = new System.Windows.Forms.CheckBox();
            this.lblExportKey = new System.Windows.Forms.Label();
            this.tbKeyExportPath = new System.Windows.Forms.TextBox();
            this.lblEncrytionExport = new System.Windows.Forms.Label();
            this.tbExportKey = new System.Windows.Forms.TextBox();
            this.tbExportKeyConfirm = new System.Windows.Forms.TextBox();
            this.tbMasterKey = new System.Windows.Forms.TextBox();
            this.lblMasterKey = new System.Windows.Forms.Label();
            this.lblConfirmMasterKey = new System.Windows.Forms.Label();
            this.tbConfirmMasterKey = new System.Windows.Forms.TextBox();
            this.cmbTargetDatabase = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPostScripts = new System.Windows.Forms.TextBox();
            this.btnSelectPost = new System.Windows.Forms.Button();
            this.lblPostScripts = new System.Windows.Forms.Label();
            this.tbSourceCollation = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSourceVersion = new System.Windows.Forms.Label();
            this.cmbSourceVersion = new System.Windows.Forms.ComboBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.cmbSourceDbs = new System.Windows.Forms.ComboBox();
            this.btnStartMigration = new System.Windows.Forms.Button();
            this.gbMigrationOptions.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbMigrationOptions
            // 
            this.gbMigrationOptions.Controls.Add(this.btnValidation);
            this.gbMigrationOptions.Controls.Add(this.progressBar);
            this.gbMigrationOptions.Controls.Add(this.tbExportPath);
            this.gbMigrationOptions.Controls.Add(this.lblExportPath);
            this.gbMigrationOptions.Controls.Add(this.groupBox1);
            this.gbMigrationOptions.Controls.Add(this.tbMasterKey);
            this.gbMigrationOptions.Controls.Add(this.lblMasterKey);
            this.gbMigrationOptions.Controls.Add(this.lblConfirmMasterKey);
            this.gbMigrationOptions.Controls.Add(this.tbConfirmMasterKey);
            this.gbMigrationOptions.Controls.Add(this.cmbTargetDatabase);
            this.gbMigrationOptions.Controls.Add(this.label1);
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
            this.gbMigrationOptions.Location = new System.Drawing.Point(12, 12);
            this.gbMigrationOptions.Name = "gbMigrationOptions";
            this.gbMigrationOptions.Size = new System.Drawing.Size(389, 447);
            this.gbMigrationOptions.TabIndex = 3;
            this.gbMigrationOptions.TabStop = false;
            this.gbMigrationOptions.Text = "Migration Options";
            // 
            // btnValidation
            // 
            this.btnValidation.Location = new System.Drawing.Point(288, 385);
            this.btnValidation.Name = "btnValidation";
            this.btnValidation.Size = new System.Drawing.Size(75, 23);
            this.btnValidation.TabIndex = 20;
            this.btnValidation.Text = "Validate";
            this.btnValidation.UseVisualStyleBackColor = true;
            this.btnValidation.Click += new System.EventHandler(this.btnValidation_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(6, 418);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(377, 23);
            this.progressBar.TabIndex = 19;
            // 
            // tbExportPath
            // 
            this.tbExportPath.Location = new System.Drawing.Point(103, 198);
            this.tbExportPath.Name = "tbExportPath";
            this.tbExportPath.Size = new System.Drawing.Size(260, 20);
            this.tbExportPath.TabIndex = 18;
            this.tbExportPath.Text = "C:\\veneka\\Migration";
            // 
            // lblExportPath
            // 
            this.lblExportPath.AutoSize = true;
            this.lblExportPath.Location = new System.Drawing.Point(8, 201);
            this.lblExportPath.Name = "lblExportPath";
            this.lblExportPath.Size = new System.Drawing.Size(62, 13);
            this.lblExportPath.TabIndex = 17;
            this.lblExportPath.Text = "Export Path";
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
            this.groupBox1.Location = new System.Drawing.Point(5, 222);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(378, 114);
            this.groupBox1.TabIndex = 16;
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
            // tbKeyExportPath
            // 
            this.tbKeyExportPath.Location = new System.Drawing.Point(102, 33);
            this.tbKeyExportPath.Name = "tbKeyExportPath";
            this.tbKeyExportPath.Size = new System.Drawing.Size(260, 20);
            this.tbKeyExportPath.TabIndex = 2;
            this.tbKeyExportPath.Text = "C:\\veneka\\Migration";
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
            // tbMasterKey
            // 
            this.tbMasterKey.Location = new System.Drawing.Point(103, 146);
            this.tbMasterKey.Name = "tbMasterKey";
            this.tbMasterKey.Size = new System.Drawing.Size(260, 20);
            this.tbMasterKey.TabIndex = 13;
            // 
            // lblMasterKey
            // 
            this.lblMasterKey.AutoSize = true;
            this.lblMasterKey.Location = new System.Drawing.Point(8, 149);
            this.lblMasterKey.Name = "lblMasterKey";
            this.lblMasterKey.Size = new System.Drawing.Size(60, 13);
            this.lblMasterKey.TabIndex = 12;
            this.lblMasterKey.Text = "Master Key";
            // 
            // lblConfirmMasterKey
            // 
            this.lblConfirmMasterKey.AutoSize = true;
            this.lblConfirmMasterKey.Location = new System.Drawing.Point(8, 175);
            this.lblConfirmMasterKey.Name = "lblConfirmMasterKey";
            this.lblConfirmMasterKey.Size = new System.Drawing.Size(63, 13);
            this.lblConfirmMasterKey.TabIndex = 14;
            this.lblConfirmMasterKey.Text = "Confirm Key";
            // 
            // tbConfirmMasterKey
            // 
            this.tbConfirmMasterKey.Location = new System.Drawing.Point(103, 172);
            this.tbConfirmMasterKey.Name = "tbConfirmMasterKey";
            this.tbConfirmMasterKey.Size = new System.Drawing.Size(260, 20);
            this.tbConfirmMasterKey.TabIndex = 15;
            // 
            // cmbTargetDatabase
            // 
            this.cmbTargetDatabase.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbTargetDatabase.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbTargetDatabase.FormattingEnabled = true;
            this.cmbTargetDatabase.Location = new System.Drawing.Point(103, 119);
            this.cmbTargetDatabase.Name = "cmbTargetDatabase";
            this.cmbTargetDatabase.Size = new System.Drawing.Size(260, 21);
            this.cmbTargetDatabase.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Target Database";
            // 
            // tbPostScripts
            // 
            this.tbPostScripts.Location = new System.Drawing.Point(103, 351);
            this.tbPostScripts.Name = "tbPostScripts";
            this.tbPostScripts.ReadOnly = true;
            this.tbPostScripts.Size = new System.Drawing.Size(204, 20);
            this.tbPostScripts.TabIndex = 7;
            this.tbPostScripts.Text = "C:\\veneka\\Migration\\PostScripts";
            // 
            // btnSelectPost
            // 
            this.btnSelectPost.Location = new System.Drawing.Point(314, 350);
            this.btnSelectPost.Name = "btnSelectPost";
            this.btnSelectPost.Size = new System.Drawing.Size(49, 22);
            this.btnSelectPost.TabIndex = 8;
            this.btnSelectPost.Text = "Select";
            this.btnSelectPost.UseVisualStyleBackColor = true;
            // 
            // lblPostScripts
            // 
            this.lblPostScripts.AutoSize = true;
            this.lblPostScripts.Location = new System.Drawing.Point(6, 354);
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
            // btnStartMigration
            // 
            this.btnStartMigration.Location = new System.Drawing.Point(140, 385);
            this.btnStartMigration.Name = "btnStartMigration";
            this.btnStartMigration.Size = new System.Drawing.Size(75, 23);
            this.btnStartMigration.TabIndex = 9;
            this.btnStartMigration.Text = "Do Migration";
            this.btnStartMigration.UseVisualStyleBackColor = true;
            this.btnStartMigration.Click += new System.EventHandler(this.btnStartMigration_Click);
            // 
            // MigrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 498);
            this.Controls.Add(this.gbMigrationOptions);
            this.Name = "MigrationForm";
            this.Text = "MigrationForm";
            this.gbMigrationOptions.ResumeLayout(false);
            this.gbMigrationOptions.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMigrationOptions;
        private System.Windows.Forms.TextBox tbPostScripts;
        private System.Windows.Forms.Button btnSelectPost;
        private System.Windows.Forms.Label lblPostScripts;
        private System.Windows.Forms.TextBox tbSourceCollation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSourceVersion;
        private System.Windows.Forms.ComboBox cmbSourceVersion;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.ComboBox cmbSourceDbs;
        private System.Windows.Forms.Button btnStartMigration;
        private System.Windows.Forms.ComboBox cmbTargetDatabase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbMasterKey;
        private System.Windows.Forms.Label lblMasterKey;
        private System.Windows.Forms.Label lblConfirmMasterKey;
        private System.Windows.Forms.TextBox tbConfirmMasterKey;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblConfirmExportKey;
        private System.Windows.Forms.CheckBox ckbExportKeys;
        private System.Windows.Forms.Label lblExportKey;
        private System.Windows.Forms.TextBox tbKeyExportPath;
        private System.Windows.Forms.Label lblEncrytionExport;
        private System.Windows.Forms.TextBox tbExportKey;
        private System.Windows.Forms.TextBox tbExportKeyConfirm;
        private System.Windows.Forms.TextBox tbExportPath;
        private System.Windows.Forms.Label lblExportPath;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnValidation;
    }
}