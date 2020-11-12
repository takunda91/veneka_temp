namespace IndigoToolkit
{
    partial class CreateDatabaseForm
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
            this.gbCreateDB = new System.Windows.Forms.GroupBox();
            this.cmbTargetCollation = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblConfirmExportKey = new System.Windows.Forms.Label();
            this.ckbExportKeys = new System.Windows.Forms.CheckBox();
            this.lblExportKey = new System.Windows.Forms.Label();
            this.tbKeyExportPath = new System.Windows.Forms.TextBox();
            this.lblEncrytionExport = new System.Windows.Forms.Label();
            this.tbExportKey = new System.Windows.Forms.TextBox();
            this.tbExportKeyConfirm = new System.Windows.Forms.TextBox();
            this.btnCreateDatabase = new System.Windows.Forms.Button();
            this.cmbTargetDatabase = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbMasterKey = new System.Windows.Forms.TextBox();
            this.tbExportPath = new System.Windows.Forms.TextBox();
            this.lblMasterKey = new System.Windows.Forms.Label();
            this.lblExportPath = new System.Windows.Forms.Label();
            this.lblConfirmMasterKey = new System.Windows.Forms.Label();
            this.tbConfirmMasterKey = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.gbCreateDB.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbCreateDB
            // 
            this.gbCreateDB.Controls.Add(this.cmbTargetCollation);
            this.gbCreateDB.Controls.Add(this.label2);
            this.gbCreateDB.Controls.Add(this.groupBox1);
            this.gbCreateDB.Controls.Add(this.btnCreateDatabase);
            this.gbCreateDB.Controls.Add(this.cmbTargetDatabase);
            this.gbCreateDB.Controls.Add(this.label1);
            this.gbCreateDB.Controls.Add(this.tbMasterKey);
            this.gbCreateDB.Controls.Add(this.tbExportPath);
            this.gbCreateDB.Controls.Add(this.lblMasterKey);
            this.gbCreateDB.Controls.Add(this.lblExportPath);
            this.gbCreateDB.Controls.Add(this.lblConfirmMasterKey);
            this.gbCreateDB.Controls.Add(this.tbConfirmMasterKey);
            this.gbCreateDB.Enabled = false;
            this.gbCreateDB.Location = new System.Drawing.Point(12, 12);
            this.gbCreateDB.Name = "gbCreateDB";
            this.gbCreateDB.Size = new System.Drawing.Size(390, 352);
            this.gbCreateDB.TabIndex = 2;
            this.gbCreateDB.TabStop = false;
            this.gbCreateDB.Text = "Create Database";
            // 
            // cmbTargetCollation
            // 
            this.cmbTargetCollation.AllowDrop = true;
            this.cmbTargetCollation.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbTargetCollation.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbTargetCollation.FormattingEnabled = true;
            this.cmbTargetCollation.Location = new System.Drawing.Point(109, 46);
            this.cmbTargetCollation.Name = "cmbTargetCollation";
            this.cmbTargetCollation.Size = new System.Drawing.Size(260, 21);
            this.cmbTargetCollation.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 49);
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
            this.groupBox1.Location = new System.Drawing.Point(6, 170);
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
            // cmbTargetDatabase
            // 
            this.cmbTargetDatabase.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbTargetDatabase.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbTargetDatabase.FormattingEnabled = true;
            this.cmbTargetDatabase.Location = new System.Drawing.Point(109, 19);
            this.cmbTargetDatabase.Name = "cmbTargetDatabase";
            this.cmbTargetDatabase.Size = new System.Drawing.Size(260, 21);
            this.cmbTargetDatabase.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Database Name";
            // 
            // tbMasterKey
            // 
            this.tbMasterKey.Location = new System.Drawing.Point(109, 73);
            this.tbMasterKey.Name = "tbMasterKey";
            this.tbMasterKey.Size = new System.Drawing.Size(260, 20);
            this.tbMasterKey.TabIndex = 7;
            // 
            // tbExportPath
            // 
            this.tbExportPath.Location = new System.Drawing.Point(109, 125);
            this.tbExportPath.Name = "tbExportPath";
            this.tbExportPath.Size = new System.Drawing.Size(260, 20);
            this.tbExportPath.TabIndex = 11;
            this.tbExportPath.Text = "C:\\veneka\\Migration";
            // 
            // lblMasterKey
            // 
            this.lblMasterKey.AutoSize = true;
            this.lblMasterKey.Location = new System.Drawing.Point(14, 76);
            this.lblMasterKey.Name = "lblMasterKey";
            this.lblMasterKey.Size = new System.Drawing.Size(60, 13);
            this.lblMasterKey.TabIndex = 6;
            this.lblMasterKey.Text = "Master Key";
            // 
            // lblExportPath
            // 
            this.lblExportPath.AutoSize = true;
            this.lblExportPath.Location = new System.Drawing.Point(14, 128);
            this.lblExportPath.Name = "lblExportPath";
            this.lblExportPath.Size = new System.Drawing.Size(62, 13);
            this.lblExportPath.TabIndex = 10;
            this.lblExportPath.Text = "Export Path";
            // 
            // lblConfirmMasterKey
            // 
            this.lblConfirmMasterKey.AutoSize = true;
            this.lblConfirmMasterKey.Location = new System.Drawing.Point(14, 102);
            this.lblConfirmMasterKey.Name = "lblConfirmMasterKey";
            this.lblConfirmMasterKey.Size = new System.Drawing.Size(63, 13);
            this.lblConfirmMasterKey.TabIndex = 8;
            this.lblConfirmMasterKey.Text = "Confirm Key";
            // 
            // tbConfirmMasterKey
            // 
            this.tbConfirmMasterKey.Location = new System.Drawing.Point(109, 99);
            this.tbConfirmMasterKey.Name = "tbConfirmMasterKey";
            this.tbConfirmMasterKey.Size = new System.Drawing.Size(260, 20);
            this.tbConfirmMasterKey.TabIndex = 9;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 384);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(390, 23);
            this.progressBar.TabIndex = 4;
            // 
            // CreateDatabaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 502);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.gbCreateDB);
            this.Name = "CreateDatabaseForm";
            this.Text = "CreateDatabase";
            this.Load += new System.EventHandler(this.CreateDatabaseForm_Load);
            this.gbCreateDB.ResumeLayout(false);
            this.gbCreateDB.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbCreateDB;
        private System.Windows.Forms.ComboBox cmbTargetCollation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblConfirmExportKey;
        private System.Windows.Forms.CheckBox ckbExportKeys;
        private System.Windows.Forms.Label lblExportKey;
        private System.Windows.Forms.TextBox tbKeyExportPath;
        private System.Windows.Forms.Label lblEncrytionExport;
        private System.Windows.Forms.TextBox tbExportKey;
        private System.Windows.Forms.TextBox tbExportKeyConfirm;
        private System.Windows.Forms.Button btnCreateDatabase;
        private System.Windows.Forms.ComboBox cmbTargetDatabase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbMasterKey;
        private System.Windows.Forms.TextBox tbExportPath;
        private System.Windows.Forms.Label lblMasterKey;
        private System.Windows.Forms.Label lblExportPath;
        private System.Windows.Forms.Label lblConfirmMasterKey;
        private System.Windows.Forms.TextBox tbConfirmMasterKey;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}