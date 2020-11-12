using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndigoConfigTool
{
    public partial class MainForm : Form
    {
        private const string LABEL_COMP = "lblComp";
        private const string TEXTBOX_COMP = "tbComp";
        private const string ERROR_COMP = "errorComp";

        public MainForm()
        {
            InitializeComponent();
            this.ddlComponentCount.SelectedIndex = 0;
            BuildComponents();
        }

        #region ZMK Inject
        private void ddlComponentCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildComponents();
        }

        private void BuildComponents()
        {
            //Remove components
            this.tabControl1.TabPages[0].Controls.RemoveByKey("btnInject");

            for (int i = 3; i <= 9; i++)
            {
                this.tabControl1.TabPages[0].Controls.RemoveByKey(LABEL_COMP + i);
                this.tabControl1.TabPages[0].Controls.RemoveByKey(TEXTBOX_COMP + i);
                this.tabControl1.TabPages[0].Controls.RemoveByKey(ERROR_COMP + i);
            }

            //Add components
            int tbY = this.tbComp2.Location.Y;
            int tbYIncrement = this.tbComp2.Location.Y - this.tbComp1.Location.Y;

            int lblY = this.lblComp2.Location.Y;
            int lblYIncrement = this.lblComp2.Location.Y - this.lblComp1.Location.Y;

            int compCount = int.Parse(ddlComponentCount.SelectedItem.ToString());

            for (int i = 3; i <= compCount; i++)
            {
                lblY += lblYIncrement;
                tbY += tbYIncrement;

                var lblComp = new Label();
                lblComp.Name = LABEL_COMP + i;
                lblComp.Text = "Component " + i;
                lblComp.Location = new System.Drawing.Point(this.lblComp2.Location.X, lblY);
                lblComp.Size = new System.Drawing.Size(this.lblComp2.Size.Width, this.lblComp2.Size.Height);

                var tbComp = new TextBox();
                tbComp.Name = TEXTBOX_COMP + i;
                tbComp.TabIndex = i + 2;
                tbComp.Location = new System.Drawing.Point(this.tbComp2.Location.X, tbY);
                tbComp.Size = new System.Drawing.Size(this.tbComp2.Size.Width, this.tbComp2.Size.Height);

                this.tabControl1.TabPages[0].Controls.Add(lblComp);
                this.tabControl1.TabPages[0].Controls.Add(tbComp);
            }

            var btnInject = new Button();
            btnInject.Name = "btnInject";
            btnInject.Text = "Inject";
            btnInject.Location = new System.Drawing.Point(this.tbComp2.Location.X, tbY + tbYIncrement);
            btnInject.Size = new System.Drawing.Size(75, 23);
            btnInject.Click += new EventHandler(this.tbZMKInject_Click);
            this.tabControl1.TabPages[0].Controls.Add(btnInject);
        }

        private void tbZMKInject_Click(object sender, EventArgs e)
        {
            bool inError = false;
            errorComp.Clear();

            if (String.IsNullOrWhiteSpace(this.tbComp1.Text))
            {
                inError = true;
                errorComp.SetError(this.tbComp1, "Value Empty");
            }

            string zmkKey = tbComp1.Text.Trim().Replace(" ", "");

            int compCount = int.Parse(ddlComponentCount.SelectedItem.ToString());

            for (int i = 2; i <= compCount; i++)
            {
                var tbComp = (TextBox)this.tabControl1.TabPages[0].Controls[TEXTBOX_COMP + i];

                if (String.IsNullOrWhiteSpace(tbComp.Text))
                {
                    inError = true;
                    errorComp.SetError(tbComp, "Value Empty");
                }

                if (!inError)
                    zmkKey = Veneka.Indigo.Integration.Cryptography.Utility.XORHexStringsFull(zmkKey, tbComp.Text.Trim().Replace(" ", ""));
            }

            MessageBox.Show(zmkKey);
        }
        #endregion
    }
}
