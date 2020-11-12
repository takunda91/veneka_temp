using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Veneka.Indigo.Desktop.Printer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string text = File.ReadAllText(@"C:\temp\cardname.txt");
           
            MessageBox.Show("Contents of card_name.txt = " + text);
            PrinterIntgerationController printer_integration = new PrinterIntgerationController("Taku");
            printer_integration.print_M20();
        }
    }
}
