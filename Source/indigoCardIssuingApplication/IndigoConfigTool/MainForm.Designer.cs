namespace IndigoConfigTool
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabZMKInject = new System.Windows.Forms.TabPage();
            this.tbComp2 = new System.Windows.Forms.TextBox();
            this.tbComp1 = new System.Windows.Forms.TextBox();
            this.lblComp2 = new System.Windows.Forms.Label();
            this.lblComp1 = new System.Windows.Forms.Label();
            this.tabWebMachineKeys = new System.Windows.Forms.TabPage();
            this.ddlComponentCount = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblEncrytedZMK = new System.Windows.Forms.Label();
            this.tbEncrytedZMK = new System.Windows.Forms.TextBox();
            this.lblConfigDir = new System.Windows.Forms.Label();
            this.errorComp = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnConnect = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tabZMKInject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorComp)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabZMKInject);
            this.tabControl1.Controls.Add(this.tabWebMachineKeys);
            this.tabControl1.Location = new System.Drawing.Point(12, 52);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 406);
            this.tabControl1.TabIndex = 0;
            // 
            // tabZMKInject
            // 
            this.tabZMKInject.Controls.Add(this.tbEncrytedZMK);
            this.tabZMKInject.Controls.Add(this.lblEncrytedZMK);
            this.tabZMKInject.Controls.Add(this.label4);
            this.tabZMKInject.Controls.Add(this.ddlComponentCount);
            this.tabZMKInject.Controls.Add(this.tbComp2);
            this.tabZMKInject.Controls.Add(this.tbComp1);
            this.tabZMKInject.Controls.Add(this.lblComp2);
            this.tabZMKInject.Controls.Add(this.lblComp1);
            this.tabZMKInject.Location = new System.Drawing.Point(4, 25);
            this.tabZMKInject.Name = "tabZMKInject";
            this.tabZMKInject.Padding = new System.Windows.Forms.Padding(3);
            this.tabZMKInject.Size = new System.Drawing.Size(792, 377);
            this.tabZMKInject.TabIndex = 0;
            this.tabZMKInject.Text = "ZMK Inject";
            this.tabZMKInject.UseVisualStyleBackColor = true;
            // 
            // tbComp2
            // 
            this.tbComp2.Location = new System.Drawing.Point(102, 120);
            this.tbComp2.Name = "tbComp2";
            this.tbComp2.Size = new System.Drawing.Size(329, 20);
            this.tbComp2.TabIndex = 4;
            // 
            // tbComp1
            // 
            this.tbComp1.Location = new System.Drawing.Point(102, 94);
            this.tbComp1.Name = "tbComp1";
            this.tbComp1.Size = new System.Drawing.Size(329, 20);
            this.tbComp1.TabIndex = 3;
            // 
            // lblComp2
            // 
            this.lblComp2.AutoSize = true;
            this.lblComp2.Location = new System.Drawing.Point(15, 123);
            this.lblComp2.Name = "lblComp2";
            this.lblComp2.Size = new System.Drawing.Size(70, 13);
            this.lblComp2.TabIndex = 1;
            this.lblComp2.Text = "Component 2";
            // 
            // lblComp1
            // 
            this.lblComp1.AutoSize = true;
            this.lblComp1.Location = new System.Drawing.Point(15, 97);
            this.lblComp1.Name = "lblComp1";
            this.lblComp1.Size = new System.Drawing.Size(70, 13);
            this.lblComp1.TabIndex = 0;
            this.lblComp1.Text = "Component 1";
            // 
            // tabWebMachineKeys
            // 
            this.tabWebMachineKeys.Location = new System.Drawing.Point(4, 25);
            this.tabWebMachineKeys.Name = "tabWebMachineKeys";
            this.tabWebMachineKeys.Padding = new System.Windows.Forms.Padding(3);
            this.tabWebMachineKeys.Size = new System.Drawing.Size(792, 329);
            this.tabWebMachineKeys.TabIndex = 1;
            this.tabWebMachineKeys.Text = "Machine Keys";
            this.tabWebMachineKeys.UseVisualStyleBackColor = true;
            // 
            // ddlComponentCount
            // 
            this.ddlComponentCount.FormattingEnabled = true;
            this.ddlComponentCount.Items.AddRange(new object[] {
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.ddlComponentCount.Location = new System.Drawing.Point(135, 63);
            this.ddlComponentCount.Name = "ddlComponentCount";
            this.ddlComponentCount.Size = new System.Drawing.Size(47, 21);
            this.ddlComponentCount.TabIndex = 2;
            this.ddlComponentCount.SelectedIndexChanged += new System.EventHandler(this.ddlComponentCount_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Number of Components";
            // 
            // lblEncrytedZMK
            // 
            this.lblEncrytedZMK.AutoSize = true;
            this.lblEncrytedZMK.Location = new System.Drawing.Point(15, 24);
            this.lblEncrytedZMK.Name = "lblEncrytedZMK";
            this.lblEncrytedZMK.Size = new System.Drawing.Size(81, 13);
            this.lblEncrytedZMK.TabIndex = 9;
            this.lblEncrytedZMK.Text = "Encrypted ZMK";
            // 
            // tbEncrytedZMK
            // 
            this.tbEncrytedZMK.Location = new System.Drawing.Point(102, 21);
            this.tbEncrytedZMK.Name = "tbEncrytedZMK";
            this.tbEncrytedZMK.Size = new System.Drawing.Size(329, 20);
            this.tbEncrytedZMK.TabIndex = 1;
            // 
            // lblConfigDir
            // 
            this.lblConfigDir.AutoSize = true;
            this.lblConfigDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblConfigDir.Location = new System.Drawing.Point(84, 9);
            this.lblConfigDir.Name = "lblConfigDir";
            this.lblConfigDir.Size = new System.Drawing.Size(62, 15);
            this.lblConfigDir.TabIndex = 1;
            this.lblConfigDir.Text = "lblConfigDir";
            // 
            // errorComp
            // 
            this.errorComp.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorComp.ContainerControl = this;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(3, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnConnect);
            this.panel1.Controls.Add(this.lblConfigDir);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 34);
            this.panel1.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 458);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Indigo Config";
            this.tabControl1.ResumeLayout(false);
            this.tabZMKInject.ResumeLayout(false);
            this.tabZMKInject.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorComp)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabZMKInject;
        private System.Windows.Forms.TextBox tbComp2;
        private System.Windows.Forms.TextBox tbComp1;
        private System.Windows.Forms.Label lblComp2;
        private System.Windows.Forms.Label lblComp1;
        private System.Windows.Forms.TabPage tabWebMachineKeys;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox ddlComponentCount;
        private System.Windows.Forms.TextBox tbEncrytedZMK;
        private System.Windows.Forms.Label lblEncrytedZMK;
        private System.Windows.Forms.Label lblConfigDir;
        private System.Windows.Forms.ErrorProvider errorComp;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnConnect;
    }
}

