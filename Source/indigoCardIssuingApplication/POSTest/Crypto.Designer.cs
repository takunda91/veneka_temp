namespace POSTest
{
    partial class Crypto
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
            this.lblKey = new System.Windows.Forms.Label();
            this.tbKey = new System.Windows.Forms.TextBox();
            this.lblData = new System.Windows.Forms.Label();
            this.tbData = new System.Windows.Forms.TextBox();
            this.btn3DESDecrypt = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tbClearPinBlock = new System.Windows.Forms.TextBox();
            this.tbPAN = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnDecodePINBlock = new System.Windows.Forms.Button();
            this.tbPIN = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnEncodePINBlock = new System.Windows.Forms.Button();
            this.gbxXorComponents = new System.Windows.Forms.GroupBox();
            this.tbComponents = new System.Windows.Forms.TextBox();
            this.tbXORResult = new System.Windows.Forms.TextBox();
            this.btnXOR = new System.Windows.Forms.Button();
            this.gbxTripleDES = new System.Windows.Forms.GroupBox();
            this.btn3DESEncrypt = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.gbxPinBlock = new System.Windows.Forms.GroupBox();
            this.toolTipPinBlockEncode = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipDecodePINBlock = new System.Windows.Forms.ToolTip(this.components);
            this.gbxXorComponents.SuspendLayout();
            this.gbxTripleDES.SuspendLayout();
            this.gbxPinBlock.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblKey
            // 
            this.lblKey.AutoSize = true;
            this.lblKey.Location = new System.Drawing.Point(7, 21);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(25, 13);
            this.lblKey.TabIndex = 0;
            this.lblKey.Text = "Key";
            // 
            // tbKey
            // 
            this.tbKey.Font = new System.Drawing.Font("Lucida Console", 10F);
            this.tbKey.Location = new System.Drawing.Point(59, 19);
            this.tbKey.Name = "tbKey";
            this.tbKey.Size = new System.Drawing.Size(483, 21);
            this.tbKey.TabIndex = 1;
            // 
            // lblData
            // 
            this.lblData.AutoSize = true;
            this.lblData.Location = new System.Drawing.Point(7, 47);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(30, 13);
            this.lblData.TabIndex = 2;
            this.lblData.Text = "Data";
            // 
            // tbData
            // 
            this.tbData.Font = new System.Drawing.Font("Lucida Console", 10F);
            this.tbData.Location = new System.Drawing.Point(59, 45);
            this.tbData.Name = "tbData";
            this.tbData.Size = new System.Drawing.Size(483, 21);
            this.tbData.TabIndex = 3;
            // 
            // btn3DESDecrypt
            // 
            this.btn3DESDecrypt.Location = new System.Drawing.Point(170, 72);
            this.btn3DESDecrypt.Name = "btn3DESDecrypt";
            this.btn3DESDecrypt.Size = new System.Drawing.Size(75, 23);
            this.btn3DESDecrypt.TabIndex = 4;
            this.btn3DESDecrypt.Text = "Decrypt";
            this.btn3DESDecrypt.UseVisualStyleBackColor = true;
            this.btn3DESDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "PIN Block";
            // 
            // tbClearPinBlock
            // 
            this.tbClearPinBlock.Font = new System.Drawing.Font("Lucida Console", 10F);
            this.tbClearPinBlock.Location = new System.Drawing.Point(59, 103);
            this.tbClearPinBlock.Name = "tbClearPinBlock";
            this.tbClearPinBlock.Size = new System.Drawing.Size(483, 21);
            this.tbClearPinBlock.TabIndex = 11;
            // 
            // tbPAN
            // 
            this.tbPAN.Font = new System.Drawing.Font("Lucida Console", 10F);
            this.tbPAN.Location = new System.Drawing.Point(59, 45);
            this.tbPAN.Name = "tbPAN";
            this.tbPAN.Size = new System.Drawing.Size(483, 21);
            this.tbPAN.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(29, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "PAN";
            // 
            // btnDecodePINBlock
            // 
            this.btnDecodePINBlock.Location = new System.Drawing.Point(170, 74);
            this.btnDecodePINBlock.Name = "btnDecodePINBlock";
            this.btnDecodePINBlock.Size = new System.Drawing.Size(75, 23);
            this.btnDecodePINBlock.TabIndex = 14;
            this.btnDecodePINBlock.Text = "Decode";
            this.btnDecodePINBlock.UseVisualStyleBackColor = true;
            this.btnDecodePINBlock.Click += new System.EventHandler(this.btnDecodePINBlock_Click);
            // 
            // tbPIN
            // 
            this.tbPIN.Font = new System.Drawing.Font("Lucida Console", 10F);
            this.tbPIN.Location = new System.Drawing.Point(59, 19);
            this.tbPIN.Name = "tbPIN";
            this.tbPIN.Size = new System.Drawing.Size(483, 21);
            this.tbPIN.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(33, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "PIN";
            // 
            // btnEncodePINBlock
            // 
            this.btnEncodePINBlock.Location = new System.Drawing.Point(59, 72);
            this.btnEncodePINBlock.Name = "btnEncodePINBlock";
            this.btnEncodePINBlock.Size = new System.Drawing.Size(75, 23);
            this.btnEncodePINBlock.TabIndex = 17;
            this.btnEncodePINBlock.Text = "Encode";
            this.btnEncodePINBlock.UseVisualStyleBackColor = true;
            this.btnEncodePINBlock.Click += new System.EventHandler(this.btnEncodePINBlock_Click);
            // 
            // gbxXorComponents
            // 
            this.gbxXorComponents.Controls.Add(this.tbComponents);
            this.gbxXorComponents.Controls.Add(this.tbXORResult);
            this.gbxXorComponents.Controls.Add(this.btnXOR);
            this.gbxXorComponents.Location = new System.Drawing.Point(12, 13);
            this.gbxXorComponents.Name = "gbxXorComponents";
            this.gbxXorComponents.Size = new System.Drawing.Size(560, 200);
            this.gbxXorComponents.TabIndex = 18;
            this.gbxXorComponents.TabStop = false;
            this.gbxXorComponents.Text = "XOR Components";
            // 
            // tbComponents
            // 
            this.tbComponents.Font = new System.Drawing.Font("Lucida Console", 10F);
            this.tbComponents.Location = new System.Drawing.Point(59, 19);
            this.tbComponents.Multiline = true;
            this.tbComponents.Name = "tbComponents";
            this.tbComponents.Size = new System.Drawing.Size(483, 112);
            this.tbComponents.TabIndex = 4;
            this.tbComponents.TextChanged += new System.EventHandler(this.tbComponents_TextChanged);
            // 
            // tbXORResult
            // 
            this.tbXORResult.Font = new System.Drawing.Font("Lucida Console", 10F);
            this.tbXORResult.Location = new System.Drawing.Point(59, 166);
            this.tbXORResult.Name = "tbXORResult";
            this.tbXORResult.Size = new System.Drawing.Size(483, 21);
            this.tbXORResult.TabIndex = 2;
            // 
            // btnXOR
            // 
            this.btnXOR.Location = new System.Drawing.Point(59, 137);
            this.btnXOR.Name = "btnXOR";
            this.btnXOR.Size = new System.Drawing.Size(75, 23);
            this.btnXOR.TabIndex = 1;
            this.btnXOR.Text = "XOR";
            this.btnXOR.UseVisualStyleBackColor = true;
            this.btnXOR.Click += new System.EventHandler(this.btnXOR_Click);
            // 
            // gbxTripleDES
            // 
            this.gbxTripleDES.Controls.Add(this.btn3DESEncrypt);
            this.gbxTripleDES.Controls.Add(this.lblResult);
            this.gbxTripleDES.Controls.Add(this.tbResult);
            this.gbxTripleDES.Controls.Add(this.tbKey);
            this.gbxTripleDES.Controls.Add(this.lblKey);
            this.gbxTripleDES.Controls.Add(this.lblData);
            this.gbxTripleDES.Controls.Add(this.tbData);
            this.gbxTripleDES.Controls.Add(this.btn3DESDecrypt);
            this.gbxTripleDES.Location = new System.Drawing.Point(12, 219);
            this.gbxTripleDES.Name = "gbxTripleDES";
            this.gbxTripleDES.Size = new System.Drawing.Size(560, 141);
            this.gbxTripleDES.TabIndex = 19;
            this.gbxTripleDES.TabStop = false;
            this.gbxTripleDES.Text = "3DES";
            // 
            // btn3DESEncrypt
            // 
            this.btn3DESEncrypt.Location = new System.Drawing.Point(59, 72);
            this.btn3DESEncrypt.Name = "btn3DESEncrypt";
            this.btn3DESEncrypt.Size = new System.Drawing.Size(75, 23);
            this.btn3DESEncrypt.TabIndex = 7;
            this.btn3DESEncrypt.Text = "Encrypt";
            this.btn3DESEncrypt.UseVisualStyleBackColor = true;
            this.btn3DESEncrypt.Click += new System.EventHandler(this.btn3DESEncrypt_Click);
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(7, 103);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(37, 13);
            this.lblResult.TabIndex = 6;
            this.lblResult.Text = "Result";
            // 
            // tbResult
            // 
            this.tbResult.Font = new System.Drawing.Font("Lucida Console", 10F);
            this.tbResult.Location = new System.Drawing.Point(59, 101);
            this.tbResult.Name = "tbResult";
            this.tbResult.Size = new System.Drawing.Size(483, 21);
            this.tbResult.TabIndex = 5;
            // 
            // gbxPinBlock
            // 
            this.gbxPinBlock.Controls.Add(this.label5);
            this.gbxPinBlock.Controls.Add(this.tbClearPinBlock);
            this.gbxPinBlock.Controls.Add(this.tbPAN);
            this.gbxPinBlock.Controls.Add(this.btnEncodePINBlock);
            this.gbxPinBlock.Controls.Add(this.label6);
            this.gbxPinBlock.Controls.Add(this.label7);
            this.gbxPinBlock.Controls.Add(this.btnDecodePINBlock);
            this.gbxPinBlock.Controls.Add(this.tbPIN);
            this.gbxPinBlock.Location = new System.Drawing.Point(12, 366);
            this.gbxPinBlock.Name = "gbxPinBlock";
            this.gbxPinBlock.Size = new System.Drawing.Size(560, 136);
            this.gbxPinBlock.TabIndex = 20;
            this.gbxPinBlock.TabStop = false;
            this.gbxPinBlock.Text = "PIN Block";
            // 
            // Crypto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 512);
            this.Controls.Add(this.gbxPinBlock);
            this.Controls.Add(this.gbxTripleDES);
            this.Controls.Add(this.gbxXorComponents);
            this.Name = "Crypto";
            this.Text = "Crypto";
            this.Load += new System.EventHandler(this.Crypto_Load);
            this.gbxXorComponents.ResumeLayout(false);
            this.gbxXorComponents.PerformLayout();
            this.gbxTripleDES.ResumeLayout(false);
            this.gbxTripleDES.PerformLayout();
            this.gbxPinBlock.ResumeLayout(false);
            this.gbxPinBlock.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.TextBox tbKey;
        private System.Windows.Forms.Label lblData;
        private System.Windows.Forms.TextBox tbData;
        private System.Windows.Forms.Button btn3DESDecrypt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbClearPinBlock;
        private System.Windows.Forms.TextBox tbPAN;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnDecodePINBlock;
        private System.Windows.Forms.TextBox tbPIN;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnEncodePINBlock;
        private System.Windows.Forms.GroupBox gbxXorComponents;
        private System.Windows.Forms.GroupBox gbxTripleDES;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.GroupBox gbxPinBlock;
        private System.Windows.Forms.Button btn3DESEncrypt;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TextBox tbXORResult;
        private System.Windows.Forms.Button btnXOR;
        private System.Windows.Forms.TextBox tbComponents;
        private System.Windows.Forms.ToolTip toolTipPinBlockEncode;
        private System.Windows.Forms.ToolTip toolTipDecodePINBlock;
    }
}