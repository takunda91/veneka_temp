using IndigoDesktopApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IndigoDesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //http://stackoverflow.com/questions/19654295/wpf-mvvm-navigate-views
            //https://rachel53461.wordpress.com/2011/12/18/navigation-with-mvvm-2/
            InitializeComponent();
            
            this.DataContext = new MainViewModel();
        }        
    }
}
