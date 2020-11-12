using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Veneka.Indigo.COMS.Core.Integration;

namespace Veneka.Indigo.COMS.Core.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            IManagementAPI _proxy =null;
            WSHttpBinding binding = new WSHttpBinding();
            binding.SendTimeout = new TimeSpan(0,10,0);
            binding.OpenTimeout = new TimeSpan(0, 10, 0);

            binding.Security.Mode = SecurityMode.Transport;

            EndpointAddress endpoint = new EndpointAddress("https://localhost:8443/ManagementAPI.svc");

           
            ChannelFactory<IManagementAPI> factory = new ChannelFactory<IManagementAPI>(binding, endpoint);
           
            _proxy =factory.CreateChannel();
            IgnoreUntrustedSSL();
            string path = @"C:\veneka\ComsCore\integration.rar";
            _proxy.ReloadIntegration(File.ReadAllBytes(path), CalculateMD5(path));

        }
        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        private void IgnoreUntrustedSSL()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3;

            ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate,
                                                                        X509Chain chain,
                                                                        SslPolicyErrors sslPolicyErrors) => true;
        }
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "dll files (*.dll) |*.dll";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
              
                foreach (string filename in openFileDialog.FileNames)
                {
                    lbFiles.Items.Add(System.IO.Path.GetFileName(filename));
                    DirectoryInfo directory = new DirectoryInfo(@"C:\veneka\ComsCore\integration\temp");
                    if(!directory.Exists)
                    {
                        directory.Create();
                    }
                    System.IO.File.Copy(filename,directory.FullName+ "\\"+System.IO.Path.GetFileName(filename),true);
                }
            }

        }
    }
}
