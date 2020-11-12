using Catel.Windows;
using System.Windows.Controls;

namespace IndigoToolkit2.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null)
            {
                return;
            }

            tb.ScrollToEnd();
            // set selection to end of document
            //tb.SelectionStart = int.MaxValue;
            //tb.SelectionLength = 0;
        }
    }
}
