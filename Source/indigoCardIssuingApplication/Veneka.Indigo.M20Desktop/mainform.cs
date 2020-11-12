using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Veneka.Indigo.Integration.M20Printing;
using Common.Logging;

namespace Veneka.Indigo.M20Desktop
{
    public partial class mainform : Form
    {
        public mainform()
        {
            InitializeComponent();
            
           
        }

        private void mainform_Load(object sender, EventArgs e)
        {
            PrinterIntgerationController integrate = new PrinterIntgerationController("Test User");
          //  integrate.print_M20();
            MessageBox.Show("Hi Header", "Nice one");
        }
    }
}
